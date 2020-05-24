/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 * See LICENSE in the project root for license information.
 */

/* global window, document, Office, Word, console, OfficeExtension, XMLHttpRequest */

Office.onReady(info => {
  if (info.host === Office.HostType.Word) {
    document.getElementById('sideload-msg').style.display = 'none';
    document.getElementById('app-body').style.display = 'flex';
    document.getElementById('display-template-fields').onclick = () => wordRun(displayTemplateFields);
    document.getElementById('insert-rich-text-field').onclick = () => wordRun(insertTemplateField);
    document.getElementById('send-template').onclick = () => sendTemplate();
  }
});

function onError(error: any) {
  console.error('Error: ' + error);
  if (error instanceof OfficeExtension.Error) {
    console.error('Debug info: ' + JSON.stringify(error.debugInfo));
  }
}
function wordRun(func: { (context: Word.RequestContext): Promise<any> }) {
  Word.run(context => {
    return func(context);
  }).catch(onError);
}

function sendTemplate() {
  // https://docs.microsoft.com/en-us/office/dev/add-ins/word/get-the-whole-document-from-an-add-in-for-word
  // Get all of the content from a PowerPoint or Word document in 100-KB chunks of text.
  console.log('Sending template');
  Office.context.document.getFileAsync(Office.FileType.Compressed, { sliceSize: 100000 }, result => {
    if (result.status == Office.AsyncResultStatus.Succeeded) {
      // Get the File object from the result.
      var myFile = result.value;
      var state = {
        file: myFile,
        counter: 0,
        sliceCount: myFile.sliceCount,
        sendUrl: 'http://localhost:6001/api/xxxxx'
      };

      console.log('Getting file of ' + myFile.size + ' bytes');
      getSlice(state);
    } else {
      console.error(result.status);
    }
  });
}
// Get a slice from the file and then call sendSlice.
function getSlice(state) {
  state.file.getSliceAsync(state.counter, result => {
    if (result.status == Office.AsyncResultStatus.Succeeded) {
      console.log('Sending piece ' + (state.counter + 1) + ' of ' + state.sliceCount);
      sendSlice(result.value, state);
    } else {
      console.error(result.status);
    }
  });
}
function sendSlice(slice, state) {
  var data = slice.data;

  // If the slice contains data, create an HTTP request.
  if (data) {
    // Encode the slice data, a byte array, as a Base64 string.
    // NOTE: The implementation of myEncodeBase64(input) function isn't
    // included with this example. For information about Base64 encoding with
    // JavaScript, see https://developer.mozilla.org/docs/Web/JavaScript/Base64_encoding_and_decoding.
    var fileData = window.btoa(data);

    // Create a new HTTP request. You need to send the request
    // to a webpage that can receive a post.
    var request = new XMLHttpRequest();

    // Create a handler function to update the status
    // when the request has been sent.
    request.onreadystatechange = function() {
      if (request.readyState == 4) {
        console.log('Sent ' + slice.size + ' bytes.');
        state.counter++;

        if (state.counter < state.sliceCount) {
          getSlice(state);
        } else {
          closeFile(state);
        }
      }
    };

    request.open('POST', state.sendUrl);
    request.setRequestHeader('Slice-Number', slice.index);

    // Send the file as the body of an HTTP POST
    // request to the web server.
    request.send(fileData);
  }
}
function closeFile(state) {
  // Close the file when you're done with it.
  state.file.closeAsync(function(result) {
    // If the result returns as a success, the
    // file has been successfully closed.
    if (result.status == 'succeeded') {
      console.log('File closed.');
    } else {
      console.error('File could not be closed.');
    }
  });
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
  contentControl.appearance = 'Tags';
  contentControl.color = 'blue';
  return context.sync();
}

async function displayTemplateFields(context: Word.RequestContext): Promise<void> {
  const contentControls = await getTemplateFields(context);
  if (contentControls.items.length === 0) {
    console.log('No content control found.');
  } else {
    contentControls.items.forEach(cc => {
      console.log(cc.id, cc.type, cc.tag, cc.title);
    });
  }
  return context.sync();
}

function getTemplateFieldName(controls: Word.ContentControl[], text: string) {
  if (isValidName(text) && !contentControlExists(controls, text)) {
    return text;
  } else {
    return createContentControlName(controls);
  }
}
function createContentControlName(controls: Word.ContentControl[]): string {
  let num = 1;
  // eslint-disable-next-line no-constant-condition
  while (true) {
    const name = 'TF' + ('0000' + num).slice(-4);
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
