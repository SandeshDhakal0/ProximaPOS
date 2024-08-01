using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using TheHighInnovation.POS.Web.Model.Request.Module;
using TheHighInnovation.POS.Web.Model.Response.Base;
using TheHighInnovation.POS.Web.Model.Response.Module;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Module
{
    private List<ModuleResponseDto> _modules { get; set; }
    private bool _showUpsertModuleDialog { get; set; }
    private bool _showDeleteModuleDialog { get; set; }
    private ModuleRequestDto _moduleModel { get; set; } = new();
    private string _dialogTitle { get; set; }
    private string _dialogOkLabel { get; set; }
    private string _upsertModuleErrorMessage { get; set; }
    private string _deleteModuleErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await BaseService.GetAsync<Derived<List<ModuleResponseDto>>>("module");

        _modules = result?.Result;
    }
    
    private async Task OpenUpsertModuleDialog(int? moduleId = null)
    {
        _dialogTitle = "Add a new module";

        _dialogOkLabel = "Add";

        _upsertModuleErrorMessage = "";

        _showUpsertModuleDialog = true;

        if (moduleId.HasValue)
        {
            var parameters = new Dictionary<string, string>
            {
                { "moduleId", moduleId.Value.ToString() },
            };

            var result = (await BaseService.GetAsync<Derived<ModuleResponseDto>>("module", parameters))?.Result;

            _moduleModel = new ModuleRequestDto()
            {
                Id = result!.Id,
                Name = result.Name,
                ModuleType = result.ModuleType,
                SequenceNumber = result.SequenceNumber,
                ParentModuleId = result.ParentModuleId,
                URL = result.URL
            };
        }
        else
        {
            _moduleModel = new ModuleRequestDto();
        }
    }

    private async Task OnUpsertModuleDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertModuleDialog = false;
        }
        else
        {
            if (string.IsNullOrEmpty(_moduleModel.Name) || _moduleModel.SequenceNumber == 0)
            {
                _upsertModuleErrorMessage = "Please fill in all the details";

                return;
            }
            var jsonRequest = JsonSerializer.Serialize(_moduleModel);

            var jsonContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("module", jsonContent);

            _showUpsertModuleDialog = false;

            await OnInitializedAsync();
        }
    }
}