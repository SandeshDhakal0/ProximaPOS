using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;
using TheHighInnovation.POS.Model.Request.Customer;
using TheHighInnovation.POS.Model.Request.Product;
using TheHighInnovation.POS.Model.Request.SafeDrop;
using TheHighInnovation.POS.Model.Response;
using TheHighInnovation.POS.Model.Response.CloseTill;
using TheHighInnovation.POS.Model.Response.Hold;
using TheHighInnovation.POS.Model.Response.Item;
using TheHighInnovation.POS.Model.Response.Product;
using TheHighInnovation.POS.Model.Response.Reservation;
using TheHighInnovation.POS.Web.Models;
using TheHighInnovation.POS.Web.Services.Reports;

namespace TheHighInnovation.POS.Web.Pages;
public partial class CashierCorner()
{
    private IEnumerable<ItemResponseDto> _items = Array.Empty<ItemResponseDto>();
    private IEnumerable<AddProductRequestDto> _itemsToAdd = Array.Empty<AddProductRequestDto>();

    private List<CategoryProductResponseDto> _subcategories = new List<CategoryProductResponseDto>();
    private AddProductRequestDto? _selectedProduct;
    private bool _isDiscountSelected = false;
    private bool _isSaleSelected = false;
    private string _calculatorAmount;
    private string _tenderAmount;
    private bool _showMainCategories = true;
    private int _selectedCategoryId;
    private decimal _totalAmount = 0;
    private decimal _subTotalAmount = 0;
    private decimal _taxAmount = 0;
    private decimal _discount = 0;
    private decimal _savedDiscount = 0;
    private decimal _totalDiscount = 0;
    private decimal _itemDiscount = 0;
    private bool _isCustomerSelected = false;
    private bool _showLandingPage = true;
    private bool _showCashView = false;
    private bool _showSplitView = false;
    private bool _showCardView = false;
    private bool _showScanView = false;
    private bool _showCustomerView = false;
    private bool _showAddCustomerView = false;
    private bool _showRemarksView = false;
    private string _title = "Cashier Corner";
    private bool _showLandingCashierView = true;
    private string _message = "";

    #region Hold Items

    private bool _showHoldDialog { get; set; } = false;
    private bool _showHoldDialogtable { get; set; } = false;
    private bool _showholdtablePrintOptions { get; set; } = false;
    private bool _showReservedDialogtable { get; set; } = false;
    private bool _showProductsOnHold { get; set; } = false;

    private bool _showProductsOnHoldForTable { get; set; } = false;
    
    private List<HoldProductRequestDto> _holdProducts { get; set; } = new();
    private List<HoldProductRequestDto> _holdTakeAway { get; set; } = new();
    private HoldProductRequestDto _holdProductsView { get; set; } = new();
    private List<HoldTableResponseDto> _holdTables { get; set; } = new(); 
    private List<HoldRoomResponseDto> _holdRooms { get; set; } = new();
    int _tableId = 0;


    private async void ShowHoldDialog()
    {
        _showHoldDialog = true;
        
        var response = await BaseService.GetAsync<Derived<List<Hold>>>("hold/get-takeaway-item");

        _showHoldDialog = true;

        if (response == null) return;

        _holdTakeAway = [];
        
        foreach (var holdProductTogo in response.Result.Select(hold => new HoldProductRequestDto {
                     Id = hold.HoldId,
                     DiscountAmount = hold.DiscountAmount ?? 0m,
                     ProductsOnHold = hold.ProductDetails.Select(productDetail => new AddProductRequestDto
                     {
                         Id = productDetail.Id,
                         Title = productDetail.Name,
                         Quantity = productDetail.Quantity,
                         UnitPrice = productDetail.ProductAmount,
                         Discount = productDetail.DiscountAmount,
                     }).ToList()
                 }))
        {
            _holdTakeAway.Add(holdProductTogo);
        }
                    
        _showHoldDialog = true;

        _itemsToAdd = new List<AddProductRequestDto>();
    } 

    private void ShowPrintOption(int tableid)
    {
        _showHoldDialogtable = false;
        _showholdtablePrintOptions = true;        
        _tableId = tableid;


    }
    private void ClosePrintOption()
    {
        
        _showholdtablePrintOptions = false;
        _showHoldDialogtable = true;

    }
    private async Task ShowHoldDialogtable()
    {
        _showHoldDialogtable = true;

        var holdTables = await BaseService.GetAsync<Derived<List<HoldTableResponseDto>>>("hold/get-hold-tables");

        _holdTables = holdTables?.Result ?? new();

        var holdRooms = await BaseService.GetAsync<Derived<List<HoldRoomResponseDto>>>("hold/get-hold-rooms");

        _holdRooms = holdRooms?.Result ?? new();
    }

    
    private List<ReservationResponseDto>? ReservationList { get; set; }

    private async Task ShowReservedDialogTable()
    {
        _showReservedDialogtable = true;

        HoldType = "Reservation";
        
        var parameters = new Dictionary<string, string>
        {
            { "page_no", "1" },
            { "page_size", 100.ToString() },
            { "current_status", "Checked-In" }
        };
        
        var reservations = await BaseService.GetAsync<Derived<List<ReservationResponseDto>>>("Reservation/reservation-active-list", parameters);

        ReservationList = reservations.Result.ToList();
    }

    private async Task HoldItems(int itemId, string type)
    {
        if(!_itemsToAdd.Any()) return;
        
        if (type == "Reservation")
        {
            await ReturnItems(itemId, type);
            
            var holdDetails = _itemsToAdd.Select(x => new
            {
                id = x.Id,
                name = x.Title,
                quantity = x.Quantity,
                productAmount = x.UnitPrice,
                discountAmount = x.Discount,
            });
            
            var result = new
            {
                p_hold_type = type,
                p_hold_id = itemId,
                p_is_active = true,
                p_product_details = holdDetails,
                p_discount_amount = _discount
            };
            var jsonRequest = JsonSerializer.Serialize(result);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("hold/hold-items", content);

            _itemsToAdd = new List<AddProductRequestDto>();

            _showHoldDialogtable = false;

            _showReservedDialogtable = false;

            _subTotalAmount = 0;

            _taxAmount = 0;

            _itemDiscount = 0;

            _discount = 0;

            _savedDiscount = _discount;
        
            _totalDiscount = 0;

            _totalAmount = 0;
        
            _selectedProduct = null;
            _showholdtablePrintOptions = false;
            _showHoldDialogtable = false;
        }
        else
        {
            var holdDetails = _itemsToAdd.Select(x => new
            {
                id = x.Id,
                name = x.Title,
                quantity = x.Quantity,
                productAmount = x.UnitPrice,
                discountAmount = x.Discount,
            });
            
            var result = new
            {
                p_hold_type = type,
                p_hold_id = _tableId,
                p_is_active = true,
                p_product_details = holdDetails,
                p_discount_amount = _discount
            };
            var jsonRequest = JsonSerializer.Serialize(result);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("hold/hold-items", content);

            _itemsToAdd = new List<AddProductRequestDto>();

            _showHoldDialogtable = false;

            _showReservedDialogtable = false;

            _subTotalAmount = 0;

            _taxAmount = 0;

            _itemDiscount = 0;

            _discount = 0;

            _savedDiscount = _discount;
        
            _totalDiscount = 0;

            _totalAmount = 0;
        
            _selectedProduct = null;
            _showholdtablePrintOptions = false;
            _showHoldDialogtable = false;
        }
    }

   /* private CCReservationResponse reservationResponse = new();

    private async Task GetReservationItem(int itemId)
    {
        var parameter = new Dictionary<string, string>
        {
            {"id", itemId.ToString() }
        };
        var reservation = await BaseService.GetAsync<Derived<BookingResponseDto>>("Reservation/reservation-active-list-by-id", parameter);
        if(reservation != null)
        {
            reservationResponse = new CCReservationResponse
            {
                CustomerId = reservation.Result.CustomerId,
                RoomDetails = reser

            };
        }
    }*/

    private async Task ReturnItems(int itemId, string type)
    {
        var response = await BaseService.GetAsync<Derived<List<Hold>>>("hold/get-hold-item");

        if (response == null) return;
        
        var result = response.Result.FirstOrDefault(x => x.HoldId == itemId && x.HoldType == type);

        HoldType = type;
        
        if (_itemsToAdd.Any())
        {
            foreach(var item in result!.ProductDetails)
            {
                if (_itemsToAdd.Any(x => x.Id == item.Id))
                {
                    var existingItem = _itemsToAdd.FirstOrDefault(x => x.Id == item.Id);

                    if (existingItem == null) continue;
                    
                    existingItem.Quantity += item.Quantity;
                    existingItem.Discount += item.DiscountAmount;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice - existingItem.Discount;
                }
                else
                {
                    var newRecord = new AddProductRequestDto()
                    {
                        Id = item.Id,
                        Discount = item.DiscountAmount,
                        Quantity = item.Quantity,
                        Title = item.Name,
                        UnitPrice = item.ProductAmount,
                        TotalPrice = item.ProductAmount * item.Quantity - item.DiscountAmount
                    };

                    _itemsToAdd = _itemsToAdd.Append(newRecord).ToList();
                }
            }
          
                await BaseService.DeleteAsync<Derived<object>>($"hold/delete-hold-item?id={itemId}&holdType={type}");
            
            
            _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
    
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _discount = result.DiscountAmount + _discount ?? 0;
    
            _totalDiscount = _itemDiscount + result.DiscountAmount + _discount ?? 0;
    
            _taxAmount = (decimal)0.13 * _subTotalAmount;
    
            _totalAmount = _subTotalAmount + _taxAmount - _discount;

            if (type != "Reservation")
            {
                await HoldItems(itemId, type);
            }
        }
        else
        {
            _holdProductsView.Id = itemId;
            
            _holdProductsView.DiscountAmount = result?.DiscountAmount ?? 0;

            if (result?.ProductDetails != null)
            {
                _holdProductsView.ProductsOnHold = result.ProductDetails.Select(x => new AddProductRequestDto()
                {
                    Id = x.Id,
                    Title = x.Name,
                    UnitPrice = x.ProductAmount,
                    Quantity = x.Quantity,
                    Discount = x.DiscountAmount,
                    TotalPrice = x.ProductAmount * x.Quantity - x.DiscountAmount ,
                }).ToList();
                
                _holdProductsView.TotalAmount = _holdProductsView.ProductsOnHold.Sum(x => x.TotalPrice);
            }
            
            _showHoldDialogtable = false;

            _showProductsOnHoldForTable = true;
        }
    }

    public string HoldType = "";
    public int _reservationId = 0;
    
    private async Task ReturnItemsToCashierCorner(int itemId, string type)
    {
        if (type == "Reservation")
        {
            var parameters = new Dictionary<string, string>
            {
                { "id", itemId.ToString() }
            };
            
            var reservation = await BaseService.GetAsync<Derived<BookingResponseDto>>("Reservation/reservation-active-list-by-id", parameters);
        
            var customer = await BaseService.GetAsync<Derived<SelectCustomerRequestDto>>("customer/get-customer-by-id?customerId=" + reservation?.Result.CustomerId);
        
            _customer = customer?.Result!;
            _reservationId = itemId;

        }
        
        if (_itemsToAdd.Any()) return;

        var response = await BaseService.GetAsync<Derived<List<Hold>>>("hold/get-hold-item");

        if (response == null) return;

        var result = response.Result.FirstOrDefault(x => x.HoldId == itemId && x.HoldType == type);

        foreach(var item in result!.ProductDetails)
        {
            var newRecord = new AddProductRequestDto()
            {
                Id = item.Id,
                Discount = item.DiscountAmount,
                Quantity = item.Quantity,
                Title = item.Name,
                UnitPrice = item.ProductAmount,
                TotalPrice = item.ProductAmount * item.Quantity - item.DiscountAmount
            };

            _itemsToAdd = _itemsToAdd.Append(newRecord).ToList();
        }
        if(type != "Reservation")
        {
            await BaseService.DeleteAsync<Derived<object>>($"hold/delete-hold-item?id={itemId}&holdType={type}");

        }

        _showHoldDialogtable = false;
        _showReservedDialogtable = false;
        
        _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
        
        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

        _discount = result.DiscountAmount ?? 0;
        
        _totalDiscount = _itemDiscount + result.DiscountAmount ?? 0;
        
        _taxAmount = (decimal)0.13 * _subTotalAmount;
        
        _totalAmount = _subTotalAmount + _taxAmount - _discount;

        _showProductsOnHoldForTable = false;
    }
    
    private void ShowProductsOnHoldDialog()
    {
        _showHoldDialog = false;
        
        _showProductsOnHold = true;
    }

    private void CloseCustomerDialog()
    {
        _showAddCustomerView = false;
        _addCustomer.p_address = "";
        _addCustomer.p_contact = "";
        _addCustomer.p_first_name = "";
        _addCustomer.p_last_name = "";
    }

    private async void AddProductsOnHold()
    {
        if (!_itemsToAdd.Any()) return;
        var takeaway = _itemsToAdd.Select(x => new
        {
            id = x.Id,
            name = x.Title,
            quantity = x.Quantity,
            productAmount = x.UnitPrice,
            discountAmount = x.Discount,
        });

        var result = new
        {
            p_hold_type = "TakeAway",
            p_hold_id = 0,
            p_is_active = true,
            p_product_details = takeaway.ToList(),
            p_discount_amount = _discount
        };

        var jsonRequest = JsonSerializer.Serialize(result);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        await BaseService.PostAsync<Derived<object>>("hold/hold-items", content);
        _itemsToAdd = new List<AddProductRequestDto>();

        _showHoldDialog = false;

        _showProductsOnHold = false;
        _subTotalAmount = 0;
        _taxAmount = 0;
        _itemDiscount = 0;

        _discount = 0;

        _savedDiscount = _discount;

        _totalDiscount = 0;

        _totalAmount = 0;
  
        _selectedProduct = null;

    }

    /*  private void AddProductsOnHold()
      {
          var maxId = _holdProducts.Any() ? _holdProducts.Max(x => x.Id) : 0;

          _holdProducts.Add(new HoldProductRequestDto()
          {
              Id = maxId + 1,
              TotalAmount = _totalAmount,
              DiscountAmount = _discount,
              ProductsOnHold = _itemsToAdd.ToList()
          });

          _showHoldDialog = false;

          _showProductsOnHold = false;

          _subTotalAmount = 0;  
          _taxAmount = 0;
          _itemDiscount = 0;
          _discount = 0;
          _totalDiscount = 0;
          _totalAmount = 0;

          _itemsToAdd = new List<AddProductRequestDto>();
      }
      */
    private void ModalCancel()
    {
        _showReservedDialogtable = false;
        _showHoldDialogtable = false;
        _showHoldDialog = false;
        _showProductsOnHold = false;
        _showProductsOnHoldForTable = false;
    }
    
    private void ModalCanceltable()
    {
        _showHoldDialogtable = false;
        _showReservedDialogtable = false;

    }

    private void AddProductsOnHoldToCashierCorner(int id)
    {
        var selectedHoldProduct = _holdProducts.FirstOrDefault(x => x.Id == id);

        if (selectedHoldProduct == null) return;
        
        _itemsToAdd = selectedHoldProduct.ProductsOnHold;
        
        _totalAmount = selectedHoldProduct.TotalAmount;

        _discount = selectedHoldProduct.DiscountAmount;

        _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
        
        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

        _totalDiscount = _itemDiscount + _discount;
        
        _taxAmount = (decimal)0.13 * _subTotalAmount;
        
        _totalAmount = _subTotalAmount + _taxAmount;

        _holdProducts.Remove(selectedHoldProduct);
        
        _showHoldDialog = false;

        _showProductsOnHold = false;
    }
    #endregion

    #region Safe Drop
    private  bool _showSafeDrop { get; set; } = false;

    private CashDetails CashDetails { get; set; } = new();
    
    private void ShowSafeDrop()
    {
        _showLandingCashierView = false;
        _showSafeDrop = true;
        _showCloseTill = false; // Reset the state of close till
    }
    
    private async Task SafeDropCash()
    {
        var totalAmount = CashDetails.Rs1 * 1 + CashDetails.Rs2 * 2 + CashDetails.Rs5 * 5 + 
                             CashDetails.Rs10 * 10 + CashDetails.Rs20 * 20 + CashDetails.Rs50 * 50 + 
                             CashDetails.Rs100 * 100 + CashDetails.Rs500 * 500 + CashDetails.Rs1000 * 1000;
        
        var safeDropRequestDto = new SafeDropRequestDto()
        {
            p_cashierid = _globalState.UserId,
            p_totalamount = totalAmount,
            p_amount = new CashDetails()
            {
                Rs1 = CashDetails.Rs1,
                Rs2 = CashDetails.Rs2,
                Rs5 = CashDetails.Rs5,
                Rs10 = CashDetails.Rs10,
                Rs20 = CashDetails.Rs20,
                Rs50 = CashDetails.Rs50,
                Rs100 = CashDetails.Rs100,
                Rs500 = CashDetails.Rs500,
                Rs1000 = CashDetails.Rs1000
            }
        };
        
        var jsonRequest = JsonSerializer.Serialize(safeDropRequestDto);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        await BaseService.PostAsync<Derived<object>>("safe-drop/safe-drop", content);
        
        CashDetails = new CashDetails();
        
        _showLandingCashierView = true;

        _showSafeDrop = false;
        
        _showLandingPage = true;
    }
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        await FetchCategories();
    }

    private async Task FetchCategories()
    {
        var parameters = new Dictionary<string, string>
        {
            { "companyId", _globalState.CompanyId.Value.ToString() },
        };
        
        var response = await BaseService.GetAsync<Derived<List<ItemResponseDto>>>("home/get-parent-records", parameters);

        if (response != null)
        {
            foreach (var item in response.Result)
            {
                item.ItemImage = $"{item.ItemImage}";
            }
            
            _items = response.Result;
        }
    }

    private async Task OnCategoryClick(int categoryId, string categoryTitle)
    {
        _showMainCategories = false;
        _selectedCategoryId = categoryId;
        await FetchSubcategories(_selectedCategoryId);
        _title = categoryTitle;
    }

    private void OnGoBackClick()
    {
        _showMainCategories = true;
        
        _showLandingCashierView = true;
        
        _showLandingPage = true;
    }

    private async Task FetchSubcategories(int categoryId)
    {
        _subcategories = new();
        
        var result = await BaseService.GetAsync<Derived<List<ItemResponseDto>>>($"home/get-child-records/{categoryId}");

        foreach (var item in result.Result)
        {
            item.ItemImage = item.HasSubCategories ? 
                $"{item.ItemImage}" : 
                $"{item.ItemImage}";
            
            _subcategories.Add(new CategoryProductResponseDto()
            {
                Item = item
            });
        }

        foreach (var subcategory in _subcategories)
        {
            var nestedSubcategories = await FetchNestedSubcategories(subcategory.Item.ItemId);
            subcategory.SubCategories = nestedSubcategories;
        }
    }

    private async Task<IEnumerable<CategoryProductResponseDto>> FetchNestedSubcategories(int categoryId)
    {
        var response = await BaseService.GetAsync<Derived<List<CategoryProductResponseDto>>>($"home/get-child-records/{categoryId}");

        return response.Result;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                // Pass the current component instance to JavaScript
                var dotNetRef = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("initializeBarcodeScanner", dotNetRef);
            }
        }catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        
    }

    private string lastScannedBarcode = "";

    [JSInvokable]
    public async Task ReceiveBarcode(string barcode)
    {
        try
        {
            lastScannedBarcode = barcode;
            var parameters = new Dictionary<string, string>
        {
            { "barcode", lastScannedBarcode.ToString() }
        };

            var response = await BaseService.GetAsync<Derived<List<ProductScannedResponse>>>("productmanagement/get-product-by-barcode", parameters);
            var result = response?.Result.FirstOrDefault();

            if (result == null)
            {
                // Handle case when product is not found
                return;
            }

            var salesPrice = result.SalesPrice;

            var newRecord = new AddProductRequestDto()
            {
                Id = result.Id,
                Title = result.Title,
                Quantity = 1,
                UnitPrice = salesPrice,
                TotalPrice = salesPrice * 1,
            };

            if (_itemsToAdd.Any(x => x.Id == newRecord.Id))
            {
                var item = _itemsToAdd.FirstOrDefault(x => x.Id == newRecord.Id);

                if (item == null) return;

                item.Quantity += 1;
                item.TotalPrice = item.Quantity * item.UnitPrice;

                _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);
                _taxAmount = (decimal)0.13 * _subTotalAmount;
                _totalAmount = _subTotalAmount + _taxAmount;
            }
            else
            {
                _itemsToAdd = _itemsToAdd.Append(newRecord).ToList();

                _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);
                _taxAmount = (decimal)0.13 * _subTotalAmount;
                _totalAmount = _subTotalAmount + _taxAmount;
            }

            // Ensure the UI updates to reflect changes
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    private async Task AddProductToTable(int id, string title)
    {
        var parameters = new Dictionary<string, string>
        {
            { "productId", id.ToString() },
             { "producttitle", title.ToString() },


        };

        var result = (await BaseService.GetAsync<Derived<ProductResponseDto>>("product", parameters))?.Result;

        var salesPrice = result!.SalesPrice ?? 1;
        
        var newRecord = new AddProductRequestDto()
        {
            Id = id,
            Title = title,
            Quantity = 1,
            UnitPrice = salesPrice,
            TotalPrice = salesPrice * 1,
        };

        if(_itemsToAdd.Any(x => x.Id == id))
        {
            var item = _itemsToAdd.FirstOrDefault(x => x.Id == id);
            
            if (item == null) return;
            
            item.Quantity += 1;
            
            item.TotalPrice = item.Quantity * item.UnitPrice;
            
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _taxAmount = (decimal)0.13 * _subTotalAmount;

            _totalAmount = _subTotalAmount + _taxAmount;
        }
        else
        {
            _itemsToAdd = _itemsToAdd.Append(newRecord).ToList();   
            
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _taxAmount = (decimal)0.13 * _subTotalAmount;

            _totalAmount = _subTotalAmount + _taxAmount;
        }
    }

    private async Task SetSelectedProduct(int id, string title, decimal quantity, decimal discount, decimal? price = null)
    {
        var salesPrice = price ?? 1;
        
        _selectedProduct = new AddProductRequestDto()
        {
            Id = id,
            Quantity = quantity,
            Title = title,
            UnitPrice = salesPrice,
            Discount = discount,
            TotalPrice = salesPrice * quantity - discount
        };
    }
    
    private void DeselectedProduct()
    {
        _selectedProduct = null;
    }

    private void IncrementQuantity()
    {
        if (_selectedProduct == null) return;

        var selectedProduct = _itemsToAdd.FirstOrDefault(x => x.Id == _selectedProduct.Id);

        if (selectedProduct == null) return;
        
        var itemCount = _selectedProduct.Quantity;

        selectedProduct.Quantity = _selectedProduct.Quantity = itemCount == 0 ? 1 : itemCount + 1;

        selectedProduct.TotalPrice = _selectedProduct.TotalPrice = _selectedProduct.Quantity * _selectedProduct.UnitPrice;
        
        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

        _taxAmount = (decimal)0.13 * _subTotalAmount;

        _totalAmount = _subTotalAmount + _taxAmount;
    }

    private void DecrementQuantity()
    {
        if (_selectedProduct == null) return;

        var selectedProduct = _itemsToAdd.FirstOrDefault(x => x.Id == _selectedProduct.Id);

        if (selectedProduct == null) return;
        
        var itemCount = _selectedProduct.Quantity;

        selectedProduct.Quantity = _selectedProduct.Quantity = itemCount == 0 ? 1 : itemCount - 1;

        selectedProduct.TotalPrice = _selectedProduct.TotalPrice = _selectedProduct.Quantity * _selectedProduct.UnitPrice;

        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

        _taxAmount = (decimal)0.13 * _subTotalAmount;

        _totalAmount = _subTotalAmount + _taxAmount;
        if(itemCount == 1)
        {
            decimal itemdiscount = _itemDiscount - _selectedProduct.Discount;

            _itemDiscount = itemdiscount;
            decimal totaldiscount = _totalDiscount - _selectedProduct.Discount;
            _totalDiscount = totaldiscount;
            DeselectedProduct();
        }
        
    }

    private void UpdateCalculatorAmount(string value)
    {
        if (_showLandingPage)
        {
            _calculatorAmount += value;
        }
        else
        {
            _tenderAmount += value;
        }
    }

    private void AddPointValue()
    {
        if (_showLandingPage)
        {
            _calculatorAmount += ".";
        }
        else
        {
            _tenderAmount += ".";
        }
    }
    private void AddDoubleZeroValue()
    {
        if (_showLandingPage)
        {
            _calculatorAmount += "00";
        }
        else
        {
            _tenderAmount += "00";
        }
    }

    private void ClearCalculatorAmount()
    {
        if (_showLandingPage)
        {
            _calculatorAmount = string.Empty;
        }
        else
        {
            _tenderAmount = string.Empty;
        }
    }

    private async Task CalculateTotalAmount()
    {
        if (_isDiscountSelected && _selectedProduct != null)
        {
            var discountAmount = decimal.TryParse(_calculatorAmount, out var discount) ? discount : 0;
            
            var selectedProductItem = _itemsToAdd.FirstOrDefault(x => x.Id == _selectedProduct.Id);

            selectedProductItem.Discount = discountAmount;
            
            await SetSelectedProduct(selectedProductItem!.Id, selectedProductItem.Title, selectedProductItem.Quantity, selectedProductItem.Discount, selectedProductItem.UnitPrice);

            selectedProductItem.TotalPrice = _selectedProduct.UnitPrice * selectedProductItem.Quantity - discountAmount;
            
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _taxAmount = (decimal)0.13 * _subTotalAmount;

            _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
            
            _totalDiscount = _itemDiscount + _discount;
            
            _totalAmount = _subTotalAmount + _taxAmount - _discount;

            
            _calculatorAmount = string.Empty;

            _isDiscountSelected = false;
            
            DeselectedProduct();
            
            return;
           
        }

        if (_isDiscountSelected)
        {
            var discountAmount = decimal.TryParse(_calculatorAmount, out var discount) ? discount : 0;
            
            _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
            
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _taxAmount = (decimal)0.13 * _subTotalAmount;

            _discount = discountAmount;

            _totalDiscount = _itemDiscount + _discount;

            _totalAmount = _subTotalAmount + _taxAmount - _discount;

            _calculatorAmount = string.Empty;
            
            _isDiscountSelected = false;
            
            DeselectedProduct();
            
            return;
        }

        if (_isSaleSelected)
        {
            var genericItemAmount = decimal.TryParse(_calculatorAmount, out var discount) ? discount : 0;

            var item = new AddProductRequestDto()
            {
                Id = 4799,
                Discount = 0,
                UnitPrice = genericItemAmount,
                Quantity = 1,
                TotalPrice = genericItemAmount,
                Title = "Generic Item"
            };

            _itemsToAdd = _itemsToAdd.Append(item).ToList();

            _itemDiscount = _itemsToAdd.Sum(x => x.Discount);
            
            _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

            _taxAmount = (decimal)0.13 * _subTotalAmount;

            _totalDiscount = _itemDiscount + _discount;

            _totalAmount = _subTotalAmount + _taxAmount - _discount;

            _calculatorAmount = string.Empty;
            
            _isSaleSelected = false;

            return;
        }
        
        _isDiscountSelected = false;

        if (_selectedProduct == null) return;
            
        var selectedProduct = _itemsToAdd.FirstOrDefault(x => x.Id == _selectedProduct.Id);

        if (selectedProduct == null) return;
            
        selectedProduct.Quantity = _selectedProduct.Quantity = decimal.TryParse(_calculatorAmount, out var result) ? result : 0;

        selectedProduct.TotalPrice = _selectedProduct.TotalPrice = _selectedProduct.Quantity * _selectedProduct.UnitPrice;

        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);

        _taxAmount = (decimal)0.13 * _subTotalAmount;

        _totalAmount = _subTotalAmount + _taxAmount;
            
        _isDiscountSelected = false;
            
        _selectedProduct = null;
            
        _calculatorAmount = string.Empty;

    }
    
    private void VoidItem()
    {
        if(_selectedProduct == null) return;
        _itemsToAdd = _itemsToAdd.Where(x => x.Id != _selectedProduct.Id).ToList();
        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);
        _taxAmount = (decimal)0.13 * _subTotalAmount;
        _totalAmount = _subTotalAmount + _taxAmount;
        decimal itemdiscount = _itemDiscount - _selectedProduct.Discount;
        
        _itemDiscount = itemdiscount;
        decimal totaldiscount = _totalDiscount - _selectedProduct.Discount;
        _totalDiscount = totaldiscount;
        DeselectedProduct();
      }

    private void VoidTransaction()
    {
        _selectedProduct = null;
        _isDiscountSelected = false;
        _itemsToAdd = Array.Empty<AddProductRequestDto>();
        _discount = 0;
        _totalDiscount = 0;
        _itemDiscount = 0;
        _subTotalAmount = _itemsToAdd.Sum(x => x.TotalPrice);
        _taxAmount = (decimal)0.13 * _subTotalAmount;
        _totalAmount = _subTotalAmount + _taxAmount;
        _isCustomerSelected = false;
        _searchCustomer = null;
        customersList = [];
        CloseCustomerDialog();
        DeselectedProduct();
        StateHasChanged();
    }
    private void SetDiscount()
    {
        _isDiscountSelected = !_isDiscountSelected;
    }

    private void SetSale()
    {
        _isSaleSelected = !_isSaleSelected;
    }



    private void Tender()
    {
        _showLandingPage = false;

        _showCustomerView = true;

        _showLandingCashierView = false;
    }

    private decimal TotalAmount = 0;

    private decimal ReturnAmount = 0;
    
    private void CalculateTenderAmount()
    {
        TotalAmount = decimal.TryParse(_tenderAmount, out var discount) ? discount : 0;
        ReturnAmount = TotalAmount - _totalAmount;
    }
    
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }

    private bool _showPrintOption { get; set; } = false;

    private async Task OpenTenderOption()
    {
        _showPrintOption = true;
        if (ReturnAmount <= 0)
        {
            _showPrintOption = false;
        }
        if (_totalAmount == TotalAmount)
        {
            _showPrintOption = true;
        }

    }
    private async  Task OpenTenderOption2()
    {
       
           _showPrintOption = true;
        

    }



    private async Task TenderSales(bool printOption)
    {
        try
        {
            var transactionOption = "";

            if (_showCardView) transactionOption = "Card";

            else if (_showCashView) transactionOption = "Cash";

            if (_showScanView) transactionOption = "QR Scan";


            var reservatinId = _reservationId;

            var createSalesDto = new
            {
                p_companyid = _globalState.CompanyId,
                p_customerid = _customer.Id,
                p_cashierid = _globalState.UserId,
                p_totalamount = _subTotalAmount,
                p_totalamountafterdiscount = _totalAmount,
                p_totaldiscountamount = _totalDiscount,
                p_taxamount = _taxAmount,
                p_transactionoption = transactionOption,
                p_reservation_id = reservatinId,
                p_productsdetail = _itemsToAdd.Select(x => new
                {
                    Id = x.Id,
                    ProductName = x.Title,
                    Quantity = x.Quantity,
                    ProductAmount = x.TotalPrice,
                    DiscountAmount = x.Discount
                })
            };


            var jsonRequest = JsonSerializer.Serialize(createSalesDto);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("sales/create-sales", content);

            _showPrintOption = false;

            _showLandingPage = true;
            _showLandingCashierView = true;

            if (printOption)
            {
                await PrintReceipt();
            }

            ResetStates();
        }catch(Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        }
        

    }

    private async Task CloseTenderOption()
    {
        _showPrintOption = false;
    }

    private void ResetStates()
    {
        _itemsToAdd = [];
        _customer = new();
        _subTotalAmount = 0;
        _totalAmount = 0;
        _taxAmount = 0;
        _itemDiscount = 0;
        _totalDiscount = 0;
        TotalAmount = 0;
        _discount = 0;
        ReturnAmount = 0;
        _calculatorAmount = string.Empty;
        _tenderAmount = string.Empty;
        DeselectedProduct();
        _showLandingCashierView = true;
    }

    public async Task<string> GenerateReceiptContent()
    {
        StringBuilder sb = new StringBuilder();

        // Header line with bold formatting
        sb.AppendLine("                        Omana                                 ");

        // Header of the bill
        sb.AppendLine("-----------------------------------------------------------------");
        sb.AppendLine($"Transaction Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        sb.AppendLine($"ID: {Guid.NewGuid()}");
        sb.AppendLine("-----------------------------------------------------------------");

        // Columns titles
        sb.AppendLine("Product                                                      Rate");
        sb.AppendLine("------------------------------------------------------------------");

        // List items
        foreach (var item in _itemsToAdd)
        {
            // Adjust product name to fit on the next line if too long
            string productName = item.Title.Length > 30 ? $"{item.Title.Substring(0, 30)}\n{item.Title.Substring(30)}" : item.Title;

            // Calculate padding for product name
            //int productNamePadding = item.Title.Length > 30 ? 30 : 60 - item.Title.Length;

            // Adjust padding for quantity and total price
            string paddedProductName = productName;
            string paddedQuantity = item.Quantity.ToString().PadRight(60 - (item.Title.Length + 3));
            string paddedTotalPrice = item.TotalPrice.ToString();

            sb.AppendLine($"{paddedProductName} x {paddedQuantity}{paddedTotalPrice}");

        }

        sb.AppendLine("-----------------------------------------------------------------");

        // Total amount
        sb.AppendLine("Total Amount:".PadRight(50) + $"{_totalAmount}");
        sb.AppendLine("-----------------------------------------------------------------");

        return sb.ToString();
    }


    private async Task PrintReceipt()
    {
        var receiptContent = await GenerateReceiptContent();
        var printDocument = new System.Drawing.Printing.PrintDocument();
        printDocument.PrintPage += (sender, e) =>
        {
            using (System.Drawing.Font printFont = new System.Drawing.Font("Arial", 12, FontStyle.Bold))
            {
                float lineHeight = printFont.GetHeight();
                float yPosition = 8;
    
                foreach (string line in receiptContent.Split('\n'))
                {
                    if (line.Trim() == "Omana")
                    {
                        e.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(10, yPosition));
                    }
                    else
                    {
                        using (System.Drawing.Font regularFont = new System.Drawing.Font("Arial", 8))
                        {
                            e.Graphics.DrawString(line, regularFont, System.Drawing.Brushes.Black, new System.Drawing.PointF(10, yPosition));
                        }
                    }
                    yPosition += lineHeight;
                }
            }
        };
    
       /* var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
        var printTask = Task.Run(() =>
        {
            printDocument.Print();
        }, cancellationTokenSource.Token);
        try
        {
            await printTask;
        }
        catch(OperationCanceledException)
        {
            Console.WriteLine("Printing operation timed out.");
        } */       
        printDocument.Print();
    }


    private void SwitchTenderView(int index)
    {
        switch (index)
        {
            case 1:
                _showCashView = false;
                _showCardView = false;
                _showScanView = false;
                _showSplitView = false;
                _showCustomerView = true;
                _showRemarksView = false;
                _searchCustomer = "";
                customersList = [];
                break;
            case 2:
                _showCashView = true;
                _showSplitView = false;
                _showCardView = false;
                _showScanView = false;
                _showCustomerView = false;
                _showRemarksView = false;
                break;
            case 3:
                _showCashView = false;
                _showSplitView = false;
                _showCardView = true;
                _showScanView = false;
                _showCustomerView = false;
                _showRemarksView = false;
                break;
            case 4:
                _showCashView = false;
                _showSplitView = false;
                _showCardView = false;
                _showScanView = true;
                _showCustomerView = false;
                _showRemarksView = false;
                break;
            case 5:
                _showCashView = false;
                _showSplitView = false;
                _showCardView = false;
                _showScanView = false;
                _showCustomerView = false;
                _showRemarksView = true;
                break;
            case 6:
                _showCashView = false;
                _showCardView = false;
                _showScanView = false;
                _showCustomerView = false;
                _showRemarksView = false;
                _showSplitView = true;
                break;
            default:
                break;
        }
    }
    
    private readonly AddCustomerRequestDto _addCustomer = new();

    private SelectCustomerRequestDto? _customer = new();
    
    private string _searchCustomer;
    
    private List<SelectCustomerRequestDto> customersList;
    
    private void ShowAddCustomer()
    {
        _showAddCustomerView = true;

        _searchCustomer = "";

        customersList = new();
    }
    
    private async Task SearchCustomer()
    {
        try
        {
            var response = await BaseService.GetAsync<Derived<List<SelectCustomerRequestDto>>>("customer/get-all-customers?search=" + _searchCustomer);

            customersList = response.Result;

            _showAddCustomerView = false;
        }catch (Exception ex)
        {
            _message = "The customer does not exist";
            throw ex;
        }
        
    }
    
    private async Task AddCustomer()
    {
        try
        {
            var jsonRequest = JsonSerializer.Serialize(_addCustomer);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

             var response = await BaseService.PostAsync<Derived<bool>>("customer/create-customer", content);

            
              
            if(response != null)
            {
                CloseCustomerDialog();
                _showCashView = true;  
            }
            


        }
        catch (Exception ex)
        {
            _message = "The contact number already exist.";
        }
        
    }
    private bool _isDiscountSelectedbypercentage = false;
    private bool _isDiscountSelectedbyprice = false;
    private void ToggleDiscountbypercentage()
    {
        _isDiscountSelectedbypercentage = !_isDiscountSelectedbypercentage;
        if (_isDiscountSelectedbypercentage)
        {
            _isDiscountSelectedbyprice = false;
        }
    }
    private void ToggleDiscountbyprice()
    {
        _isDiscountSelectedbyprice = !_isDiscountSelectedbyprice;
        if (_isDiscountSelectedbyprice)
        {
            _isDiscountSelectedbypercentage = false;
        }
    }
    private void HandleCustomerSelection(int customerId)
    {
        var selectedCustomer = customersList.FirstOrDefault(x => x.Id == customerId);
        if (selectedCustomer != null)
        {
            _customer = selectedCustomer;
            _isCustomerSelected = true;
        }
        else
        {
            _isCustomerSelected = false; // Customer not found, reset the flag
        }
        
        _showCashView = true;
    }


    #region Close Till
    private bool _showCloseTill { get; set; } = false;
    
    private decimal _drawerAmount { get; set; }
    
    private void ShowCloseTill()
    {
        _showLandingCashierView = false;
        _showCloseTill = true;
        _showSafeDrop = false; 
    }
    
    private async Task AddDrawerAmount()
    {
        var drawerAmount = new
        {
            drawer_amount = _drawerAmount
        };
        
        var jsonRequest = JsonSerializer.Serialize(drawerAmount);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        await BaseService.PostAsync<Derived<object>>("close-till/drawer-amount", content);
        
        _showLandingCashierView = true;
        
        _showCloseTill = false;
    }
    
    private async Task PrintCloseTillTransactionDetails()
    {
        var parameters = new Dictionary<string, string>
        {
            { "companyId", _globalState.CompanyId!.Value.ToString() },
            { "employeeId", _globalState.UserId.ToString() },
        };
        
        var response = await BaseService.GetAsync<Derived<List<CloseTillResponseDto>>>("close-till/get-closing-details", parameters);
        
        var transactionDetails = response.Result;
        
        ReportService.GeneratePdfReport(_jsRuntime, transactionDetails, _globalState.Name, _globalState.RoleName, _globalState.CompanyName);
      

        _showLandingCashierView = true;
        
        _showCloseTill = false;
    }

    #endregion
}