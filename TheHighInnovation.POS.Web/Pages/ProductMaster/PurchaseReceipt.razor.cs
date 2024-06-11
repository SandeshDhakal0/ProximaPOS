using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TheHighInnovation.POS.Model.Request.PurchaseReceipt;
using TheHighInnovation.POS.Model.Request.VendorManagement;
using TheHighInnovation.POS.Model.Request.VendorManagement.ProductsDTO;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages.ProductMaster
{
    public partial class PurchaseReceipt
    {
        [CascadingParameter]
        private GlobalState GlobalState { get; set; }
        // private List<VendorListShort>? _vendorList;
        private VendorListShort selectedVendor;
        private ProductListShort selectedProduct;
        private List<CategoryDropDownList> categoryList;
        private ProductFormList productFormList = new();
        private List<ProductStoreDTO> productStore = new List<ProductStoreDTO>();
        private IEnumerable<UnitDTO> _unit;
       // private VendorDetails vendorDetails = new();
        private PurchaseReceiptDetails purchaseReceipt = new();
        private ProductList productList;
        private List<ToastMessage> messages;
        private string inputValue;

        bool agree1 = true;
        bool agree2 = true;

        protected override async Task OnInitializedAsync()
        {

            productFormList = new();
            selectedVendor = null;
            productStore = new List<ProductStoreDTO>();
            AddProductRow();
            messages = SweetAlertService.GetMessages();

        }
        public class SelectedDates
        {
            public string? SelectedDateIssueBs { get; set; } = null;
            public string? SelectedDueDateBs { get; set; } = null ;
            public int YearBs {  get; set; }
            public int MonthBs { get; set; }    

        }
        public class DatePickerData
        {
            public int BsYear { get; set; }
            public int BsMonth { get; set; }
            public int BsDate { get; set; }
            public int WeekDay { get; set; }
            public string FormattedDate { get; set; }
            public DateTime AdDate { get; set; }
            public DateTime BsMonthFirstAdDate { get; set; }
            public int BsMonthDays { get; set; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    await JS.InvokeVoidAsync("leapfrogdatepicker");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private static SelectedDates selectedDates = new SelectedDates();

        [JSInvokable("ReceiveDatePickerData")]
        public static async Task ReceiveDatePickerData(Dictionary<string, object> inputData)
        {
            // Extract the dateType from the input data
            if (inputData.TryGetValue("dateType", out var dateTypeObj) && dateTypeObj is JsonElement dateTypeElement)
            {
                var dateType = dateTypeElement.GetString();
                Console.WriteLine($"Received date type: {dateType}");

                // Extract the datePickerData from the input data
                if (inputData.TryGetValue("datePickerData", out var datePickerDataObj) && datePickerDataObj is JsonElement datePickerDataElement)
                {
                    var jsonData = datePickerDataElement.GetRawText();
                    Console.WriteLine($"Received raw date data: {jsonData}");

                    // Deserialize the JSON to the DatePickerData class
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                    };
                    var dateData = JsonSerializer.Deserialize<DatePickerData>(jsonData, options);

                    if (dateData != null)
                    {
                        // Handle the data based on the date type
                        if (dateType == "issueDate")
                        {
                            selectedDates.SelectedDateIssueBs = dateData.FormattedDate ?? "";
                            selectedDates.YearBs = dateData.BsYear;
                            selectedDates.MonthBs = dateData.BsMonth;
                            Console.WriteLine($"Received issue date data: BsYear={dateData.BsYear}, BsMonth={dateData.BsMonth}, BsDate={dateData.BsDate}, " +
                                              $"WeekDay={dateData.WeekDay}, FormattedDate={dateData.FormattedDate}, AdDate={dateData.AdDate}, " +
                                              $"BsMonthFirstAdDate={dateData.BsMonthFirstAdDate}, BsMonthDays={dateData.BsMonthDays}");
                        }
                        else if (dateType == "dueDate")
                        {
                            selectedDates.SelectedDueDateBs = dateData.FormattedDate ?? "";
                            Console.WriteLine($"Received due date data: BsYear={dateData.BsYear}, BsMonth={dateData.BsMonth}, BsDate={dateData.BsDate}, " +
                                              $"WeekDay={dateData.WeekDay}, FormattedDate={dateData.FormattedDate}, AdDate={dateData.AdDate}, " +
                                              $"BsMonthFirstAdDate={dateData.BsMonthFirstAdDate}, BsMonthDays={dateData.BsMonthDays}");
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }
    


    private async Task<IEnumerable<VendorListShort>> FetchVendors(string searchText)
       {
            var parameters = new Dictionary<string, string>
        {
            { "vendorname", searchText },
        };

            try
            {
                var vendors = await BaseService.GetAsync<Model.Response.Base.Derived<IEnumerable<VendorListShort>>>("VendorManagement/get-vendor-for-search", parameters);
                return vendors.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vendors: {ex.Message}");
                return Enumerable.Empty<VendorListShort>();
            }
        }

        private async Task<IEnumerable<VendorListShort>> FetchVendorsPanVat(string searchText)
        {
            var parameters = new Dictionary<string, string>
        {
            { "panvatno", searchText },
        };

            try
            {
                var vendors = await BaseService.GetAsync<Model.Response.Base.Derived<IEnumerable<VendorListShort>>>("VendorManagement/get-vendor-for-search-pan-vat", parameters);
                return vendors.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vendors: {ex.Message}");
                return Enumerable.Empty<VendorListShort>();
            }
        }
             


        private void SaveValue(VendorListShort vendorListShort)
        {
            selectedVendor = vendorListShort;
        }
                
        private string Message = string.Empty;
        private string Type = string.Empty;      
        private async Task ClearMessage()
        {
            await Task.Delay(2000);
            Message = string.Empty;
            StateHasChanged();
        }

        private DateTime? issueDate;
        private DateTime? dueDate;
        
        private async Task SubmitInventory()
        {
            try
            {
                // Format the date
                string issuedate = issueDate?.ToString("yyyy-MM-dd");
                string duedate = dueDate?.ToString("yyyy-MM-dd");
                int monthAd = issueDate.HasValue ? int.Parse(issueDate.Value.ToString("MM")) : 0;
                int yearAd = issueDate.HasValue ? int.Parse(issueDate.Value.ToString("yyyy")) : 0;
                // Prepare the vendor details
                var vendor = new PurchaseReceiptDetails
                {
                    p_vendor_name = selectedVendor?.CompanyName,
                    p_vendor_id = selectedVendor?.Id ?? 0,
                    p_pan_vat = selectedVendor?.PanVat,
                    p_issue_date_ad = issuedate,
                    p_issue_date_bs = selectedDates.SelectedDateIssueBs,
                    p_branch = purchaseReceipt?.p_branch,
                    p_invoice_no = purchaseReceipt?.p_invoice_no,
                    p_due_date_ad = duedate,
                    p_due_date_bs = selectedDates.SelectedDueDateBs,
                    p_receipt_type = purchaseReceipt?.p_receipt_type,
                    p_bill_no = purchaseReceipt?.p_bill_no,
                    p_mop = purchaseReceipt?.p_mop,
                    p_remarks = purchaseReceipt?.p_remarks,
                    p_bill_prepared_by = purchaseReceipt?.p_bill_prepared_by,
                    p_amount_in_words = purchaseReceipt?.p_amount_in_words,
                    p_gross_amount = purchaseReceipt?.p_gross_amount ?? 0,
                    p_product_discount = purchaseReceipt?.p_product_discount ?? 0,
                    p_taxable_amount = purchaseReceipt?.p_taxable_amount ?? 0,
                    p_discount = purchaseReceipt?.p_discount ?? 0,
                    p_vat = purchaseReceipt?.p_vat ?? 0,
                    p_rounded_off = purchaseReceipt?.p_rounded_off ?? 0,
                    p_net_amount = purchaseReceipt?.p_net_amount ?? 0,
                    p_paid_amount = purchaseReceipt?.p_paid_amount ?? 0,
                    p_remaining_amount = purchaseReceipt?.p_remaining_amount ?? 0,
                    p_month_bs = selectedDates.MonthBs,
                    p_year_bs = selectedDates.YearBs,
                    p_month_ad = monthAd,
                    p_year_ad = yearAd
                };

                if (vendor != null && productStore != null && Products != null && Products.Any())
                {
                    // Serialize and send the vendor details
                    var jsonRequestVendor = JsonSerializer.Serialize(vendor);
                    var contentVendor = new StringContent(jsonRequestVendor, System.Text.Encoding.UTF8, "application/json");
                    var apiEndpointVendor = "PurchaseReceipt/create-purchase-receipt";
                    var resultVendor = await BaseService.PostAsync<Model.Response.Base.Derived<int>>(apiEndpointVendor, contentVendor);

                    if (resultVendor != null)
                    {
                        var productList = new List<ProductForApi>();

                        foreach (var product in Products)
                        {
                            if (product?.SelectedProduct == null)
                            {
                                Console.WriteLine("SelectedProduct is null for one of the products.");
                                continue; 
                            }

                            productList.Add(new ProductForApi
                            {
                                p_product_name = product.SelectedProduct.ProductName,
                                p_qty = product.Qty,
                                p_unit = product.Unit,
                                p_disc = product.DiscAmount ?? 0,
                                p_rate = product.Rate,
                                p_amount = product.Amount ?? 0,
                                p_product_receipt_id = resultVendor.Result,
                                p_product_id = product.SelectedProduct.Id,
                                p_uom = string.Concat(product.Uom ?? "","", product.UomUnit ?? ""),
                                p_disc_percent = product.DiscPercent ?? 0
                            });
                        }

                        if (productList.Any())
                        {
                            var jsonRequestProduct = JsonSerializer.Serialize(productList);
                            var content = new StringContent(jsonRequestProduct, System.Text.Encoding.UTF8, "application/json");
                            var apiEndpoint = "PurchaseReceipt/create-purchase-product";
                            var result = await BaseService.PostAsync<Model.Response.Base.Derived<object>>(apiEndpoint, content);

                            if (result != null && resultVendor != null)
                            {
                                await SweetAlertService.Alert("success", result.Status, "success");
                                await ClearAfterSubmit();
                                 
                            }
                        }
                        else
                        {
                            SweetAlertService.ShowMessage(ToastType.Warning, "No valid products", "The product list is empty or contains invalid items.");
                        }
                    }
                }
                else
                {
                    SweetAlertService.ShowMessage(ToastType.Warning, "Invalid input", "Vendor or Products list is invalid.");
                }
            }
            catch (Exception ex)
            {
                SweetAlertService.ShowMessage(ToastType.Danger, "An Error has occurred!", "An error occurred during submission.");
                Console.WriteLine($"An exception occurred: {ex}");
            }
        }


        private async Task ClearAfterSubmit()
        {
            purchaseReceipt = new();
           
            DateTime? issueDate;
            DateTime? dueDate;
            new SelectedDates();
            List<ProductRow> Products = new List<ProductRow>();
            await OnInitializedAsync();
            
        }


        private List<ProductRow> Products = new List<ProductRow>();

        private void AddProductRow()
        {
            var newRow = new ProductRow
            {
                SerialNumber = Products.Count + 1
            };
            Products.Add(newRow);         
            StateHasChanged();
        }
             
        private void RemoveProductRow(ProductRow product)
        {
            Products.Remove(product);
            UpdateSerialNumbers();
        }


         private void OnAmountChanged(ChangeEventArgs e, ProductRow product)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal newValue))
            {
                product.Amount = newValue;
                UpdateAmount();
            }
        }

        private void UpdateAmount()
        {
            // Calculate gross amount and product discount
            purchaseReceipt.p_gross_amount = Products.Sum(p => p.Amount) ?? 0;
            purchaseReceipt.p_product_discount = Products.Sum(p => p.DiscAmount);

            // Calculate taxable amount
            purchaseReceipt.p_taxable_amount = purchaseReceipt.p_gross_amount - (purchaseReceipt.p_discount ?? 0 + purchaseReceipt.p_product_discount);

            // Calculate VAT (13% of taxable amount)
            purchaseReceipt.p_vat = (purchaseReceipt.p_taxable_amount * 13m / 100m) + purchaseReceipt.p_taxable_amount;

            // Calculate net amount considering rounded off value
            purchaseReceipt.p_net_amount = purchaseReceipt.p_rounded_off != 0
                                            ? purchaseReceipt.p_vat - purchaseReceipt.p_rounded_off
                                            : purchaseReceipt.p_vat;

            // Calculate remaining amount considering paid amount
            if (purchaseReceipt.p_paid_amount != 0)
            {
                purchaseReceipt.p_remaining_amount = purchaseReceipt.p_paid_amount != purchaseReceipt.p_net_amount
                                                     ? 0
                                                     : purchaseReceipt.p_net_amount - purchaseReceipt.p_paid_amount;
            }
            else
            {
                purchaseReceipt.p_remaining_amount = purchaseReceipt.p_net_amount;
            }

            // Notify state has changed
            StateHasChanged();
        }


        private void UpdateSerialNumbers()
        {
            for (int i = 0; i < Products.Count; i++)
            {
                Products[i].SerialNumber = i + 1;
            }
        }

        private void SaveProductValue(ProductRow productRow, ProductListShort selectedProduct)
        {
            try
            {
                productRow.SelectedProduct = selectedProduct;
                productRow.Rate = selectedProduct.SalesPrice;
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }                        
        }

        private async Task<IEnumerable<ProductListShort>> FetchProduct(string searchText)
        {
            var parameters = new Dictionary<string, string>
        {
            { "productname", searchText }
        };

            try
            {
                var products = await BaseService.GetAsync<Model.Response.Base.Derived<IEnumerable<ProductListShort>>>("productmanagement/get-product-name", parameters);
                return products.Result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vendors: {ex.Message}");
                return Enumerable.Empty<ProductListShort>();
            }
        }

        public class ProductRow
        {
            public int SerialNumber { get; set; }
            public ProductListShort SelectedProduct { get; set; }
            public string Uom { get; set; }       
            public string UomUnit { get; set; }
            public int Qty { get; set; }
            public string Unit { get; set; }
            public decimal? DiscPercent { get; set; } 
            public decimal? DiscAmount { get; set; }
            public decimal Rate { get; set; }
            public decimal? Amount { get; set; }
        }


    }

}
