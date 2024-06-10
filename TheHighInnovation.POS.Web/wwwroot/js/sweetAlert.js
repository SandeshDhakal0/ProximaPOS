function showAlert(title, message, type) {
    Swal.fire({
        title: title,
        text: message,
        icon: type,
        timer: 2500,
        showConfirmButton: false,
        timerProgressBar: true
    });
}
function AskBox(title) {
    return Swal.fire({
        title: title,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: "Yes",
        denyButtonText: `No`
    }).then((result) => {
        return result.isConfirmed;
    });
}


