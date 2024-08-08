namespace TheHighInnovation.POS.Web.Model.Response.Hold;

public class HoldTableResponseDto
{
    public int Id { get; set; }

    public string TableName { get; set; }

    public string Description { get; set; }

    public bool IsOccupied { get; set; }
}
public class HoldRoomResponseDto
{
    public int Id { get; set; }

    public string RoomName { get; set; }

    public string Description { get; set; }

    public bool IsOccupied { get; set; }
}
