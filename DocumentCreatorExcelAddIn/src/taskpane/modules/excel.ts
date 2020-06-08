import { Dispatch } from "react";
import {
  DocumentCreatorActionTypes,
  Template,
  EvaluationRequest,
  EvaluationExpression,
  EvaluationSource,
  EvaluationOutput
} from "../store/dc/types";
import { activateWorksheet, raiseError } from "../store/dc/actions";

var _workbookOnActivated: OfficeExtension.EventHandlerResult<Excel.WorksheetActivatedEventArgs>;
var _dispatch: Dispatch<DocumentCreatorActionTypes>;

function errorHandlerFunction(error: any): void {
  console.error("Excel.run", error);
  _dispatch(raiseError(typeof error === "string" ? error : error.message || "An Excel.run error occurred."));
}

async function handleWorkbookActivation(event: Excel.WorksheetActivatedEventArgs): Promise<any> {
  return Excel.run(async function(context) {
    try {
      _dispatch(activateWorksheet(event.worksheetId));
      await context.sync();
    } catch (error) {
      errorHandlerFunction(error);
    }
  });
}

async function unregisterEventHandler<T>(eventHandler: OfficeExtension.EventHandlerResult<T>): Promise<void> {
  if (eventHandler) {
    return Excel.run(eventHandler.context, async function(context) {
      try {
        eventHandler.remove();
        await context.sync();
        eventHandler = null;
      } catch (error) {
        errorHandlerFunction(error);
      }
    });
  }
  return Promise.resolve();
}

function registerWorksheetCollectionEventHandlers() {
  Excel.run(async function(context) {
    const workbook = context.workbook.worksheets;
    // onActivated: Occurs when an object is activated.
    await unregisterEventHandler(_workbookOnActivated);
    _workbookOnActivated = workbook.onActivated.add(handleWorkbookActivation);
    // onDeactivated: Occurs when an object is deactivated.

    // onAdded: Occurs when an object is added to the collection.
    // onDeleted: Occurs when an object is deleted from the collection.

    // onSelectionChanged: Occurs when the active cell or selected range is changed. Warning
    // onChanged: // Occurs when data within cells is changed.

    // onRowSorted
    // onRowHiddenChanged
    // onColumnSorted
    // onFormatChanged
    // onCalculated: Occurs when all the worksheets of the collection have finished calculation.
    await context.sync();
  }).catch(errorHandlerFunction);
}

function registerWorksheetEventHandlers() {
  // onActivated: Occurs when an object is activated.
  // onDeactivated: Occurs when an object is deactivated.
  // onSelectionChanged: Occurs when the active cell or selected range is changed. Warning
  // onChanged: Occurs when data within cells is changed.
  // onSingleClicked: Occurs when left-clicked/tapped action occurs in the worksheet.
  // onRowSorted
  // onRowHiddenChanged
  // onColumnSorted
  // onFormatChanged
  // onCalculated: Occurs when a worksheet has finished calculation.
}

function registerWorkbookEventHandlers() {
  // onAutoSaveSettingChanged
}

function getActiveWorksheet() {
  Excel.run(async function(context) {
    const worksheet = context.workbook.worksheets.getActiveWorksheet();
    worksheet.load(["id", "name"]);
    await context.sync();
    _dispatch(activateWorksheet(worksheet.id));
  }).catch(errorHandlerFunction);
}

function initializeExcel(dispatch: Dispatch<DocumentCreatorActionTypes>) {
  _dispatch = dispatch;
  registerWorkbookEventHandlers();
  registerWorksheetCollectionEventHandlers();
  registerWorksheetEventHandlers();
  getActiveWorksheet();
}

function formatHeaders(sheet: Excel.Worksheet) {
  sheet.getRange("A1:N1").set({
    format: {
      fill: {
        color: "#4472C4"
      },
      font: {
        color: "white",
        bold: true
      }
    }
  });
  sheet.getRanges(`B1:C1, G1:H1`).format.font.color = "orange";
  sheet.getRange("A2:N2").set({
    format: {
      fill: {
        color: "#4472C4"
      },
      font: {
        color: "white"
      }
    }
  });
}

function formatRanges(sheet: Excel.Worksheet, lastRow: number) {
  const range3 = sheet.getRanges(`A3:D${lastRow}, F3:K${lastRow}, M3:N${lastRow}`);
  range3.format.borders.getItem("EdgeTop").color = "#BFBFBF";
  range3.format.borders.getItem("EdgeBottom").color = "#BFBFBF";
  range3.format.borders.getItem("EdgeLeft").color = "#BFBFBF";
  range3.format.borders.getItem("EdgeRight").color = "#BFBFBF";
  range3.format.borders.getItem("InsideVertical").color = "#BFBFBF";
  range3.format.borders.getItem("InsideHorizontal").color = "#BFBFBF";

  // Read-only columns
  sheet.getRanges(`A3:D${lastRow}, I3:K${lastRow}`).set({
    format: {
      fill: { color: "#F2F2F2" }
    }
  });
  // Read-write columns
  sheet.getRanges(`F3:H${lastRow}, M3:N${lastRow}`).set({
    format: {
      fill: { color: "white" }
    }
  });
  // Expression columns
  sheet.getRanges(`F3:F${lastRow}, J3:J${lastRow}`).set({
    format: {
      columnWidth: 200,
      font: { bold: true }
    }
  });
  // Formula column
  sheet.getRange(`I3:I${lastRow}`).set({
    format: {
      font: { name: "Consolas", size: 9 }
    }
  });
  // Check column
  const checkColumn = sheet.getRange(`K3:K${lastRow}`);
  checkColumn.set({
    format: {
      font: { color: "#F2F2F2" },
      columnWidth: 25
    }
  });
  const conditionalFormat = checkColumn.conditionalFormats.add(Excel.ConditionalFormatType.iconSet);
  const iconSet = conditionalFormat.iconSet;
  iconSet.style = Excel.IconSet.threeSymbols2;
  iconSet.criteria = [
    {} as any,
    {
      type: Excel.ConditionalFormatIconRuleType.number,
      operator: Excel.ConditionalIconCriterionOperator.greaterThanOrEqual,
      formula: "=0"
    },
    {
      type: Excel.ConditionalFormatIconRuleType.number,
      operator: Excel.ConditionalIconCriterionOperator.greaterThanOrEqual,
      formula: "=1"
    }
  ];
  // Source payload column
  sheet.getRange(`N3:N${lastRow}`).set({
    format: {
      columnWidth: 125
    }
  });
  // Separator columns
  sheet.getRanges(`E1:E${lastRow}, L1:L${lastRow}`).set({
    format: {
      columnWidth: 6.75,
      fill: { color: "white" }
    }
  });
}

function resolveNumFormat(numFormat: string): { id: number, code: string } {
  var id = null;
  var code = null;
  if (numFormat && numFormat.length && numFormat !== "General") {
    code = numFormat
  }
  return {
    id: id,
    code: code
  };
}

async function formatActiveSheet(): Promise<void> {
  return Excel.run(async context => {
    try {
      const sheet = context.workbook.worksheets.getActiveWorksheet();
      const range = sheet.getUsedRange();
      range.load("rowCount");
      await context.sync();
      const lastRow = range.rowCount;

      sheet.getUsedRange().clear("Formats");
      await context.sync();

      formatHeaders(sheet);
      formatRanges(sheet, lastRow);
      await context.sync();
    } catch (error) {
      errorHandlerFunction(error);
    }
  });
}
async function fillActiveSheet(template: Template): Promise<boolean> {
  return Excel.run(async context => {
    try {
      const sheet = context.workbook.worksheets.getActiveWorksheet();

      sheet.getUsedRange().clear("All");
      await context.sync();

      const templateName = template ? template.templateName : "#NEW";
      sheet.getRange("A1:N2").values = [
        ["FIELDS", templateName, null, null, null, "MAPPING", null, null, null, null, null, null, "SOURCES", null],
        [
          "Name",
          "ParentId",
          "IsColl",
          "Content",
          null,
          "Value",
          "Expected",
          "Comment",
          "Expression",
          "API result",
          "Ch",
          null,
          "Name",
          "Value"
        ]
      ];
      formatHeaders(sheet);

      if (template && template.fields && template.fields.length) {
        const cellData = [];
        template.fields.map(field =>
          cellData.push([field.name, field.parent, field.isCollection ? true : null, field.content])
        );

        const lastRow = template.fields.length + 2;
        formatRanges(sheet, lastRow);
      }
      await context.sync();
      return true;
    } catch (error) {
      errorHandlerFunction(error);
      return false;
    }
  });
}

async function activateSheet(name: string, createIfNotExists: boolean): Promise<boolean> {
  return Excel.run(async context => {
    try {
      var sheets = context.workbook.worksheets;
      sheets.load("items/name");
      await context.sync();

      let sheet = sheets.items.find(v => v.name === name);
      if (sheet === undefined && createIfNotExists) {
        sheet = sheets.add(name);
        await context.sync();
      }

      if (sheet !== undefined) {
        sheet.activate();
        await context.sync();
        return true;
      }
      return false;
    } catch (error) {
      errorHandlerFunction(error);
      return false;
    }
  });
}

async function getEvaluationPayload(templateName: string): Promise<EvaluationRequest> {
  return Excel.run(async context => {
    try {
      // Get range
      const sheet = context.workbook.worksheets.getActiveWorksheet();
      const usedRange = sheet.getUsedRange(true);
      usedRange.load(["formulas", "rowCount", "columnCount", "numberFormat"]);
      await context.sync();


      if (usedRange.rowCount >= 3 && usedRange.columnCount >= 6) {
        const payload: EvaluationRequest = {
          templateName: templateName,
          expressions: [],
          sources: []
        };

        usedRange.formulas.forEach((row, rowIndex) => {
          if (rowIndex >= 2) {
            if (row[0] && row[0] !== "" && row[5] && row[5] !== "") {
              const numberFormat = resolveNumFormat(usedRange.numberFormat[rowIndex][5]);
              const expression: EvaluationExpression = {
                name: row[0],
                cell: "F" + (rowIndex + 1),
                expression: row[5],
                parent: row[1] !== "" ? row[1] : null,
                isCollection: !!row[2],
                content: row[3] !== "" ? row[3] : null,
                numFormatId: numberFormat.id,
                numFormatCode: numberFormat.code
              };
              payload.expressions.push(expression);
            }

            if (row.length >= 14 && row[12] && row[12] !== "") {
              const source: EvaluationSource = {
                name: row[12],
                cell: "N" + (rowIndex + 1),
                payload: JSON.parse(row[13])
              };
              payload.sources.push(source);
            }
          }
        });

        return payload;
      }
      return undefined;
    } catch (error) {
      errorHandlerFunction(error);
      return undefined;
    }
  });
}

async function setEvaluationResult(evalRequest: EvaluationRequest, evalResponse: EvaluationOutput): Promise<void> {
  return Excel.run(async context => {
    try {
      const sheet = context.workbook.worksheets.getActiveWorksheet();

      sheet.getRange("I3:K100").clear("Contents");
      evalResponse.results.forEach(result => {
        const expr = evalRequest.expressions.find(e => e.name === result.name);
        if (expr) {
          const rowIndex = expr.cell.substr(1);
          // I: =IFNA(FORMULATEXT(F3);"")
          // K: =IF(ISNA(FORMULATEXT(F3));"";IF(F3=J3;1;IF(F3=IFNA(VALUE(J3);J3);1;2)))
          sheet.getRange(`I${rowIndex}:K${rowIndex}`).formulas = [
            [
              `=IFNA(FORMULATEXT(F${rowIndex}),"")`,
              result.error || result.text,
              `=IF(ISNA(FORMULATEXT(F${rowIndex})),"",IF(F${rowIndex}=J${rowIndex},1,IF(F${rowIndex}=IFNA(VALUE(J${rowIndex}),J${rowIndex}),1,-1)))`
            ]
          ];
        }
      });
      await context.sync();
    } catch (error) {
      errorHandlerFunction(error);
      return undefined;
    }
  });
}

export const ExcelHelper = {
  initializeExcel: initializeExcel,
  formatActiveSheet: formatActiveSheet,
  fillActiveSheet: fillActiveSheet,
  getEvaluationPayload: getEvaluationPayload,
  setEvaluationResult: setEvaluationResult,
  activateSheet: activateSheet
};
