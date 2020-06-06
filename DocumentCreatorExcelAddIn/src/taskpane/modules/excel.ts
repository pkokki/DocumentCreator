import { Dispatch } from "react";
import { DocumentCreatorActionTypes, Template } from "../store/dc/types";
import { activateWorksheet, raiseError } from "../store/dc/actions";

var _workbookOnActivated: OfficeExtension.EventHandlerResult<Excel.WorksheetActivatedEventArgs>;
var _dispatch: Dispatch<DocumentCreatorActionTypes>;

function errorHandlerFunction(error: any): void {
  console.error("Excel.run", error);
  _dispatch(raiseError(error));
}

async function handleWorkbookActivation(event: Excel.WorksheetActivatedEventArgs): Promise<any> {
  try {
    return Excel.run(async function(context) {
      _dispatch(activateWorksheet(event.worksheetId));
      await context.sync();
    });
  } catch (error) {
    errorHandlerFunction(error);
  }
}

async function unregisterEventHandler<T>(eventHandler: OfficeExtension.EventHandlerResult<T>): Promise<void> {
  if (eventHandler) {
    try {
      return Excel.run(eventHandler.context, async function(context) {
        eventHandler.remove();
        await context.sync();
        eventHandler = null;
        console.log("Event handler successfully removed.");
      });
    } catch (error) {
      errorHandlerFunction(error);
    }
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

async function fillActiveSheet(template: Template): Promise<boolean> {
  try {
    return Excel.run(async context => {
      const sheet = context.workbook.worksheets.getActiveWorksheet();
      const range1 = sheet.getRange("A1:N3");
      range1.values = [
        ["TEMPLATE:", template.templateName, null, null, null, null, null, null, null, null, null, null, null, null],
        ["FIELDS", null, null, null, null, "MAPPING", null, null, null, null, null, null, "SOURCES", null],
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
          "Check",
          null,
          "Name",
          "Value"
        ]
      ];
      range1.format.autofitColumns();
      const cellData = [];
      template.fields.map(field => cellData.push([field.name, field.parent, field.isCollection, field.content]));
      const range2 = sheet.getRange("A4:D" + (cellData.length + 3));
      range2.values = cellData;
      range2.format.autofitColumns();
      await context.sync();
      return true;
    });
  } catch (error) {
    errorHandlerFunction(error);
    return false;
  }
}

async function activateSheet(name: string, createIfNotExists: boolean): Promise<boolean> {
  console.log("activateSheet", name, createIfNotExists);
  try {
    return Excel.run(async context => {
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
    });
  } catch (error) {
    errorHandlerFunction(error);
    return false;
  }
}

async function testMappings(templateName: string, mappingName: string): Promise<void> {
  console.log("testMappings", templateName, mappingName);
  try {
    return Excel.run(async (context) => {
      return context.sync();
    });
  }
  catch (error) {
    errorHandlerFunction(error);
  }
}

export const ExcelHelper = {
  initializeExcel: initializeExcel,
  fillActiveSheet: fillActiveSheet,
  testMappings: testMappings,
  activateSheet: activateSheet
}
