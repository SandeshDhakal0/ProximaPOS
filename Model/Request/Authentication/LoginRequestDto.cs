using System.ComponentModel.DataAnnotations;

namespace TheHighInnovation.POS.WEB.Model.Request.Authentication;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
}