namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}
