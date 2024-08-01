using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.Product;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TheHighInnovation.POS.Web.Model;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Response.Category;
using TheHighInnovation.POS.Web.Model.Response.Company;
using TheHighInnovation.POS.Web.Model.Response.Organization;
using TheHighInnovation.POS.Web.Model.Response.Product;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Product
{
	[CascadingParameter]
	private GlobalState _globalState { get; set; }

	private List<ProductResponseDto> _products { get; set; }

	private List<CompanyResponseDto>? _companies { get; set; }

	private List<CategoryResponseDto>? _categories { get; set; } = new();

	private List<OrganizationResponseDto>? _organizations { get; set; }

	private bool _showUpsertProductDialog { get; set; }

	private bool _showDeleteProductDialog { get; set; }

	private CreateProductRequestDto _productModel { get; set; } = new();

	private string _dialogTitle { get; set; }

	private string _dialogOkLabel { get; set; }

	private string _upsertProductErrorMessage { get; set; }

	private string _deleteProductErrorMessage { get; set; }

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

	private void SelectColor(string color)
	{
		SelectedColor = color;
		_productModel.ImageURL = SelectedColor;
	}

	private async Task HandleFilter()
	{
		Filter.PageSize = 10;

		var parameters = new Dictionary<string, string>
		{
			{ "categoryId", Filter.CategoryId.ToString()! },
			{ "search", Filter.Search },
			{ "pageNumber", "1" },
			{ "pageSize", Filter.PageSize.ToString() },
		};

		var products = await BaseService.GetAsync<Derived<List<ProductResponseDto>>>("product", parameters);

		_pagerDto = new PagerDto(products.TotalCount ?? 1, 1, 10);

		_products = products.Result ?? [];

		Filter.IsInitialized = true;
	}


	private void OnCategorySelection(ChangeEventArgs e)
	{
		if (e.Value != null && !string.IsNullOrWhiteSpace(e.Value.ToString()))
		{
			if (Int32.TryParse(e.Value.ToString(), out int categoryId))
			{
				Filter.CategoryId = categoryId;
			}
		}
		else
		{
			Filter.CategoryId = null;
		}

	}

	protected override async Task OnInitializedAsync()
	{
		if (_globalState.OrganizationId != null)
		{
			var parameters = new Dictionary<string, string>
			{
			{ "pageNumber", "1" },
			{ "pageSize", "1000" },
		};

			var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parameters);

			_categories = categories?.Result ?? [];

			Filter.PageSize = 10;

			var parametersinitial = new Dictionary<string, string>
			{
			{ "categoryId", Filter.CategoryId.ToString()! },
			{ "search", Filter.Search },
			{ "pageNumber", "1" },
			{ "pageSize", Filter.PageSize.ToString() },
			};

			var products = await BaseService.GetAsync<Derived<List<ProductResponseDto>>>("product", parametersinitial);

			_pagerDto = new PagerDto(products.TotalCount ?? 1, 1, 10);

			_products = products.Result ?? [];

			Filter.IsInitialized = true;
		}
		else
		{
			var parameters = new Dictionary<string, string>
			{
				{ "pageNumber", "1" },
				{ "pageSize", "1000" },
			};

			var organizations = await BaseService.GetAsync<Derived<List<OrganizationResponseDto>>>("organization", parameters);

			_organizations = organizations!.Result;
		}
	}

	private async Task OpenUpsertProductDialog(int? productId = null)
	{
		_dialogTitle = "Add a new product";

		_dialogOkLabel = "Add";

		_upsertProductErrorMessage = "";

		if (productId.HasValue)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "productId", productId.Value.ToString() },
			};

			var result = (await BaseService.GetAsync<Derived<ProductResponseDto>>("product", parameters))?.Result;

			_productModel = new CreateProductRequestDto()
			{
				Id = result!.Id,
				Title = result.Title,
				Description = result.Description,
				Unit = result.Unit,
				CategoryId = result.CategoryId,
				CompanyId = result.CompanyId,
				CostPrice = result.CostPrice,
				SalesPrice = result.SalesPrice,
				ImageURL = result.ProductImageUrl,
				IsActive = result.IsActive
			};
		}
		else
		{

			var parametersCat = new Dictionary<string, string>
				{
				{ "pageNumber", "1" },
				{ "pageSize", "1000" },
				 };

			var categories = await BaseService.GetAsync<Derived<List<CategoryResponseDto>>>("category", parametersCat);

			_categories = categories?.Result ?? [];

		}

		_showUpsertProductDialog = true;
	}



	private async Task OnUpsertProductDialogClose(bool isClosed)
	{
		try
		{
			if (isClosed)
			{
				_showUpsertProductDialog = false;
				_productModel = new();
				SelectedColor = "";
			}
			else
			{
				if (string.IsNullOrEmpty(_productModel.Title) || _productModel.CategoryId == 0 ||
					string.IsNullOrEmpty(_productModel.Description))
				{
					_upsertProductErrorMessage = "Please fill in all the details";

					return;
				}

				var jsonRequest = JsonSerializer.Serialize(_productModel);

				var jsonContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

				await BaseService.PostAsync<Derived<object>>("product", jsonContent);

				_showUpsertProductDialog = false;
				_productModel = new();
				SelectedColor = "";
				await OnInitializedAsync();

			}
		}
		catch (Exception e)
		{
			_upsertProductErrorMessage = e.Message;
		}

	}


	private async Task HandlePaginationChange(ChangeEventArgs e)
	{
		if (e.Value == null) return;

		var pageSize = int.Parse(e.Value.ToString()!);

		Filter.PageSize = pageSize;

		await OnPagination(1);
	}

	private async Task OnPagination(int pageNumber)
	{
		var pageSize = Filter.PageSize;

		var parameters = new Dictionary<string, string>
		{
			{ "categoryId", Filter.CategoryId.ToString() },
			{ "pageNumber", pageNumber.ToString() },
			{ "pageSize", pageSize.ToString() },
		};

		var products = await BaseService.GetAsync<Derived<List<ProductResponseDto>>>("product", parameters);

		_pagerDto = new PagerDto(products.TotalCount ?? 1, pageNumber, pageSize);

		_products = products?.Result ?? [];
	}
}