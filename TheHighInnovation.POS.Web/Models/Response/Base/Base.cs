using System.Net;

namespace TheHighInnovation.POS.Model.Response.Base;

public abstract class Base<T>
{
    public HttpStatusCode StatusCode { get; set; }

    public string Status { get; set; }

    public string Message { get; set; }

    public int? TotalCount { get; set; }
    
    public T Result { get; set; }
}
