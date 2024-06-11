using Microsoft.AspNetCore.Components;
using System.Text.Json;
using TheHighInnovation.POS.Model.Request.Filter;
using TheHighInnovation.POS.Model.Request.VendorManagement;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages
{
    public partial class Vendor
    {
        [CascadingParameter]
        private GlobalState GlobalState { get; set; }

        private bool _openaddvendordialogue = false;

        private VendorFilter vendorFilter = new();
        private PagerDto _pagerDto { get; set; } = new();
        private string _dialogTitle { get; set; }
        private string _dialogOkLabel { get; set; }
        private VendorRequestDto _vendorRequestDto = new();
        private List<VendorList>? _vendorList { get; set; } = new();
        private string _message { get; set; }
        private bool _isLoading { get; set; }


        protected override async Task OnInitializedAsync()
        {
            StateHasChanged();
            vendorFilter.IsActive = true;
           await LoadVendorsAsync(1);
        }
        private async Task HandleSearch()
        {
            vendorFilter.VendorName = _vendorRequestDto.CompanyName != null ? _vendorRequestDto.CompanyName : "";
            vendorFilter.PanVat = _vendorRequestDto.PanVatNo != null ? _vendorRequestDto.PanVatNo : "";
            vendorFilter.IsActive = _vendorRequestDto.IsActive;
            await LoadVendorsAsync(1);
        }


            private async Task LoadVendorsAsync(int pageNumber)
            {
                try
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "p_page_number", pageNumber.ToString() },
                        { "p_page_size", vendorFilter.PageSize.ToString() },
                        { "p_is_active", vendorFilter.IsActive.ToString() },
                        { "p_company_name", vendorFilter.VendorName },
                        { "p_pan_no", vendorFilter.PanVat },
                    };

                   var vendorList = await BaseService.GetAsync<Model.Response.Base.Derived<List<VendorList>>>("VendorManagement/get-vendors-list", parameters);

                    if (vendorList != null && vendorList.Result != null)
                    {
                     _vendorList = vendorList.Result;
                    _pagerDto = new PagerDto(vendorList.TotalCount ?? 1, pageNumber, vendorFilter.PageSize);
                    vendorFilter.IsInitialized = true;

                    _message = "Vendors loaded successfully.";
                    }
                    else
                    {
                        _message = "No vendors found.";
                    }
                }
                catch (Exception ex)
                {
                    _message = $"Failed to load vendors: {ex.Message}";
                }
                finally
                {
                    _isLoading = false;
                }
            }

        private async Task GetVendorById(int vendorId)
        {
            try
            {
                var parameters = new Dictionary<string, string>
            {
                {"vendorId", vendorId.ToString()},
            };
               
                var list = await BaseService.GetAsync<Model.Response.Base.Derived<List<VendorListId>>>("VendorManagement/get-vendor-by-id", parameters);
                if (list != null && list.Result != null)
                {
                    _vendorRequestDto = new VendorRequestDto
                    {
                        Id = vendorId,
                        CompanyName = list.Result.First().CompanyName,
                        PanVatNo = list.Result.First().Pan_VatNo,
                        ContactNo = list.Result.First().Contact_No,
                        ContactPerson = list.Result.First().ContactPerson, 
                        Address = list.Result.First().Address,
                        Email = list.Result.First().Email
                    };
                    OpenVendorDialogBoxUpdate();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }                      
        }

        public async Task DeleteVendor(int vendorId, bool status)
        {
            try
            {
                bool alert;

                if (status == true)
                {
                    alert = await SweetAlertService.Ask("Do you want to Active the Vendor?");
                }
                else
                {
                    alert = await SweetAlertService.Ask("Do you want to In-Active the Vendor?");
                }

                if (alert && vendorId > 0)
                {
                    var result = await BaseService.DeleteAsyncWithId<Model.Response.Base.Derived<bool>>("VendorManagement/inactive-vendor", vendorId, status);
                    if (result != null)
                    {
                        await SweetAlertService.Alert("Success", "Vendor status updated", "success");
                        _vendorRequestDto.IsActive = true;
                        await OnInitializedAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void openaddvendordialogue()
        {
            _openaddvendordialogue = true;
            _dialogTitle = "Add a new Vendor";
            _dialogOkLabel = "Add";
            StateHasChanged();
        }

        private void OpenVendorDialogBoxUpdate()
        {
            _openaddvendordialogue = true;
            _dialogTitle = "Update a Vendor";
            _dialogOkLabel = "Update";
            _message = null;
            StateHasChanged();
        }


        private async Task OnUpsertVendor(bool isClosed)
        {
            try
            {
                if (isClosed)
                {
                    _openaddvendordialogue = false;
                    ClearVendorForm();
                    return;
                }

                var vendor = new
                {
                    p_id = _vendorRequestDto.Id,
                    p_company_name = _vendorRequestDto.CompanyName,
                    p_pan_vat_no = _vendorRequestDto.PanVatNo,
                    p_address = _vendorRequestDto.Address,
                    p_email = _vendorRequestDto.Email,
                    p_contact_person = _vendorRequestDto.ContactPerson,
                    p_contact_no = _vendorRequestDto.ContactNo,
                    p_is_active = true
                };
                var jsonRequest = JsonSerializer.Serialize(vendor);
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                var apiEndpoint = "VendorManagement/upsert-vendor";
                var result = await BaseService.PostAsync<Model.Response.Base.Derived<object>>(apiEndpoint, content);     
                if (result.Status == "Success")
                {
                    _openaddvendordialogue = false;
                    await SweetAlertService.Alert("Success", result.Message, "success");
                    ClearVendorForm();
                    await OnInitializedAsync();
                    
                }                              
                
            }catch(Exception)
            {
                
                _openaddvendordialogue = true;
                _message = "Pan number already exist.";
                Console.WriteLine(_message.ToString());
            }
              
                                                      
        }
        private async Task HandlePaginationChange(ChangeEventArgs e)
        {
            if (e.Value == null) return;

            var pageSize = int.Parse(e.Value.ToString()!);

            vendorFilter.PageSize = pageSize;

            await OnPagination(1);
        }
        
        private async Task OnPagination(int pageNumber)
        {
            
            await LoadVendorsAsync(pageNumber);
        }

        private void HandleInput()
        {
            _message = null;
        }
        private void ClearVendorForm()
        {
            _vendorRequestDto = new();
            _message = null;
        }
    }
}