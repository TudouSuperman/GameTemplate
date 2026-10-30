using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AgentBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using UnityEngine;

namespace GameApp.Editor
{
    public sealed partial class ExcelCommand
    {
        private const int MaxResponseCharacters = 600000;
        private const int MaxWorkbookBytes = 32 * 1024 * 1024;
        private const int MaxScanRows = 100000;
        private static readonly Regex s_CellAddress = new Regex(@"^[A-Za-z]{1,3}[1-9][0-9]{0,6}$");

        private static string GetString(JObject input, string name, string fallback)
        {
            return input?[name]?.Value<string>() ?? fallback;
        }

        private static int GetInt(JObject input, string name, int fallback)
        {
            return input?[name]?.Value<int?>() ?? fallback;
        }

        private static CommandException Invalid(string message)
        {
            return new CommandException(ErrorCodes.InvalidParams, message);
        }

        private static string Hash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
            }
        }

        private static byte[] ReadWorkbookBytes(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length > MaxWorkbookBytes)
                {
                    throw Invalid("Workbook exceeds the 32 MiB limit.");
                }
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }

        private sealed class WorkbookSession : IDisposable
        {
            public readonly string FullPath;
            public readonly string RelativePath;
            public readonly string Version;
            public readonly ExcelPackage Package;

            public WorkbookSession(JObject input)
            {
                string requested = GetString(input, "filePath", null);
                if (string.IsNullOrWhiteSpace(requested)) throw Invalid("Missing filePath.");
                string root = Directory.GetParent(Application.dataPath).Parent.FullName;
                FullPath = Path.GetFullPath(Path.IsPathRooted(requested) ? requested : Path.Combine(root, requested));
                string prefix = root + Path.DirectorySeparatorChar;
                if (!FullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    throw Invalid("filePath must stay inside the repository.");
                if (!string.Equals(Path.GetExtension(FullPath), ".xlsx", StringComparison.OrdinalIgnoreCase))
                    throw Invalid("Only existing .xlsx workbooks are supported by this project's EPPlus library.");
                if (!File.Exists(FullPath)) throw Invalid("Workbook not found: " + requested);
                // Reject junctions/symlinks so lexical containment also guarantees filesystem containment.
                for (string current = FullPath; !string.Equals(current, root, StringComparison.OrdinalIgnoreCase);
                     current = Path.GetDirectoryName(current))
                {
                    if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                        throw Invalid("Workbook paths cannot traverse symlinks or junctions.");
                }
                RelativePath = FullPath.Substring(prefix.Length).Replace('\\', '/');
                byte[] bytes = ReadWorkbookBytes(FullPath);
                Version = Hash(bytes);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                Package = new ExcelPackage(new MemoryStream(bytes));
            }

            public void EnsureUnchanged()
            {
                if (Hash(ReadWorkbookBytes(FullPath)) != Version)
                    throw new CommandException(ConflictErrorCode, "Workbook changed; query it again before writing.");
            }

            public string Save()
            {
                Package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                byte[] bytes = Package.GetAsByteArray();
                if (bytes.Length > MaxWorkbookBytes) throw Invalid("Updated workbook exceeds 32 MiB.");
                string temporary = Path.Combine(Path.GetDirectoryName(FullPath), "." + Guid.NewGuid().ToString("N") + ".tmp");
                try
                {
                    using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush(true);
                    }
                    EnsureUnchanged();
                    File.Replace(temporary, FullPath, null);
                    return Hash(bytes);
                }
                finally
                {
                    if (File.Exists(temporary)) File.Delete(temporary);
                }
            }

            public void Dispose() { Package.Dispose(); }
        }

        private static object RunWorkbook(JObject input, string action, Func<WorkbookSession, JObject> operation)
        {
            try
            {
                using (var session = new WorkbookSession(input))
                {
                    JObject result = operation(session);
                    result["action"] = action;
                    result["filePath"] = session.RelativePath;
                    if (result.ToString(Formatting.None).Length > MaxResponseCharacters)
                        throw Invalid("Result is too large; narrow the query or split the changes.");
                    session.EnsureUnchanged();
                    if (action == "set_cells" || action == "upsert_rows")
                    {
                        bool dryRun = input?["dryRun"]?.Value<bool>() ?? false;
                        bool changed = result.Value<bool>("changed");
                        result.Remove("changed");
                        result["dryRun"] = dryRun;
                        result["saved"] = changed && !dryRun;
                        result["versionBefore"] = session.Version;
                        result["versionAfter"] = changed && !dryRun ? session.Save() : session.Version;
                    }
                    else result["version"] = session.Version;
                    return result;
                }
            }
            catch (CommandException) { throw; }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException)
            { throw Invalid(ex.Message); }
            catch (Exception ex)
            { throw new CommandException(ErrorCode, "Excel operation failed. Close the workbook in Excel and check its format. " + ex.Message); }
        }

        private static ExcelWorksheet RequireSheet(WorkbookSession session, JObject input)
        {
            string name = GetString(input, "sheet", null);
            if (string.IsNullOrWhiteSpace(name)) throw Invalid("Missing sheet.");
            return session.Package.Workbook.Worksheets[name] ?? throw Invalid("Sheet not found: " + name);
        }

        private static ExcelAddress Address(string address, bool range = false)
        {
            string[] parts = (address ?? "").Split(':');
            if (parts.Length > (range ? 2 : 1) || parts.Any(part => !s_CellAddress.IsMatch(part)))
                throw Invalid("Expected an A1 " + (range ? "range: " : "cell: ") + address);
            var parsed = new ExcelAddress(address);
            if (parsed.Start.Row > parsed.End.Row || parsed.Start.Column > parsed.End.Column ||
                parsed.End.Row > 1048576 || parsed.End.Column > 16384)
                throw Invalid("Cell address is outside worksheet bounds: " + address);
            return parsed;
        }

        private static JObject Snapshot(ExcelRangeBase cell)
        {
            object value = cell.Value;
            string type = !string.IsNullOrEmpty(cell.Formula) ? "formula" : value == null ? "blank" :
                value is bool ? "boolean" : value is DateTime ? "date" : value is string ? "string" :
                value is ExcelErrorValue ? "error" : "numeric";
            return new JObject { ["type"] = type, ["value"] = value == null ? JValue.CreateNull() :
                value is ExcelErrorValue ? new JValue(value.ToString()) : JToken.FromObject(value),
                ["formula"] = string.IsNullOrEmpty(cell.Formula) ? JValue.CreateNull() : new JValue(cell.Formula) };
        }

        private static bool EqualValue(JToken left, JToken right)
        {
            if (left == null || left.Type == JTokenType.Null) return right == null || right.Type == JTokenType.Null;
            if (right == null || right.Type == JTokenType.Null) return false;
            if ((left.Type == JTokenType.Integer || left.Type == JTokenType.Float) &&
                (right.Type == JTokenType.Integer || right.Type == JTokenType.Float))
                return left.Value<double>().Equals(right.Value<double>());
            return JToken.DeepEquals(left, right);
        }

        private static string Formula(string formula)
        {
            formula = formula?.Trim();
            if (formula?.StartsWith("=", StringComparison.Ordinal) == true) formula = formula.Substring(1);
            if (string.IsNullOrWhiteSpace(formula) || formula.Length > 8192) throw Invalid("Invalid formula.");
            return formula;
        }

        private static void Assign(ExcelRangeBase cell, JToken value)
        {
            if (value is JObject expression)
            {
                if (expression.Count != 1 || expression["formula"]?.Type != JTokenType.String)
                    throw Invalid("Formula values must contain only a string formula property.");
                cell.Value = null;
                cell.Formula = Formula(expression.Value<string>("formula"));
                return;
            }
            cell.Formula = "";
            if (value == null || value.Type == JTokenType.Null) cell.Value = null;
            else if (value.Type == JTokenType.String) cell.Value = value.Value<string>();
            else if (value.Type == JTokenType.Boolean) cell.Value = value.Value<bool>();
            else if (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
            {
                double number = value.Value<double>();
                if (double.IsNaN(number) || double.IsInfinity(number)) throw Invalid("Numeric values must be finite.");
                cell.Value = number;
            }
            else throw Invalid("Cell value must be null, string, boolean, number, or a formula object.");
        }

        private static bool Matches(ExcelRangeBase cell, JToken value)
        {
            return value is JObject expression ? cell.Formula == Formula(expression.Value<string>("formula")) :
                string.IsNullOrEmpty(cell.Formula) && EqualValue(Snapshot(cell)["value"], value);
        }

        private static void ValidateVersion(WorkbookSession session, JObject input)
        {
            string expected = GetString(input, "expectedVersion", null);
            if (expected != null && !string.Equals(expected, session.Version, StringComparison.OrdinalIgnoreCase))
                throw new CommandException(ConflictErrorCode, "expectedVersion does not match; query the workbook again.");
        }

        private static int HeaderRow(WorkbookSession session, ExcelWorksheet sheet, JObject input)
        {
            if (input?["headerRow"] != null) return GetInt(input, "headerRow", 1);
            for (int row = 1; row <= Math.Min(sheet.Dimension?.End.Row ?? 0, 20); row++)
                if (sheet.Cells[row, 1].Text.Trim() == "##var") return row;
            return session.RelativePath.StartsWith("ClientExcel/", StringComparison.OrdinalIgnoreCase)
                ? DataTableConfig.GetDataTableConfig().NameRow + 1 : 1;
        }

        private static int DataStartRow(WorkbookSession session, ExcelWorksheet sheet, JObject input, int header)
        {
            int row;
            if (input?["dataStartRow"] != null) row = GetInt(input, "dataStartRow", header + 1);
            else if (sheet.Cells[header, 1].Text.Trim() == "##var") row = header + 3;
            else if (session.RelativePath.StartsWith("ClientExcel/", StringComparison.OrdinalIgnoreCase))
                row = DataTableConfig.GetDataTableConfig().ContentStartRow + 1;
            else row = header + 1;
            if (row <= header || row > 1048576) throw Invalid("dataStartRow must follow headerRow and stay within worksheet bounds.");
            return row;
        }

        private static Dictionary<string, int> Headers(ExcelWorksheet sheet, int row)
        {
            if (row < 1 || row > 1048576) throw Invalid("Invalid headerRow.");
            var result = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int column = 1; column <= (sheet.Dimension?.End.Column ?? 0); column++)
            {
                string name = sheet.Cells[row, column].Text.Trim();
                if (name.Length == 0) continue;
                if (result.ContainsKey(name)) throw Invalid("Duplicate header: " + name);
                result.Add(name, column);
            }
            if (result.Count == 0) throw Invalid("Header row is empty.");
            return result;
        }

        private static string[] Columns(JArray requested, Dictionary<string, int> headers)
        {
            string[] columns = requested == null ? headers.Keys.Where(name => !name.StartsWith("##", StringComparison.Ordinal)).ToArray() :
                requested.Values<string>().ToArray();
            if (columns.Length == 0 || columns.Distinct(StringComparer.Ordinal).Count() != columns.Length ||
                columns.Any(name => !headers.ContainsKey(name))) throw Invalid("Columns must be unique, existing headers.");
            return columns;
        }
    }
}
