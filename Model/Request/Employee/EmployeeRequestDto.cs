namespace TheHighInnovation.POS.WEB.Model.Request.Employee;

public class EmployeeRequestDto
{
    public int EmployeeId { get; set; }
    
    public string FullName { get; set; } = null!;
    
    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? CitizenshipNumber { get; set; } = null!;
   
    public string? PAN { get; set; } = null!;
    
    public string ContactNumber { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    
    public string Gender { get; set; } = null!;

    public int? OrganizationId { get; set; } = null!;

    public int? CompanyId { get; set; } = null!;

    public int? RoleId { get; set; } = null!;

    public bool? IsAdmin { get; set; } = null!;

    public bool? IsSuperAdmin { get; set; } = false;
}