namespace TheHighInnovation.POS.Web.Model.Response.Reservation;

public class HoldRoomAvailabilityDTO
{
    public int Id { get; set; }
    
    public string RoomName { get; set; }
 
    public string Description { get; set; }
    public string ArrivalDate { get; set; }
    public string DepartureDate { get; set; }
    
    public string AvailabilityStatus { get; set; }

    public bool IsSelected { get; set; }
}

public class RoomAvailability
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public string Description { get; set; }
	public string ArrivalDate { get; set; }
	public string DepartureDate { get; set; }
	public string AvailabilityStatus { get; set;}
    public bool IsSelected { get; set; }
}

public class FrontRoomDetails
{
    public string RoomName { get; set; }
    public string Description { get; set; }
    public DateTime ArrivalDate { get; set; } = DateTime.Now;
    public DateTime DepartureDate { get; set; } = DateTime.Now.AddDays(6);
    public string AvailabilityStatus { get; set; }
}