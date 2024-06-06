namespace TheHighInnovation.POS.WEB.Model.Request.Module;

public class ModuleRequestDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int? ParentModuleId { get; set; }
    
    public int SequenceNumber { get; set; }
    
    public string URL { get; set; }
    
    public string Icon { get; set; }
    
    public int ModuleType { get; set; }
}