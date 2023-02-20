namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable
public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}