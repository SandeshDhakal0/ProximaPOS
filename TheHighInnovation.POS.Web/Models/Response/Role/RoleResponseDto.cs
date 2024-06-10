namespace TheHighInnovation.POS.Model.Response.Role;

public class RoleResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }    
    
    public string Description { get; set; }
    
    public string Seniority { get; set; }
    
    public int CompanyId { get; set; }
    
    public string CompanyName { get; set; }

    public int OrganizationId { get; set; }
    
    public string OrganizationName { get; set; }
    
    public int Type { get; set; }

    public string TypeName { get; set; }
}