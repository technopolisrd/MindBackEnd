namespace Mind.Entity.Account;

using Mind.Entity.SecurityAccount.Enum;
using System.ComponentModel.DataAnnotations;

#nullable disable

public class UpdateRequest
{
    private string _password;
    private string _confirmPassword;
    private string _role;
    private string _email;
    private string _linkCV;
    private string _englishLevel;
    private string _technicalSkills;

    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [EnumDataType(typeof(Role))]
    public string Role
    {
        get => _role;
        set => _role = replaceEmptyWithNull(value);
    }

    [EmailAddress]
    public string Email
    {
        get => _email;
        set => _email = replaceEmptyWithNull(value);
    }

    [MaxLength(100)]
    public string LinkCV 
    { 
        get => _linkCV; 
        set => _linkCV = replaceEmptyWithNull(value);
    }

    public string EnglishLevel 
    { 
        get => _englishLevel; 
        set => _englishLevel = replaceEmptyWithNull(value);
    }

    public string TechnicalSkills 
    { 
        get => _technicalSkills; 
        set => _technicalSkills = replaceEmptyWithNull(value);
    }

    [MinLength(6)]
    public string Password
    {
        get => _password;
        set => _password = replaceEmptyWithNull(value);
    }

    [Compare("Password")]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => _confirmPassword = replaceEmptyWithNull(value);
    }

    // helpers

    private string replaceEmptyWithNull(string value)
    {
        // replace empty string with null to make field optional
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
