using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Text.Json;
using TheHighInnovation.POS.Web.Model;
using TheHighInnovation.POS.Web.Model.Request.Customer;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Request.Reservation;
using TheHighInnovation.POS.Web.Model.Response.Reservation;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages
{
    public partial class Reservations
    {
        [CascadingParameter]
        private GlobalState _globalState { get; set; }
        private PagerDto _pagerDto { get; set; } = new();

        private FilterRequestDto Filter = new();
        private FilterReservation FilterReservation = new();
        private string CurrentStatus { get; set; }

        private bool IsReservationModalOpen = new();

        private bool Holdmodelopen { get; set; } = false;

        private bool IsBookedRoomDetails = false;

        private bool IsRoomDetails = false;
        private bool IsCalendar = false;

        private int ReservationId { get; set; } = 0;

        private ReservationRequestDto ReservationModel = new();

        private string _dialogTitle;

        private string _dialogOkLabel;

        private List<ReservationResponseDto>? ReservationList { get; set; }

        private List<string>? _roomTypes;

        private List<SelectCustomerRequestDto>? customersList;

        private List<HoldRoomAvailabilityDTO>? roomsList;
        private List<RoomAvailability>? roomDetails;
        private FrontRoomDetails frontRoomDetails = new();
        public Customer? Customers;
        private List<string> selectedRoomTypes = new List<string>();

        private readonly AddCustomerRequestDto _addCustomer = new();
        private string _message = "";
        private TimeOnly _arrivalTime = new TimeOnly(14, 0); // 2:00 PM
        private TimeOnly _departureTime = new TimeOnly(12, 0); // 12:00 PM
        private string _contactNumber = null;
        private int customerId;
        private string _firstName = "";
        private string _lastName = "";
        private string arrivalDateS = "";
        private string departureDateS = "";

        protected override async Task OnInitializedAsync()
        {
           
            var roomTypes = await BaseService.GetAsync<Derived<List<RoomTypeResponseDto>>>("Reservation/room-type");

            _roomTypes = roomTypes!.Result.Select(x => x.Name).ToList();
            await UpdateFilter("Booked");
        }
       
        private async Task UpdateFilter(string filter)
        {           
          
                Filter.PageSize = 5;
                CurrentStatus = filter;
            string arrivalDateString = FilterReservation.ArrivalDate.ToString("yyyy-MM-dd");
            string departureDateString = FilterReservation.DepartureDate.ToString("yyyy-MM-dd");

            arrivalDateS = arrivalDateString;
            departureDateS = departureDateString;
            await FetchReservationList(1);
                         
        }
       
        private async Task SearchButton(string search_customer, DateTime arrival_date, DateTime departure_date)
        {
            Filter.PageSize = 5;
           
            FilterReservation.SearchCustomer = search_customer;

            string arrivalDateString = arrival_date.ToString("yyyy-MM-dd");
            string departureDateString = departure_date.ToString("yyyy-MM-dd");

            arrivalDateS = arrivalDateString;
            departureDateS = departureDateString;

            await FetchReservationList(1);
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
            await FetchReservationList(pageNumber);
        }

        private async Task FetchReservationList(int pageNumber)
        {
            var parameters = new Dictionary<string, string>
        {
            { "page_no", pageNumber.ToString() },
            { "page_size", Filter.PageSize.ToString() },
            {"current_status", CurrentStatus},
            {"customer_search", FilterReservation.SearchCustomer },
            {"arrival_date", arrivalDateS },
            {"departure_date", departureDateS}
        };

            var reservations = await BaseService.GetAsync<Derived<List<ReservationResponseDto>>>("Reservation/reservation-active-list", parameters);

            if (reservations != null && reservations.Result != null)
            {
                ReservationList = reservations.Result;
                _pagerDto = new PagerDto(reservations.TotalCount ?? 1, pageNumber, Filter.PageSize);
                Filter.IsInitialized = true;
            }
        }

        private int NumberOfNights
        {
            get
            {
                int departuredays = ReservationModel.DepartureDate.DayOfYear;
                int arrivaldays = ReservationModel.ArrivalDate.DayOfYear;
                return departuredays - arrivaldays;
              /*  if (ReservationModel.DepartureDate > ReservationModel.ArrivalDate)
                {
                    return (ReservationModel.DepartureDate - ReservationModel.ArrivalDate).Days ;
                }*/
                /*return 0; 
*/            }
        }

        private Task OnArrivalDateChaged(ChangeEventArgs e)
        {
            ReservationModel.ArrivalDate = DateTime.Parse(e.Value.ToString());
            UpdateNights();
            return Task.CompletedTask;
        }

        private Task OnDepartureDateChanged(ChangeEventArgs e)
        {
            ReservationModel.DepartureDate = DateTime.Parse(e.Value.ToString());
            UpdateNights();
            return Task.CompletedTask;
        }
        private void UpdateNights()
        {
            StateHasChanged(); 
        }

        private async Task OpenReservationModal(string bookingType)
        {
            _dialogTitle = "Add";

            _dialogOkLabel = "Add";

            IsReservationModalOpen = true;

            ReservationModel.BookingType = bookingType;
        }

        #region RoomType Selection
        private bool IsRoomTypeSelected(string roomType)
        {
            return selectedRoomTypes.Contains(roomType);
        }

        private void HandleRoomType(string roomType)
        {
            if (selectedRoomTypes.Contains(roomType))
            {
                selectedRoomTypes.Remove(roomType);
            }
            else
            {
                selectedRoomTypes.Add(roomType);
            }
            ReservationModel.RoomType = string.Join(',', selectedRoomTypes);
        }

        #endregion

      
        private async Task GetReservationById(int id)
        {
            var parameters = new Dictionary<string, string>
        {
            { "id", id.ToString() }
        };

            _dialogTitle = "Update";

            _dialogOkLabel = "Update";

            var reservation = await BaseService.GetAsync<Derived<BookingResponseDto>>("Reservation/reservation-active-list-by-id", parameters);

            if (reservation != null)
            {
                var arrivalDate = DateTime.ParseExact(reservation.Result.ArrivalDate + " " + reservation.Result.ArrivalTime, "yyyy-MM-dd h:mm tt", null);
                var departureDate = DateTime.ParseExact(reservation.Result.DepartureDate + " " + reservation.Result.DepartureTime, "yyyy-MM-dd h:mm tt", null);

                ReservationModel = new ReservationRequestDto
                {
                    Id = id,
                    FirstName = reservation.Result.FirstName,
                    LastName = reservation.Result.LastName,
                    Address = reservation.Result.Address,
                    Contact = reservation.Result.Contact,
                    Company = reservation.Result.Company,
                    CustomerId = reservation.Result.CustomerId,
                    ArrivalDate = arrivalDate,
                    DepartureDate = departureDate,
                    RoomType = reservation.Result.RoomType,
                    SpecialInstruction = reservation.Result.SpecialInstruction,
                    Rooms = reservation.Result.Rooms,
                    Adult = reservation.Result.Adult,
                    Child = reservation.Result.Child,
                    Meal = reservation.Result.Meal,
                    ModeOfReservation = reservation.Result.ModeOfReservation,
                    InitialPayment = reservation.Result.InitialPayment,
                    CurrentStatus = reservation.Result.CurrentStatus,   
                    BookedBy = reservation.Result.BookedBy,

                };
                _firstName = ReservationModel.FirstName;
                _lastName = ReservationModel.LastName;
                _addCustomer.p_address = ReservationModel.Address;
             //   _roomTypes = ReservationModel.RoomType;
                ReservationModel.Nationality = ReservationModel.Nationality;
                _addCustomer.p_company = ReservationModel.Company;
                _contactNumber = ReservationModel.Contact;  
            } 

            IsReservationModalOpen = true;
        }

        private async Task OnUpsertReservation(bool isClosed)
        {
            if (isClosed)
            {
                IsReservationModalOpen = false;
                ClearForm();
                return;
            }

            int? customerId = ReservationModel.CustomerId == 0 ? null : ReservationModel.CustomerId;

            // Create a customer only if customerId is not already set.
            if (!customerId.HasValue)
            {
                var customerparam = new AddCustomerRequestDto
                {
                    p_first_name = _firstName,
                    p_last_name = _lastName,
                    p_address = _addCustomer.p_address,
                    p_contact = _contactNumber,
                    p_company = _addCustomer.p_company,
                };
                var jsonRequestCustomer = JsonSerializer.Serialize(customerparam);
                var contentCustomer = new StringContent(jsonRequestCustomer, System.Text.Encoding.UTF8, "application/json");

                var result = await BaseService.PostAsync<Derived<int>>("customer/create-customer", contentCustomer);

                if (result.Result != null)
                {
                    customerId = result.Result; // Update customerId with the new value.
                }
                else
                {                    
                    return;
                }
            }
            if(ReservationModel.Id == 0)
            {
                ReservationModel.CurrentStatus = "Booked";                
            }

            // Prepare and serialize the reservation data.
            var reservation = new
            {
                p_id = ReservationModel.Id,
                p_group_name = ReservationModel.GroupName,
                p_guest_details = $"{_firstName} {_lastName}",
                p_booking_type = ReservationModel.BookingType,
                p_meal = ReservationModel.Meal,
                p_rooms = ReservationModel.Rooms,
                p_adult = ReservationModel.Adult,
                p_child = ReservationModel.Child,
                p_room_type = ReservationModel.RoomType,
                p_arrival_date = ReservationModel.ArrivalDate.ToString("yyyy-MM-dd"),
                p_departure_date = ReservationModel.DepartureDate.ToString("yyyy-MM-dd"),
                p_arrival_time = _arrivalTime.ToString("h:mm tt"),
                p_departure_time = _departureTime.ToString("h:mm tt"),
                p_initial_status = ReservationModel.InitialStatus,
                p_current_status = ReservationModel.CurrentStatus,
                p_mode_of_reservation = ReservationModel.ModeOfReservation,
                p_special_instruction = ReservationModel.SpecialInstruction,
                p_initial_payment = ReservationModel.InitialPayment,
                p_customer_id = customerId.Value,
                p_nationality = ReservationModel.Nationality,
                p_booked_by = ReservationModel.BookedBy
            };

            var jsonRequest = JsonSerializer.Serialize(reservation);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var apiEndPoint = ReservationModel.Id == 0 ? "Reservation/create-reservation" : "Reservation/update-reservation";

            // Send reservation data to server.
            await BaseService.PostAsync<Derived<object>>(apiEndPoint, content);

            // Reinitialize components.
            await OnInitializedAsync();
            IsReservationModalOpen = false;
            ClearForm();
        }


     /*   private async Task AddCustomer()
        {
            var jsonRequest = JsonSerializer.Serialize(_addCustomer);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var result = await BaseService.PostAsync<Derived<int>>("customer/create-customer", content);

            customerId = result.Result;
        }*/

        private void ClearForm()
        {
            ReservationModel = new();
            selectedRoomTypes = new();
            Customers = null;
            _contactNumber = "";
            _firstName = "";
            _lastName = "";
            _addCustomer.p_address = "";                        
        }

       
        private async Task CheckInCustomer(int reservationId)
        {
            try
            {
                var reservation = new
                {
                    p_id = reservationId
                };

                var jsonRequest = JsonSerializer.Serialize(reservation);

                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                var apiEndPoint = "Reservation/checkin-customer";

                var checkin = await BaseService.PostAsync<Derived<object>>(apiEndPoint, content);

                await OnInitializedAsync();

            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("showAlert", "Error", "Please select the rooms first.","error");
            }
        }

        private void CustomerModalActionButton()
        {
            IsBookedRoomDetails = false;
            ClearForm();
        }

        private void RoomsCancelActionButton()
        {
            IsRoomDetails = false;
        }

        private async Task RoomDetailsModal()
        {
            IsRoomDetails = true;
           await GetAllRoomsDetails();
        }

        private void GoToCalendar()
        {
            IsCalendar = true;
        }

        private void RoomsModalActionButton()
        {
            IsBookedRoomDetails = false;
        }

        
        private async Task SearchRoomDate()
        {           
            await GetAllRoomsDetails();

        }
        private async Task GetAllRoomsDetails()
        {
            try
            {
                var arrivalDate = frontRoomDetails.ArrivalDate.ToString("yyyy-MM-dd");
                var departureDate = frontRoomDetails.DepartureDate.ToString("yyyy-MM-dd");

                var parameter = new Dictionary<string, string>
                {
                    {"from", arrivalDate },
                    {"to", departureDate }
                };
                var reservations = await BaseService.GetAsync<Derived<List<HoldRoomAvailabilityDTO>>>("Reservation/get-all-rooms-reservation", parameter);

                roomsList = reservations.Result;
                                          
            }
            catch (Exception)
            {
                _message = "Something is wrong.";
            }

        }

        private async Task GetRoomDetails(int reservationId)
        {
            ReservationId = reservationId;

            var parameters = new Dictionary<string, string>
            {
                { "reservationId", reservationId.ToString() },               
            };

            var reservations = await BaseService.GetAsync<Derived<List<HoldRoomAvailabilityDTO>>>("Reservation/get-reservation-room-details", parameters);

            roomsList = reservations!.Result;

            foreach (var rooms in roomsList)
            {
                if (rooms.AvailabilityStatus == "Selected") rooms.IsSelected = true;
            }

            IsBookedRoomDetails = true;
        }

        #region CancelReservation
        private async Task CancelReservation(int reservationId)
        {
            try
            {
                // Display SweetAlert confirmation dialog
                var result = await _jsRuntime.InvokeAsync<object>("Swal.fire", new
                {
                    title = "Are you sure?",
                    text = "Are you sure you want to cancel this reservation?",
                    icon = "warning",
                    showCancelButton = true,
                    confirmButtonText = "Yes",
                    cancelButtonText = "No"
                }).ConfigureAwait(false);

                // Parse the result to determine user's choice (check `isConfirmed` property)
                var resultAsJson = JsonSerializer.Serialize(result);
                var confirmed = resultAsJson.Contains("\"isConfirmed\":true");

                if (confirmed)
                {
                    var parameter = new
                    {
                        id = reservationId,
                    };
                    var jsonRequest = JsonSerializer.Serialize(parameter);

                    var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                    var apiEndPoint = "Reservation/cancel-reservation";

                    var response = await BaseService.PostAsync<Derived<object>>(apiEndPoint, content);
                   
                    await _jsRuntime.InvokeVoidAsync("showAlert", "Success", "Reservation Cancelled.", "success");
                    await OnInitializedAsync();


                }
                else
                {
                    // User clicked "no", do nothing
                }
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("showAlert", "Error", "Something went wrong.", "error");
            }
        }

        #endregion


        private async Task SubmitReservationRooms()
        {

            var selectedRooms = roomsList?.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (selectedRooms.Count == 0) return;
            var roomDetails = new
            {
                ReservationId = ReservationId,
                CompanyId = _globalState.CompanyId,
                CashierId = 0,
                Rooms = selectedRooms
            };

            var jsonRequest = JsonSerializer.Serialize(roomDetails);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("Reservation/assign-rooms", content);

            CustomerModalActionButton();
        }

      /*  private async Task HandleCustomerChange(ChangeEventArgs e)
        {
            var selectedCustomer = e.Value?.ToString();

            var result = int.TryParse(selectedCustomer, out var customerId);

            if (result)
            {
                ReservationModel.CustomerId = customerId;
            }
        }*/


        #region CustomerSearch
        private Timer _debounceTimer;
        private string selectedId = "";
        private bool showDropdown = false;
        private string _selectedContactNumber = "";

        private void OnContactNumberChanged(ChangeEventArgs e)
        {
            _contactNumber = e.Value.ToString();
           
                Customers = null; 
                showDropdown = !string.IsNullOrEmpty(_contactNumber);
                ResetTimerForSearch();
            
           
        }
        private void ResetTimerForSearch()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(DebouncedSearch, null, 500, Timeout.Infinite);
        }


        private void DebouncedSearch(object state)
        {
            InvokeAsync(async () =>
            {
                await SearchCustomer();
               /* showDropdown = Customers != null;  // Only show dropdown if there are search results*/
                StateHasChanged();
            });
        }

        private async Task SearchCustomer()
        {
            if (!string.IsNullOrEmpty(_contactNumber))
            {
                var parameters = new Dictionary<string, string>
            {
                {"contact_id", _contactNumber}
            };

                var customerData = await BaseService.GetAsync<Derived<Customer>>("customer/get-customer-by-contact", parameters);
                Customers = customerData?.Result;  
            }
            if (string.IsNullOrEmpty(_contactNumber))
            {
                ClearCustomerSearch();
            }
        }

        private void OnCustomerSelected(ChangeEventArgs e)
        {
            var selectedId = e.Value.ToString();
            if (!string.IsNullOrEmpty(selectedId))
            {
                if (Customers != null )
                {
                    _firstName = Customers.FirstName;
                    _lastName = Customers.LastName;
                    ReservationModel.CustomerId = Customers.Id;
                    _contactNumber = Customers.Contact;
                    _addCustomer.p_address = Customers.Address;
                    StateHasChanged() ; 
                }
                else
                {
                    ClearCustomerSearch();
                }
            }
        }

        private void ClearCustomerSearch()
        {
            _firstName = "";
            _lastName = "";
            ReservationModel.CustomerId = 0;
            _contactNumber = "";
            _addCustomer.p_address = "";
            StateHasChanged();
        }

        #endregion


    }
}