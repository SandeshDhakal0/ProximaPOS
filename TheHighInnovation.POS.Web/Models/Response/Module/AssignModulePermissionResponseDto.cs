namespace TheHighInnovation.POS.Web.Model.Response.Module;

public class AssignModulePermissionResponseDto
{
    public int RoleId { get; set; }
    
    public string RoleName { get; set; }
    
    public int ModuleId { get; set; }
    
    public string ModuleName { get; set; }
    
    public int ModuleType { get; set; }
    
    public string AssignedStatus { get; set; }
    
    public bool IsChecked { get; set; }
}