using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Request.VendorManagement;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages.ProductMaster
{
    public partial class InventoryCategory
    {
        [CascadingParameter]
        private GlobalState _globalState { get; set; }

        private bool OpenCategoryModel = false;
        private bool isClosed = false;
        private string? _message { get; set; }
        private string? _dialogTitle { get; set; }
        private string? _dialogOkLabel { get; set; }
        private CategoryResponseDTO _categoryResponseDTO = new();
        private CategoryFilter categoryFilter = new();
        private List<CategoryList> categoryList { get; set; } = new();
        private PagerDto _pagerDto { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            _globalState = await BaseService.GetGlobalState();

            categoryFilter.IsInitialized = true;
            await LoadCategoryAsync(1);
        }

        private void OpenCategoryUpsertModel()
        {
            OpenCategoryModel = true;
            _dialogTitle = "Add a new Category";
            _dialogOkLabel = "Add";
            _message = null;
        }
        private void OpenCategoryUpsertModelUpdate()
        {
            OpenCategoryModel = true;
            _dialogTitle = "Update Category";
            _dialogOkLabel = "Update";
            _message = null;
        }

        private async Task OnUpsertCategory(bool isClosed)
        {
            try
            {
                if (isClosed)
                {
                    OpenCategoryModel = false;
                    return;
                }

                var category = new
                {
                    p_id = _categoryResponseDTO.Id > 0 ? _categoryResponseDTO.Id : 0,
                    p_category_name = _categoryResponseDTO.CategoryName,
                    p_category_description = _categoryResponseDTO.CategoryDescription
                };

                var jsonRequest = JsonSerializer.Serialize(category);
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                var apiEndpoint = "VendorManagement/upsert-category";
                var result = await BaseService.PostAsync<Derived<object>>(apiEndpoint, content);
                if (result.Status == "Success")
                {
                    OpenCategoryModel = false;
                    await SweetAlertService.Alert("Success", result.Message, "success");
                    ClearCategoryForm();
                    await OnInitializedAsync(); // Refresh category list after upsert
                }
            }
            catch (Exception ex)
            {
                OpenCategoryModel = true;
                _message = ex.Message;
                Console.WriteLine(_message.ToString());
            }
        }

        private async Task HandleSearch()
        {
            await LoadCategoryAsync(1);
        }

        private async Task LoadCategoryAsync(int pageNumber)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "pageno", pageNumber.ToString() },
                    { "pagesize", categoryFilter.PageSize.ToString() },
                    { "categorycode", categoryFilter.CategoryCode ?? string.Empty },
                    { "categoryname", categoryFilter.CategoryName ?? string.Empty }
                };

                var categorylist = await BaseService.GetAsync<Derived<List<CategoryList>>>("VendorManagement/get-category", parameters);
                if (categorylist != null && categorylist.Result != null)
                {
                    categoryList = categorylist.Result;
                    _pagerDto = new PagerDto(categorylist.TotalCount ?? 1, pageNumber, categoryFilter.PageSize);
                    _message = "Category loaded successfully.";
                }
                else
                {
                    _message = "No Category found.";
                }
            }
            catch (Exception ex)
            {
                _message = $"Failed to load Category: {ex.Message}";
            }
        }

        private async Task LoadCategoryForUpdate(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "categoryid", categoryId.ToString() }
                };

                var response = await BaseService.GetAsync<Derived<List<CategoryList>>>("VendorManagement/get-category-by-id", parameters);

                if (response != null && response.Status == "Success")
                {
                    _categoryResponseDTO = new CategoryResponseDTO
                    {
                        Id = categoryId,
                        CategoryName = response.Result.First().CategoryName,
                        CategoryDescription = response.Result.First().CategoryDescription,
                    };
                    OpenCategoryUpsertModelUpdate();
                }
                else
                {
                    _message = response?.Message ?? "Failed to load category.";
                }
            }
            catch (Exception ex)
            {
                _message = $"Failed to load category: {ex.Message}";
            }
        }


        private async Task HandlePaginationChange(ChangeEventArgs e)
        {
            if (e.Value == null) return;

            var pageSize = int.Parse(e.Value.ToString()!);

            categoryFilter.PageSize = pageSize;

            await OnPagination(1);
        }

        private async Task OnPagination(int pageNumber)
        {
            await LoadCategoryAsync(pageNumber);
        }

        private void ClearCategoryForm()
        {
            _categoryResponseDTO = new();
            _message = null;
        }
    }
}
