using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.Product;
using TheHighInnovation.POS.Model.Request.Service;
using TheHighInnovation.POS.Model.Response.Base;
using TheHighInnovation.POS.Model.Response.Category;
using TheHighInnovation.POS.Model.Response.Company;
using TheHighInnovation.POS.Model.Response.Product;
using TheHighInnovation.POS.Model.Response.Service;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Service
{
    private List<ServiceResponseDto> _services { get; set; }
    
    private bool _showUpsertServiceDialog { get; set; }
    
    private bool _showDeleteServiceDialog { get; set; }
    
    private ServiceRequestDto _serviceModel { get; set; } = new();
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertServiceErrorMessage { get; set; }
    
    private string _deleteServiceErrorMessage { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var result = await BaseService.GetAsync<Derived<List<ServiceResponseDto>>>("service");

        _services = result?.Result ?? [];
    }

    private async Task OpenUpsertServiceDialog(int? serviceId = null)
    {
        _dialogTitle = "Add a new service";

        _dialogOkLabel = "Add";

        _upsertServiceErrorMessage = "";

        _showUpsertServiceDialog = true;

        if (serviceId.HasValue)
        {
            var parameters = new Dictionary<string, string>
            {
                { "serviceId", serviceId.Value.ToString() },
            };

            var result = (await BaseService.GetAsync<Derived<ServiceResponseDto>>("service", parameters))?.Result;

            _serviceModel = new ServiceRequestDto()
            {
                Id = result!.Id,
                Name = result.Name,
                Description = result.Description
            };
        }
        else
        {
            _serviceModel = new ServiceRequestDto();
        }
    }

    private async Task OnUpsertServiceDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertServiceDialog = false;
        }
        else
        {
            if (string.IsNullOrEmpty(_serviceModel.Name) || string.IsNullOrEmpty(_serviceModel.Description))
            {
                _upsertServiceErrorMessage = "Please fill in all the details";

                return;
            }
            
            var jsonRequest = JsonSerializer.Serialize(_serviceModel);

            var jsonContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("service", jsonContent);

            _showUpsertServiceDialog = false;
            
            await OnInitializedAsync();
        }
    }
}