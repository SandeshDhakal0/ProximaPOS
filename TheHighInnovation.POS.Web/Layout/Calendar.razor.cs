using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Layout;

public partial class Calendar
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
}