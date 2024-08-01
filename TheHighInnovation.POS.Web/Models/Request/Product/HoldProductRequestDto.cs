using System;
using System.Collections.Generic;

namespace TheHighInnovation.POS.Web.Model.Request.Product;

public class HoldProductRequestDto
{
    public int Id { get; set; }
    
    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime AddedDateTime { get; set; }
    
    public List<AddProductRequestDto> ProductsOnHold { get; set; } = new List<AddProductRequestDto>();
}