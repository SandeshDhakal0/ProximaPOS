using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Response.Category;
using TheHighInnovation.POS.Web.Model.Response.Company;
using TheHighInnovation.POS.Web.Model.Response.Organization;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Category
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }

    private List<CategoryResponseDto> _categories { get; set; } = new();
    private List<CategoryResponseDto> _parentCategories { get; set; } = new();
    private List<OrganizationResponseDto>? _organizations { get; set; }
    private List<CompanyResponseDto>? _companies { get; set; }

    private bool _showUpsertCategoryDialog { get; set; }
    private bool _showDeleteCategoryDialog { get; set; }

    private CategoryRequestDto _categoryModel { get; set; } = new();
    private string _dialogTitle { get; set; }
    private string _dialogOkLabel { get; set; }
    private string _upsertCategoryErrorMessage { get; set; }
    private string _deleteCategoryErrorMessage { get; set; }

    private FilterRequestDto Filter = new();
    private PagerDto _pagerDto { get; set; } = new();

    private List<string> ColorOptions = new()
    {
        "#FF5733", // Red-Orange
        "#33FF57", // Green
        "#3357FF", // Blue
        "#F0F0F0"  // Light Gray
    };

    private string SelectedColor { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            Filter.PageSize = 10;
            var parameters = new Dictionary<string, string>
            {
                { "search", Filter.Search },
                { "pageNumber", "1" },
                { "pageSize", Filter.PageSize.ToString() }
            };

            var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);

            _pagerDto = new PagerDto(categories.TotalCount ?? 1, 1, 10);
            _categories = categories.Result ?? new List<CategoryResponseDto>();
            Filter.IsInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
            _upsertCategoryErrorMessage = "An error occurred while loading categories. Please try again later.";
        }
    }

    private async Task HandleFilter()
    {
        await LoadCategoriesAsync();
    }

    private void SelectColor(string color)
    {
        SelectedColor = color;
        _categoryModel.ImageURL = SelectedColor;
    }

    private async Task OpenUpsertCategoryDialog(int? categoryId = null)
    {
        try
        {
            _dialogTitle = categoryId.HasValue ? "Edit Category" : "Add a new category";
            _dialogOkLabel = categoryId.HasValue ? "Update" : "Add";
            _upsertCategoryErrorMessage = "";

            if (categoryId.HasValue)
            {
                var parameters = new Dictionary<string, string> { { "categoryId", categoryId.Value.ToString() } };
                var result = (await BaseService.GetAsync<Derived<CategoryResponseDto>>("category", parameters))?.Result;

                if (result != null)
                {
                    _categoryModel = new CategoryRequestDto
                    {
                        Id = result.Id,
                        Title = result.Title,
                        Description = result.Description,
                        CompanyId = result.CompanyId,
                        ParentCategoryId = result.ParentCategoryId,
                        ImageURL = result.ImageUrl
                    };
                }
            }
            else
            {
                _categoryModel = new CategoryRequestDto { ImageURL = SelectedColor };
            }

            var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category");
            _parentCategories = categories?.Result ?? new List<CategoryResponseDto>();
            _showUpsertCategoryDialog = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening category dialog: {ex.Message}");
            _upsertCategoryErrorMessage = "An error occurred while opening the category dialog. Please try again later.";
        }
    }

    private async Task OnUpsertCategoryDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertCategoryDialog = false;
            _categoryModel = new CategoryRequestDto();
            return;
        }

        if (string.IsNullOrEmpty(_categoryModel.Title))
        {
            _upsertCategoryErrorMessage = "Please fill in all the details";
            return;
        }

        try
        {
            var jsonRequest = JsonSerializer.Serialize(_categoryModel);
            var stringContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("category", stringContent);
            _showUpsertCategoryDialog = false;
            _categoryModel = new CategoryRequestDto();
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upserting category: {ex.Message}");
            _upsertCategoryErrorMessage = "An error occurred while saving the category. Please try again later.";
        }
    }

    private async Task HandlePaginationChange(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        Filter.PageSize = int.Parse(e.Value.ToString()!);
        await OnPagination(1);
    }

    private async Task OnPagination(int pageNumber)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {

                { "pageNumber", pageNumber.ToString() },
                { "pageSize", Filter.PageSize.ToString() }
            };

            var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);

            _pagerDto = new PagerDto(categories.TotalCount ?? 1, pageNumber, Filter.PageSize);
            _categories = categories?.Result ?? new List<CategoryResponseDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during pagination: {ex.Message}");
            _upsertCategoryErrorMessage = "An error occurred while paginating categories. Please try again later.";
        }
    }
}
