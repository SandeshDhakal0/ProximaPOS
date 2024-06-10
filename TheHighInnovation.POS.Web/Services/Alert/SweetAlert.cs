// using Microsoft.JSInterop;
//
// namespace TheHighInnovation.POS.Web.Services.Alert
// {
//     public class SweetAlert
//     {
//         private readonly IJSRuntime _jsRuntime;
//
//         private List<ToastMessage> messages = new List<ToastMessage>();
//               
//         public SweetAlert(IJSRuntime jsRuntime)
//         {
//             _jsRuntime = jsRuntime;
//         }
//
//         public async Task Alert(string alert, string message, string status)
//         {
//            await _jsRuntime.InvokeVoidAsync("showAlert", alert, message, status);
//         }
//
//         public async Task<bool> Ask(string title)
//         {
//             return await _jsRuntime.InvokeAsync<bool>("AskBox", title);
//         }
//
//         public void ShowMessage(ToastType toastType, string title, string message)
//         {
//             messages.Add(CreateToastMessage(toastType, title, message));
//         }
//
//         public List<ToastMessage> GetMessages() => messages;
//
//         private ToastMessage CreateToastMessage(ToastType toastType, string title, string message)
//         {
//             return new ToastMessage
//             {
//                 Type = toastType,
//                 Title = title,
//                 HelpText = $"{DateTime.Now}",
//                 Message = message,
//             };
//         }
//
//
//     }
// }
