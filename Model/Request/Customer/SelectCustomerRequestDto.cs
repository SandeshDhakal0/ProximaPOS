using System;

namespace TheHighInnovation.POS.WEB.Model.Request.Customer;

public class SelectCustomerRequestDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Contact { get; set; }
    
    public string Address { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string? FirstName { get; set; }   
    public string? LastName { get; set; }    
    public string? Contact { get; set; }
    public string? Address { get; set; }

}