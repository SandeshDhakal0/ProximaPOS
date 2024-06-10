window.leapfrogdatepicker = () => {
    console.log("leapfrogdatepicker initialized");
    $(document).ready(function () {
        console.log("Document ready");

        // Initialize the bod-picker elements with Nepali date picker
        $('.bod-picker').nepaliDatePicker({
            dateFormat: '%D, %M %d, %y',
            closeOnDateSelect: true
        });

        // Initialize the issue-date-picker-bs element with Nepali date picker
        $('#issue-date-picker-bs').nepaliDatePicker({
            dateFormat: '%y-%m-%d',
            closeOnDateSelect: true
        });

        // Attach dateChange event listener to issue-date-picker-bs element
        $('#issue-date-picker-bs').on('dateChange', function (event) {
            console.log("Issue Date event triggered", event);
            eventLog(event, 'issueDate');
        });

        // Initialize the due-date-picker-bs element with Nepali date picker
        $('#due-date-picker-bs').nepaliDatePicker({
            dateFormat: '%y-%m-%d',
            closeOnDateSelect: true
        });

        // Attach dateChange event listener to due-date-picker-bs element
        $('#due-date-picker-bs').on('dateChange', function (event) {
            console.log("Due Date Event triggered", event);
            eventLog(event, 'dueDate');
        });

        // Clear button functionality for issue date picker
        $('#clear-issue-date').on('click', function () {
            $('#issue-date-picker-bs').val('');
        });

        // Clear button functionality for due date picker
        $('#clear-due-date').on('click', function () {
            $('#due-date-picker-bs').val('');
        });
    });
};

window.eventLog = (event, dateType) => {
    console.log("eventLog called", event);
    var datePickerData = event.datePickerData || {};
    var outputData = {
        dateType: dateType,
        datePickerData: datePickerData
    };
    console.log(outputData);

    if (DotNet && DotNet.invokeMethodAsync) {
        DotNet.invokeMethodAsync('TheHighInnovation.POS', 'ReceiveDatePickerData', outputData)
            .then(result => {
                console.log("ReceiveDatePickerData invoked successfully", result);
            })
            .catch(error => {
                console.error("Error invoking ReceiveDatePickerData", error);
            });
    } else {
        console.error("DotNet.invokeMethodAsync is not available");
    }
};

window.getCurrentNepaliMonth = function () {
    const currentDate = new Date();
    const currentNepaliMonth = calendarFunctions.getBsMonthByAdDate(currentDate.getFullYear(), currentDate.getMonth() + 1, currentDate.getDate());
    return currentNepaliMonth;
};




