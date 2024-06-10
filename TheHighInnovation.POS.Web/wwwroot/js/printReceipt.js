window.printReceipt = function (receiptContent) {
    // Create a hidden iframe
    var iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    document.body.appendChild(iframe);

    var doc = iframe.contentWindow.document;
    doc.open();
    doc.write(receiptContent);
    doc.close();

    iframe.contentWindow.print();

    // Remove the iframe after printing
    setTimeout(function () {
        iframe.parentNode.removeChild(iframe);
    }, 1000); // Adjust the timeout as needed
}
