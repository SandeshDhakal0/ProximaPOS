namespace TheHighInnovation.POS.WEB.Model.Response.Module;

public class ModuleResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int? ParentModuleId { get; set; }

    public string? ParentModuleName { get; set; }
    
    public int SequenceNumber { get; set; }
    
    public string URL { get; set; }
    
    public string Icon { get; set; }
    
    public int ModuleType { get; set; }
}