/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 * See LICENSE in the project root for license information.
 */

import { postTemplate } from "./components/templates";
import { insertTemplateField, getTemplateFields, insertTemplateRowField } from "./components/template-fields";

/* global document, console, Office, Word, OfficeExtension */

Office.onReady(info => {
  if (info.host === Office.HostType.Word) {
    document.getElementById("sideload-msg").style.display = "none";
    document.getElementById("app-body").style.display = "flex";
    document.getElementById("display-template-fields").onclick = () => wordRun(displayTemplateFields);
    document.getElementById("insert-template-field").onclick = () => wordRun(insertTemplateField);
    document.getElementById("insert-table-row-field").onclick = () => wordRun(insertTemplateRowField);
    document.getElementById("send-template").onclick = () => postTemplate();
  }
});

function wordRun(func: { (context: Word.RequestContext): Promise<any> }) {
  Word.run(context => {
    return func(context);
  }).catch(error => {
    console.error("Error: " + error);
    if (error instanceof OfficeExtension.Error) {
      console.error("Debug info: " + JSON.stringify(error.debugInfo));
    }
  });
}

async function displayTemplateFields(context: Word.RequestContext): Promise<void> {
  const contentControls = await getTemplateFields(context);
  if (contentControls.items.length === 0) {
    console.log("No content control found.");
  } else {
    console.log("Template fields", contentControls.items.length);
    contentControls.items.forEach(cc => {
      console.log(cc.id, cc.type, cc.subtype, cc.tag, cc.title, 
        "parent=", cc.parentContentControlOrNullObject.id);
      //console.log(cc);
    });
  }
  return context.sync();
}
