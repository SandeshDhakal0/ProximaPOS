
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheHighInnovation.POS.Model.Response.Item;

public class CategoryProductResponseDto
{
    public ItemResponseDto Item { get; set; }
    
    public IEnumerable<CategoryProductResponseDto> SubCategories { get; set; }
}