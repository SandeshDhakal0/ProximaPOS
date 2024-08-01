namespace TheHighInnovation.POS.Web.Model.Response.Authentication;

public class LoginResponseDto
{
    public int UserId { get; set; }
    
    public int? EmployeeId { get; set; }

    public string Name { get; set; } = null!;
    
    public int? RoleId { get; set; }

    public string? RoleName { get; set; } = null!;

    public int? RoleType { get; set; }
    
    public int? CompanyId { get; set; }

    public string? CompanyName { get; set; } = null!;

    public int? OrganizationId { get; set; }

    public string? OrganizationName { get; set; } = null!;

    public bool? IsAdmin { get; set; }

    public bool? IsSuperAdmin { get; set; }

    public string Token { get; set; } = null!;

}