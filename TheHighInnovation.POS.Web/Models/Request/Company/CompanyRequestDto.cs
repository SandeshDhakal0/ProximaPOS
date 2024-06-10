namespace TheHighInnovation.POS.Model.Request.Company;

public class CompanyRequestDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Location { get; set; }
    
    public int OrganizationId { get; set; }
}