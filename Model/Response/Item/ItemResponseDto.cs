namespace TheHighInnovation.POS.WEB.Model.Response.Item;

public class ItemResponseDto
{
    public int ItemId { get; set; }

    public string ItemTitle { get; set; }

    public string ItemType { get; set; }

    public string ItemImage { get; set; }
    
    public bool HasSubCategories { get; set; }
}