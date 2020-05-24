/* global console */

/**
 * Inserts a new content control at the current selection point.
 * If selection has a valid and not-existing name, then this name is used.
 * Otherwise, the next available name is used (TFnnnn).
 */
export async function insertTemplateField(context: Word.RequestContext): Promise<void> {
  const contentControls = await getTemplateFields(context);
  const selection = context.document.getSelection();
  selection.load("text");
  await context.sync();

  const name = getTemplateFieldName(contentControls.items, selection.text);
  const contentControl = selection.insertContentControl();
  contentControl.title = name;
  contentControl.tag = name;
  contentControl.appearance = "Tags";
  contentControl.color = "blue";
  return context.sync();
}

export async function insertTemplateRowField(context: Word.RequestContext): Promise<void> {
  
  const selection = context.document.getSelection();
  selection.load([
    "parentTableCellOrNullObject/isNullObject",
    "parentTableOrNullObject/isNullObject",
  ]);
  await context.sync();

  if (selection.parentTableCellOrNullObject.isNullObject || selection.parentTableOrNullObject.isNullObject) {
    console.log("parent cell or table is null")
  }
  else {
    const contentControls = await getTemplateFields(context);
    const name = getTemplateFieldName(contentControls.items, null);

    const table = selection.parentTableOrNullObject;
    console.log("found table row", table, name);
    const contentControl = table.insertContentControl();
    contentControl.title = name;
    contentControl.tag = name;
    contentControl.appearance = "Tags";
    contentControl.color = "yellow";
    console.log("contentControl", contentControl);
  }
  return context.sync();
}

/**
 * Return the collection of the content controls of the document.
 */
export function getTemplateFields(context: Word.RequestContext): Promise<Word.ContentControlCollection> {
  // https://docs.microsoft.com/en-us/javascript/api/word/word.contentcontrolcollection?view=word-js-preview#load-option-
  // Create a proxy object for the content controls collection.
  const contentControls = context.document.contentControls;
  // Queue a command to load the selected properties for all content controls.
  context.load(contentControls, [
    "id",
    "type",
    "subtype",
    "tag",
    "title",
    "parentContentControlOrNullObject/id"
  ]);
  // Synchronize the document state by executing the queued commands
  return context.sync(contentControls);
}

function getTemplateFieldName(controls: Word.ContentControl[], text: string) {
  if (text && isValidName(text) && !contentControlExists(controls, text)) {
    return text;
  } else {
    return getNextAvailableName(controls);
  }
}

function getNextAvailableName(controls: Word.ContentControl[]): string {
  let num = 1;
  // eslint-disable-next-line no-constant-condition
  while (true) {
    const name = "TF" + ("0000" + num).slice(-4);
    if (!contentControlExists(controls, name)) return name;
    ++num;
  }
}

function contentControlExists(controls: Word.ContentControl[], value: string): boolean {
  for (let i = 0; i < controls.length; i++) {
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
