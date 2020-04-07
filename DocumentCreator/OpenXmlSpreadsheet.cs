using DocumentCreator.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator
{
    public static class OpenXmlSpreadsheet
    {
        public static Worksheet GetFirstWorkSheet(SpreadsheetDocument mappingsDoc)
        {
            var wbPart = mappingsDoc.WorkbookPart;
            var sheet = wbPart.Workbook.Descendants<Sheet>().First();
            var worksheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;
            return worksheet;
        }

        public static void FillMappingsSheet(SpreadsheetDocument mappingsDoc, string mappingName, IEnumerable<TemplateField> templateFields, string testUrl)
        {
            var worksheet = GetFirstWorkSheet(mappingsDoc);
            var stringTablePart = GetSharedStringTablePart(mappingsDoc);
            var rowIndex = 3U;
            var fieldId = 1;
            foreach (var field in templateFields)
            {
                UpdateCellValue(worksheet, rowIndex, "A", fieldId.ToString(), CellValues.Number);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "B", field.Name);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "C", field.Parent);
                if (field.IsCollection)
                    UpdateCellValue(worksheet, rowIndex, "D", "1", CellValues.Boolean);
                else
                    UpdateCellText(stringTablePart, worksheet, rowIndex, "D", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "F", field.Content);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "G", string.Empty);

                UpdateCellText(stringTablePart, worksheet, rowIndex, "H", mappingName);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "I", field.Name);
                UpdateCellFormula(worksheet, rowIndex, "J", "NA()");
                UpdateCellText(stringTablePart, worksheet, rowIndex, "K", string.Empty);
                UpdateCellText(stringTablePart, worksheet, rowIndex, "L", string.Empty);
                UpdateCellFormula(worksheet, rowIndex, "M", $"IF(J{rowIndex}=K{rowIndex},1,0)");
                ++rowIndex;
                ++fieldId;
            }
            UpdateCellText(stringTablePart, worksheet, 17, "P", testUrl);
        }

        private static SharedStringTablePart GetSharedStringTablePart(SpreadsheetDocument doc)
        {
            // Get the SharedStringTablePart. If it does not exist, create a new one.
            var stringTablePart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (stringTablePart == null)
                stringTablePart = doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
            return stringTablePart;
        }

        public static void UpdateCellText(SharedStringTablePart stringTablePart, Worksheet worksheet, uint rowIndex, string column, string text)
        {
            text ??= string.Empty;
            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(stringTablePart, text);

            var cell = GetCell(GetRow(worksheet, rowIndex), $"{column}{rowIndex}");
            // Set the value of cell A1.
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
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

        public static void UpdateCellValue(Worksheet worksheet, uint rowIndex, string columnName, string value, CellValues valueType)
        {
            var cell = GetCell(GetRow(worksheet, rowIndex), $"{columnName}{rowIndex}");
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(valueType);
        }
        public static void UpdateCellFormula(Worksheet worksheet, uint rowIndex, string columnName, string text)
        {
            var cell = GetCell(GetRow(worksheet, rowIndex), $"{columnName}{rowIndex}");
            cell.CellValue = new CellValue(string.Empty);
            cell.CellFormula = new CellFormula(text);
        }

        public static Row GetRow(Worksheet worksheet, uint rowIndex)
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
        public static Cell GetCell(Row row, string cellRef)
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
    }
}
