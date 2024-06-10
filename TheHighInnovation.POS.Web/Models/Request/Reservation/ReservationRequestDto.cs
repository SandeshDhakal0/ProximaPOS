namespace TheHighInnovation.POS.Model.Request.Reservation;

public class ReservationRequestDto
{
    public int Id { get; set; }

    public string GroupName { get; set; }
    
    public string GuestDetails { get; set; }
    
    public string BookingType { get; set; }
    
    public string Meal { get; set; }
    
    public int Rooms { get; set; }
    
    public int Adult { get; set; }
    
    public int Child { get; set; }
    
    public string RoomType { get; set; }
    
    public DateTime ArrivalDate { get; set; } = DateTime.Now;
    
    public DateTime DepartureDate { get; set; } = DateTime.Now.AddDays(1);
    
    public string ArrivalTime { get; set; }
    
    public string DepartureTime { get; set; }
    
    public string InitialStatus { get; set; }
    
    public string CurrentStatus { get; set; }
    
    public string ModeOfReservation { get; set; }
    
    public string SpecialInstruction { get; set; }
    
    public decimal InitialPayment { get; set; }
    public string BookedBy { get; set; }
    
    public int CustomerId { get; set; }
    
    public int CompanyId { get; set; }
    public string Nationality { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Contact { get; set; }
    public string Company { get; set; }

}