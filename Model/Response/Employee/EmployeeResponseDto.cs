namespace TheHighInnovation.POS.WEB.Model.Response.Employee;

public class EmployeeResponseDto
{
    public int UserId { get; set; }
    
    public int EmployeeId { get; set; }
    
    public string FullName { get; set; }
    
    public string EmailAddress { get; set; }
    
    public int CompanyId { get; set; }
    
    public string CompanyName { get; set; }
    
    public bool? IsAdmin { get; set; }
    
    public int? RoleId { get; set; } 
    
    public string? RoleName { get; set; }
}