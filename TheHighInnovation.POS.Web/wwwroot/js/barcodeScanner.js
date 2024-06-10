window.initializeBarcodeScanner = function (dotNetRef) {
    var barcode = '';
    var interval;
    console.log(barcode);
    document.addEventListener('keydown', function (evt) {
        if (interval) {
            clearInterval(interval);
        }
        if (evt.code == 'Enter') {
            if (barcode) {
                handleBarcode(barcode);
            }
            barcode = '';
            return;
        }
        if (evt.code !== 'Shift') {
            barcode += evt.key;
            interval = setInterval(() => barcode = '', 20);
        }
    });
    console.log(evt);
    function handleBarcode(scannedBarcode) {
        console.log(scannedBarcode);
        dotNetRef.invokeMethodAsync('ReceiveBarcode', scannedBarcode);
    }
}
