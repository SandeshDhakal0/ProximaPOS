//window.downloadFileFromStream = function (fileName, fileType, streamRef) {
//    streamRef.arrayBuffer().then(function (buffer) {
//        const blob = new Blob([buffer], { type: fileType });
//        const url = URL.createObjectURL(blob);
//        const link = document.createElement('a');
//        link.href = url;
//        link.download = fileName;
//        document.body.appendChild(link);
//        link.click();
//        document.body.removeChild(link);
//        URL.revokeObjectURL(url);
//    });
//};


window.saveAsFile = function (filename, byteBase64) {
    var link = this.document.createElement('a');
    link.download = filename;  // Use the argument 'filename' here
    console.log("This is ready now.");
    console.log(filename);  // Log the argument 'filename'
    link.href = "data:application/octet-stream;base64," + byteBase64;
    this.document.body.appendChild(link);
    link.click();
    this.document.body.removeChild(link);
};
