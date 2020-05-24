/* global window, console, XMLHttpRequest, Office */

/**
 * Uploads the current document to DocumentCreator API
 */
export function postTemplate() {
  // https://docs.microsoft.com/en-us/office/dev/add-ins/word/get-the-whole-document-from-an-add-in-for-word
  // Get all of the content from a PowerPoint or Word document in 100-KB chunks of text.
  console.log("Sending template");
  Office.context.document.getFileAsync(Office.FileType.Compressed, { sliceSize: 100000 }, result => {
    if (result.status == Office.AsyncResultStatus.Succeeded) {
      // Get the File object from the result.
      var myFile = result.value;
      var state = {
        file: myFile,
        counter: 0,
        sliceCount: myFile.sliceCount,
        sendUrl: "http://localhost:6001/api/xxxxx"
      };

      console.log("Getting file of " + myFile.size + " bytes");
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
      console.log("Sending piece " + (state.counter + 1) + " of " + state.sliceCount);
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
    request.onreadystatechange = () => {
      if (request.readyState == 4) {
        console.log("Sent " + slice.size + " bytes.");
        state.counter++;

        if (state.counter < state.sliceCount) {
          getSlice(state);
        } else {
          closeFile(state);
        }
      }
    };

    request.open("POST", state.sendUrl);
    request.setRequestHeader("Slice-Number", slice.index);

    // Send the file as the body of an HTTP POST request to the web server.
    request.send(fileData);
  }
}
function closeFile(state) {
  // Close the file when you're done with it.
  state.file.closeAsync(result => {
    // If the result returns as a success, the
    // file has been successfully closed.
    if (result.status == "succeeded") {
      console.log("File closed.");
    } else {
      console.error("File could not be closed.");
    }
  });
}
