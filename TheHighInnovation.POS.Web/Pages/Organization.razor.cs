using System.Text.Json;
using Application.DTOs.Request;
using TheHighInnovation.POS.Web.Model.Response.Base;
using TheHighInnovation.POS.Web.Model.Response.Organization;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Organization
{
    private List<OrganizationResponseDto> _organizations { get; set; }
    private bool _showUpsertOrganizationDialog { get; set; }
    private bool _showDeleteOrganizationDialog { get; set; }
    private OrganizationRequestDto _organizationModel { get; set; }
    private string _dialogTitle { get; set; }
    private string _dialogOkLabel { get; set; }
    private string? _upsertOrganizationErrorMessage { get; set; }
    private string _deleteOrganizationErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _upsertOrganizationErrorMessage = "";
        
        var result = await BaseService.GetAsync<Derived<List<OrganizationResponseDto>>>("organization");

        _organizations = result?.Result ?? [];
    }

    private async Task OpenUpsertOrganizationDialog(int? organizationId = null)
    {
        _dialogTitle = "Add a new organization";

        _dialogOkLabel = "Add";

        _upsertOrganizationErrorMessage = "";

        if (organizationId.HasValue)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", organizationId.Value.ToString() },
            };

            var result = (await BaseService.GetAsync<Derived<OrganizationResponseDto>>("organization", parameters))?.Result;
            
            _organizationModel = new OrganizationRequestDto()
            {
                Id = result!.Id,
                Name = result.Name,
                Location = result.Location
            };
        }
        else
        {
            _organizationModel = new OrganizationRequestDto();
            
        }
        
        _showUpsertOrganizationDialog = true;

    }

    private async Task OnUpsertOrganizationDialogClose(bool isClosed)
    {
        try
        {
            if (isClosed)
            {
                _showUpsertOrganizationDialog = false;
            }
            else
            {
                if (string.IsNullOrEmpty(_organizationModel.Name) || string.IsNullOrEmpty(_organizationModel.Location))
                {
                    _upsertOrganizationErrorMessage = "Please fill in all the details.";
            
                    return;
                }
            
                var jsonRequest = JsonSerializer.Serialize(_organizationModel);

                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                await BaseService.PostAsync<Derived<object>>("organization", content);
            
                _showUpsertOrganizationDialog = false;

                await OnInitializedAsync();
            }
        }
        catch (Exception e)
        {
            _upsertOrganizationErrorMessage = e.Message;
        }
        
    }
}