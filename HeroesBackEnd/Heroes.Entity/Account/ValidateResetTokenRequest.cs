namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable

public class ValidateResetTokenRequest
{
    [Required]
    public string Token { get; set; }
}