/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 * See LICENSE in the project root for license information.
 */

/* global document, Office, Word, console, OfficeExtension */

Office.onReady(info => {
  if (info.host === Office.HostType.Word) {
    document.getElementById("sideload-msg").style.display = "none";
    document.getElementById("app-body").style.display = "flex";
    document.getElementById("display-template-fields").onclick = () => wordRun(displayTemplateFields);
    document.getElementById("insert-rich-text-field").onclick = () => wordRun(insertTemplateField);
  }
});

function onError(error: any) {
  console.error("Error: " + error);
  if (error instanceof OfficeExtension.Error) {
      console.error("Debug info: " + JSON.stringify(error.debugInfo));
  }
}
function wordRun(func: {(context: Word.RequestContext): Promise<any>}) {
  Word.run(context => {
      return func(context);
  })
  .catch(onError);
}


function getTemplateFields(context: Word.RequestContext): Promise<Word.ContentControlCollection> {
  // https://docs.microsoft.com/en-us/javascript/api/word/word.contentcontrolcollection?view=word-js-preview#load-option-
  // Create a proxy object for the content controls collection.
  const contentControls = context.document.contentControls;
  // Queue a command to load the selected properties for all content controls.
  context.load(contentControls, ['id', 'type', 'tag', 'title']);
  // Synchronize the document state by executing the queued commands
  return context.sync(contentControls);
}
function getSelectionText(context: Word.RequestContext): Promise<Word.Range> {
  const selection = context.document.getSelection();
  selection.load('text');
  return context.sync(selection);
}

async function insertTemplateField(context: Word.RequestContext): Promise<void> {
  const contentControls = await getTemplateFields(context);
  const selection = await getSelectionText(context);
  const name = getTemplateFieldName(contentControls.items, selection.text);
  const contentControl = selection.insertContentControl();
  contentControl.title = name;
  contentControl.tag = name;
  contentControl.appearance = "Tags";
  contentControl.color = "blue";
  return context.sync();
}

async function displayTemplateFields(context: Word.RequestContext): Promise<void> {
  const contentControls = await getTemplateFields(context);
  if (contentControls.items.length === 0) {
    console.log('No content control found.');
  }
  else {
    contentControls.items.forEach(cc => {
      console.log(cc.id, cc.type, cc.tag, cc.title);
    });
  }
  return context.sync();
}


function getTemplateFieldName(controls: Word.ContentControl[], text: string) {
  if (isValidName(text) && !contentControlExists(controls, text)) {
    return text;
  }
  else {
    return createContentControlName(controls);
  }
}
function createContentControlName(controls: Word.ContentControl[]): string {
  let num = 1;
  // eslint-disable-next-line no-constant-condition
  while (true) {
    const name = 'TF' + ('0000' + num).slice(-4);
    if (!contentControlExists(controls, name))
      return name;
    ++num;
  }
}
function contentControlExists(controls: Word.ContentControl[], value: string): boolean {
  for (let i = 0; i < controls.length; i++)
  {
    const cc = controls[i];
    if (cc.title == value || cc.tag == value) {
      return true;
    }
  }
  return false;
}
function isValidName(text: string): boolean {
  const regex = /^[a-zA-Z_$][0-9a-zA-Z_$]*$/g;
  return regex.exec(text) != null;
}




