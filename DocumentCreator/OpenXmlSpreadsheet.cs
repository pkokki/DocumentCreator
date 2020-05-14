using DocumentCreator.Core.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using JsonExcelExpressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public static class OpenXmlSpreadsheet
    {
        public static MappingInfo GetMappingInfo(Stream mappingsStream, IEnumerable<EvaluationSource> externalSources)
        {
            using var mappingsDoc = SpreadsheetDocument.Open(mappingsStream, false);
            return GetMappingInfo(mappingsDoc, externalSources);
        }

        public static Stream FillMappingsSheet(Stream mappingBytes, IEnumerable<TemplateField> templateFields, FillMappingInfo mappingInfo)
        {
            using var mappingsStream = mappingBytes.ToMemoryStream();
            using (SpreadsheetDocument mappingsDoc = SpreadsheetDocument.Open(mappingsStream, true))
            {
                FillMappingsSheet(mappingsDoc, templateFields, mappingInfo);
            }
            return mappingsStream.ToMemoryStream();
        }

        public static MappingInfo BuildIdentityExpressions(IEnumerable<TemplateField> templateFields, IEnumerable<EvaluationSource> sources)
        {
            var sourceName = sources?.FirstOrDefault()?.Name ?? "X";
            var expressions = templateFields.Select((o, index) => new MappingExpression()
            {
                Name = o.Name,
                Cell = o.Name,
                Content = o.Content,
                IsCollection = o.IsCollection,
                Parent = o.Parent,
                Expression = $"=SOURCE(\"{sourceName}\",{o.Name})"
            });
            return new MappingInfo()
            {
                Expressions = expressions,
                Sources = sources
            };
        }

        private static MappingInfo GetMappingInfo(SpreadsheetDocument doc, IEnumerable<EvaluationSource> externalSources)
        {
            var templateFieldExpressions = new List<MappingExpression>();
            var worksheet = GetFirstWorkSheet(doc);
            var stringTablePart = GetOrCreatePart<SharedStringTablePart>(doc);
            var stylesPart = GetOrCreatePart<WorkbookStylesPart>(doc);
            var rowIndex = 3U;
            string name;
            do
            {
                name = GetCellValue(GetCell(worksheet, $"A{rowIndex}"), stringTablePart);
                if (!string.IsNullOrEmpty(name))
                {
                    var formulaCell = GetCell(worksheet, $"F{rowIndex}");
                    var formulaCellFormat = GetCellFormat(formulaCell, stylesPart);
                    var templateFieldExpression = new MappingExpression()
                    {
                        Name = name,
                        Parent = GetCellValue(GetCell(worksheet, $"B{rowIndex}"), stringTablePart),
                        IsCollection = GetCellValueAsBoolean(GetCell(worksheet, $"C{rowIndex}"), stringTablePart),
                        Content = GetCellValue(GetCell(worksheet, $"D{rowIndex}"), stringTablePart),
                        Expression = GetCellFormula(formulaCell),
                        Cell = $"F{rowIndex}",
                        NumFormatId = formulaCellFormat.Item1,
                        NumFormatCode = formulaCellFormat.Item2
                    };
                    templateFieldExpressions.Add(templateFieldExpression);
                    ++rowIndex;
                }
            } while (!string.IsNullOrEmpty(name));

            templateFieldExpressions = ReorderExpressionsWithCalcChain(doc.WorkbookPart, templateFieldExpressions);

            var sources = new List<EvaluationSource>();
            if (externalSources != null)
                sources.AddRange(externalSources);
            rowIndex = 3U;
            do
            {
                name = GetCellValue(GetCell(worksheet, $"M{rowIndex}"), stringTablePart);
                if (!string.IsNullOrEmpty(name))
                {
                    var existing = sources.FirstOrDefault(o => string.Equals(name, o.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (existing != null)
                    {
                        existing.Cell = $"N{rowIndex}";
                    }
                    else 
                    {
                        var payload = GetCellValue(GetCell(worksheet, $"N{rowIndex}"), stringTablePart);
                        sources.Add(new EvaluationSource()
                        {
                            Name = name,
                            Cell = $"N{rowIndex}",
                            Payload = JObject.Parse(payload)
                        });
                    }
                    ++rowIndex;
                }
            } while (!string.IsNullOrEmpty(name));
            return new MappingInfo()
            {
                Expressions = templateFieldExpressions,
                Sources = sources
            };
        }

        
        private static void FillMappingsSheet(SpreadsheetDocument mappingsDoc, IEnumerable<TemplateField> templateFields, FillMappingInfo mappingInfo)
        {
            var worksheet = GetFirstWorkSheet(mappingsDoc);
            var stringTablePart = GetOrCreatePart<SharedStringTablePart>(mappingsDoc);
            var rowIndex = 3U;
            foreach (var field in templateFields)
            {
                UpdateCellText(stringTablePart, worksheet, rowIndex, "A", field.Name);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "B", field.Parent);
                if (field.IsCollection)
                    UpdateCellValue(worksheet, rowIndex, "C", "1", CellValues.Boolean);
                else
                    UpdateCellText(stringTablePart, worksheet, rowIndex, "C", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "D", field.Content);

                UpdateCellText(stringTablePart, worksheet, rowIndex, "F", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "G", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "H", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "I", string.Empty);
                //UpdateCellFormula(worksheet, rowIndex, "I", $"IFNA(FORMULATEXT(F{rowIndex}),\"\")");
                UpdateCellText(stringTablePart, worksheet, rowIndex, "J", string.Empty);
                //UpdateCellFormula(worksheet, rowIndex, "K", $"IF(ISNA(FORMULATEXT(F{rowIndex})),\"\",IF(F{rowIndex}=J{rowIndex},1,IF(F{rowIndex}=IFNA(VALUE(J{rowIndex}),J{rowIndex}),1,2)))");
                ++rowIndex;
            }
            UpdateCellText(stringTablePart, worksheet, 14, "N", GetCustomDocumentProperty(mappingsDoc, "DocumentCreatorVersion") ?? "?");
            UpdateCellText(stringTablePart, worksheet, 15, "N", mappingInfo.TemplateName);
            UpdateCellText(stringTablePart, worksheet, 16, "N", mappingInfo.MappingName);
            UpdateCellText(stringTablePart, worksheet, 17, "N", mappingInfo.TestUrl);

            if (mappingInfo.Payload != null && mappingInfo.Payload.Sources != null)
            {
                rowIndex = 3U;
                foreach (var source in mappingInfo.Payload.Sources)
                {
                    if (source.Name != null && source.Payload != null)
                    {
                        UpdateCellText(stringTablePart, worksheet, rowIndex, "M", source.Name);
                        UpdateCellText(stringTablePart, worksheet, rowIndex, "N", source.Payload.ToString());
                        ++rowIndex;
                        if (rowIndex > 11)
                            break;
                    }
                }
            }
        }

        private static Worksheet GetFirstWorkSheet(SpreadsheetDocument mappingsDoc)
        {
            var wbPart = mappingsDoc.WorkbookPart;
            var sheet = wbPart.Workbook.Descendants<Sheet>().First();
            var worksheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;
            return worksheet;
        }

        private static string GetCustomDocumentProperty(SpreadsheetDocument doc, string propName)
        {
            return doc.CustomFilePropertiesPart?
                .Properties?
                .ChildElements?.OfType<CustomDocumentProperty>()
                .FirstOrDefault(o => o.Name == propName)?
                .InnerText;
        }

        private static T GetOrCreatePart<T>(SpreadsheetDocument doc) where T: OpenXmlPart, IFixedContentTypePart
        {
            var stringTablePart = doc.WorkbookPart.GetPartsOfType<T>().FirstOrDefault();
            if (stringTablePart == null)
                stringTablePart = doc.WorkbookPart.AddNewPart<T>();
            return stringTablePart;
        }


        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(SharedStringTablePart stringTablePart, string text)
        {
            var stringTable = stringTablePart.SharedStringTable;
            // If the part does not contain a SharedStringTable, create one.
            if (stringTable == null)
            {
                stringTable = new SharedStringTable();
                stringTablePart.SharedStringTable = stringTable;
            }
            int index = 0;
            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (var item in stringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                    return index;
                index++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            stringTable.AppendChild(new SharedStringItem(new Text(text)));
            return index;
        }

        private static void UpdateCellValue(Worksheet worksheet, uint rowIndex, string columnName, string value, CellValues? valueType = null, uint? styleIndex = null)
        {
            var cell = GetCell(GetRow(worksheet, rowIndex), $"{columnName}{rowIndex}");
            if (string.IsNullOrEmpty(value))
            {
                cell.CellValue = new CellValue();
            }
            else
            {
                cell.CellValue = new CellValue(value);
                if (valueType.HasValue)
                    cell.DataType = new EnumValue<CellValues>(valueType.Value);
            }
            if (styleIndex.HasValue)
                cell.StyleIndex = styleIndex;
        }

        private static void UpdateCellText(SharedStringTablePart stringTablePart, Worksheet worksheet, uint rowIndex, string column, string text, uint? styleIndex = null)
        {
            text = InsertSharedStringItem(stringTablePart, text).ToString();
            UpdateCellValue(worksheet, rowIndex, column, text, CellValues.SharedString, styleIndex);
        }


        //private static void UpdateCellFormula(Worksheet worksheet, uint rowIndex, string columnName, string text, CellValues? dataType = null)
        //{
        //    var cell = GetCell(GetRow(worksheet, rowIndex), $"{columnName}{rowIndex}");
        //    var cellFormula = new CellFormula(text)
        //    {
        //        CalculateCell = true
        //    };

        //    if (dataType.HasValue)
        //        cell.DataType = dataType.Value;
        //    cell.CellValue = new CellValue();
        //    cell.CellFormula = cellFormula;
        //}

        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var queryRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex >= rowIndex);
            if (queryRow != null && queryRow.RowIndex == rowIndex)
            {
                return queryRow;
            }
            else
            {
                Row row = new Row() { RowIndex = rowIndex };
                if (queryRow == null)
                    sheetData.InsertAt(row, 0);
                else
                    sheetData.InsertBefore(row, queryRow);
                return row;
            }
        }

        private static Cell GetCell(Worksheet worksheet, string address)
        {
            return worksheet
                .Descendants<Cell>()
                .Where(c => string.Compare(c.CellReference, address, true) == 0)
                .FirstOrDefault();

        }
        private static Cell GetCell(Row row, string cellRef)
        {
            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            var queryCell = row
                .Elements<Cell>()
                .FirstOrDefault(c => string.Compare(c.CellReference.Value, cellRef, true) >= 0);
            if (queryCell != null && string.Compare(queryCell.CellReference.Value, cellRef, true) == 0)
            {
                return queryCell;
            }
            else
            {
                Cell targetCell = new Cell() { CellReference = cellRef };
                if (queryCell == null)
                    row.Append(targetCell);
                else
                    row.InsertBefore(targetCell, queryCell);
                return targetCell;
            }
        }

        private static List<MappingExpression> ReorderExpressionsWithCalcChain(WorkbookPart workbookPart,
            List<MappingExpression> expressions)
        {
            var calculationChainPart = workbookPart.CalculationChainPart;
            if (calculationChainPart == null)
                return expressions;
            var calculationChain = calculationChainPart.CalculationChain;
            var cells = expressions.Select(o => o.Cell);
            var orderedExpressions = calculationChain.Elements<CalculationCell>()
                .Where(o => cells.Contains(o.CellReference.ToString()))
                .Select(o => expressions.First(e => e.Cell.Equals(o.CellReference.ToString())))
                .ToList();
            return orderedExpressions;
        }

        private static string GetCellFormula(Cell cell)
        {
            return cell?.CellFormula?.InnerText;
        }

        private static Tuple<int?, string> GetCellFormat(Cell cell, WorkbookStylesPart stylesPart)
        {
            int? numberFormatId = null;
            string formatCode = null;
            if (cell != null)
            {
                var styleIndex = 0U;
                if (cell.StyleIndex != null && cell.StyleIndex.HasValue)
                    styleIndex = cell.StyleIndex.Value;

                var cellFormat = stylesPart.Stylesheet.CellFormats
                    .Elements<CellFormat>()
                    .ElementAtOrDefault((int)styleIndex);
                if (cellFormat != null && cellFormat.NumberFormatId.HasValue)
                {
                    numberFormatId = (int)cellFormat.NumberFormatId.Value;
                    formatCode = stylesPart.Stylesheet.NumberingFormats?
                        .Elements<NumberingFormat>()
                        .Where(o => o.NumberFormatId == numberFormatId)
                        .Select(o => o.FormatCode)
                        .FirstOrDefault();
                }
            }
            return System.Tuple.Create(numberFormatId, formatCode);
        }

        private static string GetCellValue(Cell cell, SharedStringTablePart stringTablePart)
        {
            string cellValue = null;
            if (cell != null)
            {
                cellValue = cell.CellValue?.InnerText ?? cell.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                    cellValue = stringTablePart.SharedStringTable.ElementAt(int.Parse(cellValue)).InnerText;
            }
            return cellValue;
        }
        private static bool GetCellValueAsBoolean(Cell cell, SharedStringTablePart stringTablePart)
        {
            var content = GetCellValue(cell, stringTablePart);
            if (string.IsNullOrEmpty(content))
                return false;
            return content != "0";
        }

    }
}
