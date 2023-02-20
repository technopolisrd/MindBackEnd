namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable
public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
