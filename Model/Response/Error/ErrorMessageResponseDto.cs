using System.Net;

namespace TheHighInnovation.POS.WEB.Model.Response.Error;

public class ErrorMessageResponseDto
{
    public HttpStatusCode StatusCode { get; set; }
    
    public string Message { get; set; }
    
    public string ContentType { get; set; }
}