﻿namespace Mind.Entity.Account;

using System.ComponentModel.DataAnnotations;

#nullable disable

public class RegisterRequest
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [MaxLength(100)]
    public string LinkCV { get; set; }

    [MaxLength(15)]
    public string EnglishLevel { get; set; }

    [MaxLength(250)]
    public string TechnicalSkills { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Range(typeof(bool), "true", "true")]
    public bool AcceptTerms { get; set; }
}