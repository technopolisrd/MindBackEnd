namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable
public class AuthenticateRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}