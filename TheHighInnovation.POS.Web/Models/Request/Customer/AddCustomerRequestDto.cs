namespace TheHighInnovation.POS.Web.Model.Request.Customer;

public class AddCustomerRequestDto
{
    public int p_id { get; set; }
    
    public string p_first_name { get; set; } = string.Empty;

    public string p_last_name { get; set; } = string.Empty;

    public string p_contact { get; set; } = string.Empty;

    public string p_address { get; set; } = string.Empty;
    public string p_company { get; set; } = string.Empty;   
}