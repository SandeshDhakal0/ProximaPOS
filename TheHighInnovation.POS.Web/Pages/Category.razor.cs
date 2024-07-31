using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TheHighInnovation.POS.Model.Request.Filter;
using TheHighInnovation.POS.Model.Response.Category;
using TheHighInnovation.POS.Model.Response.Company;
using TheHighInnovation.POS.Model.Response.Organization;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Category
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }
    
    private List<CategoryResponseDto> _categories { get; set; }

    private List<CategoryResponseDto> _parentCategories { get; set; } = new();

    private List<OrganizationResponseDto>? _organizations { get; set; }
    
    private List<CompanyResponseDto>? _companies { get; set; }
    
    private bool _showUpsertCategoryDialog { get; set; }
    
    private bool _showDeleteCategoryDialog { get; set; }
    
    private CategoryRequestDto _categoryModel { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertCategoryErrorMessage { get; set; }
    
    private string _deleteCategoryErrorMessage { get; set; }

    public string selectedImage { get; set; }
    
    private FilterRequestDto Filter = new();
    
    private PagerDto _pagerDto { get; set; } = new();
    
    private async Task HandleFilter()
    {
        try
        {
            Filter.PageSize = 10;
           
            var parameters = new Dictionary<string, string>
            {
                { "companyId", _globalState.CompanyId.ToString() },
                { "search", Filter.Search },
                { "pageNumber", "1" },
                { "pageSize", Filter.PageSize.ToString() },
            };

            var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);

            _pagerDto = new PagerDto(categories.TotalCount ?? 1, 1, 10);

            _categories = categories.Result ?? [];
            
            Filter.IsInitialized = true;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task OnOrganizationSelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;
        
        var organizationId = Int32.Parse(e.Value.ToString());

        Filter.OrganizationId = organizationId;
        
        Filter.CompanyId = 0;

        var parameters = new Dictionary<string, string>
        {
            { "organizationId", organizationId.ToString() },
            { "pageNumber", "1" },
            { "pageSize", "1000" },
        };

        var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
            
        _companies = companies?.Result ?? [];
    }
    private void clearsearch()
    {
        Filter.Search = "";

	}
    
  /*  private void OnCompanySelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;
        
        var companyId = Int32.Parse(e.Value.ToString());

        Filter.CompanyId = companyId;
    }
    */
    protected override async Task OnInitializedAsync()
    {
      /*  if (_globalState.OrganizationId != null)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", _globalState.OrganizationId.ToString()! },
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);

            _companies = companies!.Result;*/

            if(_globalState.CompanyId != null)
            {
                try
                {
                    Filter.PageSize = 10;

                    var parametersCat = new Dictionary<string, string>
                {
                { "companyId", _globalState.CompanyId.ToString() },
                { "search", Filter.Search },
                { "pageNumber", "1" },
                { "pageSize", Filter.PageSize.ToString() },
                };

                    var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parametersCat);

                    _pagerDto = new PagerDto(categories.TotalCount ?? 1, 1, 10);

                    _categories = categories.Result ?? [];

                    Filter.IsInitialized = true;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }      
    }

    private async Task OpenUpsertCategoryDialog(int? categoryId = null)
    {
        try
        {
            _dialogTitle = "Add a new category";

            _dialogOkLabel = "Add";

            _upsertCategoryErrorMessage = "";

            if (categoryId.HasValue)
            {
                var parameters = new Dictionary<string, string>
                {
                    { "categoryId", categoryId.Value.ToString() },
                };

                var result = (await BaseService.GetAsync<Derived<CategoryResponseDto>>("category", parameters))?.Result;

                _categoryModel = new CategoryRequestDto()
                {
                    Id = result!.Id,
                    Title = result.Title,
                    Description = result.Description,
                    CompanyId = result.CompanyId,
                    ParentCategoryId = result.ParentCategoryId,
                    ImageURL = result.ImageUrl
                };
                
                var parameterForCompany = new Dictionary<string, string>
                {
                    { "companyId", _globalState.CompanyId.ToString() },
                };
            
                var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameterForCompany);
            
                _parentCategories = categories?.Result ?? [];

                _categoryModel.CompanyId = result.CompanyId;
            }
            else
            {
                _categoryModel = new CategoryRequestDto();

                if (_globalState.OrganizationId != null)
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "organizationId", _globalState.OrganizationId.ToString()! },
                        { "pageNumber", "1" },
                        { "pageSize", "1000" },
                    };

                    var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
                    _companies = companies?.Result ?? [];
                
                }
                else
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "pageNumber", "1" },
                        { "pageSize", "1000" },
                    };

                    var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
                    _companies = companies?.Result ?? [];
                }
            }
            contentData = new();
            _showUpsertCategoryDialog = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string uploadedFileUrl = "";

    private async Task OnInputFileChanged(InputFileChangeEventArgs e)
    {
        using var formData = new MultipartFormDataContent();

        foreach (var file in e.GetMultipleFiles(2))
        {
            var fileContent = new StreamContent(file.OpenReadStream(long.MaxValue));
            
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            formData.Add(content: fileContent, name: "Files", fileName: file.Name);
        }

        formData.Add(new StringContent("1"), "FilePath");

        var response = await Http.PostAsync("api/file-upload", formData);

        var uploadedResult = await response.Content.ReadFromJsonAsync<Derived<List<string>>>();

        uploadedFileUrl = uploadedResult!.Result.FirstOrDefault()!;
    }

    
    private async Task OnUpsertCategoryDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertCategoryDialog = false;
        }
        else
        {
            if (string.IsNullOrEmpty(_categoryModel.Title) || _categoryModel.CompanyId == 0)
            {
                _upsertCategoryErrorMessage = "Please fill in all the details";
            
                return;
            }
            
            var jsonRequest = JsonSerializer.Serialize(_categoryModel);
            
            var stringContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            
            await BaseService.PostAsync<Derived<object>>("category", stringContent);

            _showUpsertCategoryDialog = false;

            await OnInitializedAsync();
            
            uploadedFileUrl = "";
        }
        
        if (Filter.OrganizationId is not 0)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", Filter.OrganizationId.ToString()! },
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
            _companies = companies?.Result ?? [];
        }
    }
    
    private string uploadedFileName = string.Empty;
    private long maxFileSize = long.MaxValue;
    
    private MultipartFormDataContent contentData = new();
    
    private void HandleFileSelection (InputFileChangeEventArgs e)
    {
        var file = e.GetMultipleFiles().FirstOrDefault();

        if (file == null) return;
        
        var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
        
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            
        contentData.Add(content: fileContent, name: "file", fileName: file.Name);

        uploadedFileName = file.Name;
    }

    private async Task HandleCompanyChange(ChangeEventArgs e)
    {
        var selectedCompanyId = e.Value?.ToString();

        var result = int.TryParse(selectedCompanyId, out int companyId);

        if (result)
        {
            var parameters = new Dictionary<string, string>
            {
                { "companyId", companyId.ToString() },
            };
            
            var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);
            
            _parentCategories = categories?.Result ?? [];

            _categoryModel.CompanyId = companyId;
        }
    }
    
    private async Task HandlePaginationChange(ChangeEventArgs e)
    {
        if(e.Value == null) return;
        
        var pageSize = int.Parse(e.Value.ToString()!);

        Filter.PageSize = pageSize;
	    
        await OnPagination(1);
    }

    private async Task OnPagination(int pageNumber)
    {
        var pageSize = Filter.PageSize;
	    
        var parameters = new Dictionary<string, string>
        {
            { "companyId", _globalState.CompanyId.ToString() },
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
        };

        var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);

        _pagerDto = new PagerDto(categories.TotalCount ?? 1, pageNumber, pageSize);

        _categories = categories?.Result ?? [];
    }
}