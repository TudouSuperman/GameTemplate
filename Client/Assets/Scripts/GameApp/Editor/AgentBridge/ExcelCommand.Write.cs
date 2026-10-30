using System;
using System.Collections.Generic;
using System.Linq;
using AgentBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace GameApp.Editor
{
    public sealed partial class ExcelCommand
    {
        private static object SetCells(JObject input)
        {
            return RunWorkbook(input, "set_cells", session =>
            {
                ValidateVersion(session, input);
                ExcelWorksheet sheet = RequireSheet(session, input);
                JArray changes = input?["changes"] as JArray;
                if (changes == null || changes.Count < 1 || changes.Count > 500) throw Invalid("changes requires 1..500 cells.");
                var addresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var results = new JArray();
                int changedCount = 0;
                foreach (JObject change in changes)
                {
                    ExcelAddress address = Address(GetString(change, "cell", null));
                    if (!addresses.Add(address.Address)) throw Invalid("Duplicate cell: " + address.Address);
                    ExcelRange cell = sheet.Cells[address.Address];
                    bool hasValue = change.Property("value") != null;
                    bool hasFormula = change.Property("formula") != null;
                    if (hasValue == hasFormula) throw Invalid("Supply exactly one of value or formula.");
                    JObject previous = Snapshot(cell);
                    bool expectedValue = change.Property("expectedValue") != null;
                    bool expectedFormula = change.Property("expectedFormula") != null;
                    if (expectedValue && expectedFormula) throw Invalid("Supply only one expectedValue or expectedFormula.");
                    if ((expectedValue && (!string.IsNullOrEmpty(cell.Formula) || !EqualValue(previous["value"], change["expectedValue"]))) ||
                        (expectedFormula && cell.Formula != Formula(change.Value<string>("expectedFormula"))))
                        throw new CommandException(ConflictErrorCode, "Old cell value/formula does not match: " + address.Address);
                    JToken value = hasFormula ? new JObject { ["formula"] = change["formula"] } : change["value"];
                    bool changed = !Matches(cell, value);
                    if (changed) { Assign(cell, value); changedCount++; }
                    results.Add(new JObject { ["cell"] = address.Address, ["changed"] = changed,
                        ["previous"] = previous, ["current"] = Snapshot(cell) });
                }
                return new JObject { ["sheet"] = sheet.Name, ["changed"] = changedCount > 0,
                    ["requestedCount"] = changes.Count, ["changedCount"] = changedCount, ["cells"] = results };
            });
        }

        private static string RowKey(IEnumerable<JToken> values)
        {
            // Preserve value types while treating Excel numeric 1 and JSON integer 1 as the same key.
            return new JArray(values.Select(value => value != null && (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
                ? new JValue(value.Value<double>()) : value?.DeepClone() ?? JValue.CreateNull())).ToString(Formatting.None);
        }

        private static object UpsertRows(JObject input)
        {
            return RunWorkbook(input, "upsert_rows", session =>
            {
                ValidateVersion(session, input);
                ExcelWorksheet sheet = RequireSheet(session, input);
                int header = HeaderRow(session, sheet, input);
                var headers = Headers(sheet, header);
                JArray keyArray = input?["keyColumns"] as JArray;
                if (keyArray == null || keyArray.Count < 1 || keyArray.Count > 8) throw Invalid("keyColumns requires 1..8 headers.");
                string[] keys = Columns(keyArray, headers);
                JArray requestedRows = input?["rows"] as JArray;
                if (requestedRows == null || requestedRows.Count < 1 || requestedRows.Count > 200) throw Invalid("rows requires 1..200 objects.");
                int start = DataStartRow(session, sheet, input, header);
                int end = Math.Max(start - 1, sheet.Dimension?.End.Row ?? 0);
                if (end - start + 1 > MaxScanRows) throw Invalid("upsert_rows indexes at most 100000 rows.");
                var existing = new Dictionary<string, int>(StringComparer.Ordinal);
                int lastDataRow = start - 1;
                for (int row = start; row <= end; row++)
                {
                    if (headers.Values.Any(column => sheet.Cells[row, column].Value != null || !string.IsNullOrEmpty(sheet.Cells[row, column].Formula)))
                        lastDataRow = row;
                    JToken[] values = keys.Select(key => Snapshot(sheet.Cells[row, headers[key]])["value"]).ToArray();
                    if (values.All(value => value.Type == JTokenType.Null || (value.Type == JTokenType.String && value.Value<string>().Length == 0))) continue;
                    string keyValue = RowKey(values);
                    if (existing.ContainsKey(keyValue)) throw Invalid("Duplicate existing key at rows " + existing[keyValue] + " and " + row + ".");
                    existing.Add(keyValue, row);
                }
                bool copyTemplate = input?["copyTemplate"]?.Value<bool>() ?? true;
                int template = GetInt(input, "templateRow", lastDataRow);
                if (copyTemplate && input?["templateRow"] != null && (template < start || template > end))
                    throw Invalid("templateRow must refer to an existing data row.");
                var inputKeys = new HashSet<string>(StringComparer.Ordinal);
                var results = new JArray();
                int inserted = 0, updated = 0, unchanged = 0, changedCells = 0;
                foreach (JObject values in requestedRows)
                {
                    if (values.Count == 0 || values.Properties().Any(property => !headers.ContainsKey(property.Name)))
                        throw Invalid("Row values must use existing header names.");
                    foreach (string key in keys)
                    {
                        JToken keyValue = values[key];
                        if (keyValue == null || keyValue.Type == JTokenType.Null || keyValue is JContainer ||
                            (keyValue.Type == JTokenType.String && string.IsNullOrWhiteSpace(keyValue.Value<string>())))
                            throw Invalid("Each row requires non-empty literal key column: " + key);
                    }
                    string rowKey = RowKey(keys.Select(key => values[key]));
                    if (!inputKeys.Add(rowKey)) throw Invalid("Duplicate input row key.");
                    bool isInsert = !existing.TryGetValue(rowKey, out int row);
                    bool copied = false;
                    if (isInsert)
                    {
                        row = ++lastDataRow;
                        if (row > 1048576) throw Invalid("Worksheet row limit reached.");
                        // Copy styles/formulas with EPPlus's relative formula translation; never copy template constants.
                        if (copyTemplate && template >= start)
                        {
                            int columns = sheet.Dimension.End.Column;
                            sheet.Cells[template, 1, template, columns].Copy(sheet.Cells[row, 1, row, columns]);
                            sheet.Row(row).Height = sheet.Row(template).Height;
                            for (int column = 1; column <= columns; column++)
                                if (string.IsNullOrEmpty(sheet.Cells[row, column].Formula)) sheet.Cells[row, column].Value = null;
                            copied = true;
                        }
                        existing.Add(rowKey, row);
                        inserted++;
                    }
                    var cells = new JArray();
                    foreach (JProperty property in values.Properties())
                    {
                        ExcelRange cell = sheet.Cells[row, headers[property.Name]];
                        if (Matches(cell, property.Value)) continue;
                        JObject previous = Snapshot(cell);
                        Assign(cell, property.Value);
                        changedCells++;
                        cells.Add(new JObject { ["column"] = property.Name, ["cell"] = cell.Address,
                            ["previous"] = previous, ["current"] = Snapshot(cell) });
                    }
                    if (!isInsert) { if (cells.Count > 0) updated++; else unchanged++; }
                    results.Add(new JObject { ["row"] = row, ["inserted"] = isInsert,
                        ["templateCopied"] = copied, ["cells"] = cells });
                }
                return new JObject { ["sheet"] = sheet.Name, ["headerRow"] = header, ["dataStartRow"] = start,
                    ["changed"] = inserted + updated > 0, ["insertedCount"] = inserted, ["updatedCount"] = updated,
                    ["unchangedCount"] = unchanged, ["changedCellCount"] = changedCells, ["rows"] = results };
            });
        }
    }
}
