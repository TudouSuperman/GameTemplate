using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace GameApp.Editor
{
    public sealed partial class ExcelCommand
    {
        private static object Inspect(JObject input)
        {
            return RunWorkbook(input, "inspect", session =>
            {
                var sheets = new JArray();
                foreach (ExcelWorksheet sheet in session.Package.Workbook.Worksheets)
                {
                    int header = HeaderRow(session, sheet, new JObject());
                    sheets.Add(new JObject
                    {
                        ["name"] = sheet.Name,
                        ["firstRow"] = sheet.Dimension?.Start.Row,
                        ["lastRow"] = sheet.Dimension?.End.Row,
                        ["firstColumn"] = sheet.Dimension?.Start.Column,
                        ["lastColumn"] = sheet.Dimension?.End.Column,
                        ["mergedRegionCount"] = sheet.MergedCells.Count,
                        ["dataValidationCount"] = sheet.DataValidations.Count,
                        ["suggestedHeaderRow"] = header,
                        ["suggestedDataStartRow"] = DataStartRow(session, sheet, new JObject(), header)
                    });
                }
                return new JObject { ["format"] = "xlsx", ["sheetCount"] = sheets.Count, ["sheets"] = sheets };
            });
        }

        private static object ReadRange(JObject input)
        {
            return RunWorkbook(input, "read_range", session =>
            {
                ExcelWorksheet sheet = RequireSheet(session, input);
                ExcelAddress range = Address(GetString(input, "range", null), true);
                long count = (long)(range.End.Row - range.Start.Row + 1) * (range.End.Column - range.Start.Column + 1);
                int maxCells = GetInt(input, "maxCells", 500);
                if (maxCells < 1 || maxCells > 2000 || count > maxCells) throw Invalid("Range exceeds maxCells (1..2000).");
                bool includeEmpty = input?["includeEmpty"]?.Value<bool>() ?? true;
                var cells = new JArray();
                for (int row = range.Start.Row; row <= range.End.Row; row++)
                for (int column = range.Start.Column; column <= range.End.Column; column++)
                {
                    ExcelRange cell = sheet.Cells[row, column];
                    JObject snapshot = Snapshot(cell);
                    if (!includeEmpty && snapshot.Value<string>("type") == "blank") continue;
                    snapshot["cell"] = cell.Address;
                    snapshot["row"] = row;
                    snapshot["column"] = column;
                    cells.Add(snapshot);
                }
                return new JObject { ["sheet"] = sheet.Name, ["range"] = range.Address,
                    ["requestedCellCount"] = count, ["returnedCellCount"] = cells.Count,
                    ["includeEmpty"] = includeEmpty, ["cells"] = cells };
            });
        }

        private static object FindRows(JObject input)
        {
            return RunWorkbook(input, "find_rows", session =>
            {
                ExcelWorksheet sheet = RequireSheet(session, input);
                int header = HeaderRow(session, sheet, input);
                var headers = Headers(sheet, header);
                string[] selected = Columns(input?["select"] as JArray, headers);
                JObject where = input?["where"] as JObject;
                if (where == null || where.Count == 0 || where.Properties().Any(p => !headers.ContainsKey(p.Name)))
                    throw Invalid("where must contain existing header names.");
                int start = DataStartRow(session, sheet, input, header);
                int end = GetInt(input, "endRow", Math.Max(start, sheet.Dimension?.End.Row ?? start));
                if (end < start || end > 1048576 || end - start + 1 > MaxScanRows) throw Invalid("Query scans at most 100000 rows.");
                int offset = GetInt(input, "offset", 0);
                int limit = GetInt(input, "limit", 50);
                if (offset < 0 || limit < 1 || limit > 200) throw Invalid("Invalid offset or limit (1..200).");
                string match = GetString(input, "match", "exact");
                if (match != "exact" && match != "contains") throw Invalid("match must be exact or contains.");
                bool ignoreCase = input?["ignoreCase"]?.Value<bool>() ?? false;
                StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                var rows = new JArray();
                int matched = 0;
                for (int row = start; row <= end; row++)
                {
                    bool matches = where.Properties().All(condition =>
                    {
                        ExcelRange cell = sheet.Cells[row, headers[condition.Name]];
                        if (match == "contains")
                            return cell.Text.IndexOf(condition.Value.Type == JTokenType.Null ? "" : condition.Value.ToString(), comparison) >= 0;
                        JToken actual = Snapshot(cell)["value"];
                        return actual.Type == JTokenType.String && condition.Value.Type == JTokenType.String
                            ? string.Equals(actual.Value<string>(), condition.Value.Value<string>(), comparison)
                            : EqualValue(actual, condition.Value);
                    });
                    if (!matches) continue;
                    if (matched++ < offset || rows.Count >= limit) continue;
                    var values = new JObject();
                    var formulas = new JObject();
                    foreach (string name in selected)
                    {
                        JObject snapshot = Snapshot(sheet.Cells[row, headers[name]]);
                        values[name] = snapshot["value"];
                        if (snapshot["formula"].Type != JTokenType.Null) formulas[name] = snapshot["formula"];
                    }
                    rows.Add(new JObject { ["row"] = row, ["values"] = values, ["formulas"] = formulas });
                }
                return new JObject { ["sheet"] = sheet.Name, ["headerRow"] = header, ["dataStartRow"] = start,
                    ["endRow"] = end, ["offset"] = offset, ["limit"] = limit, ["scannedCount"] = end - start + 1,
                    ["matchedCount"] = matched, ["returnedCount"] = rows.Count, ["hasMore"] = offset + rows.Count < matched,
                    ["rows"] = rows };
            });
        }
    }
}
