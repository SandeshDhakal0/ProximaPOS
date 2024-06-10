namespace TheHighInnovation.POS.Model.Request.Role;

public class RoleRequestDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }    
    
    public string Description { get; set; }
    
    public string Seniority { get; set; }
    
    public int CompanyId { get; set; }
    
    public int Type { get; set; }
}