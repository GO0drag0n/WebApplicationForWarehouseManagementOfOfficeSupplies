using System.ComponentModel.DataAnnotations;

public class ManageAccountViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    public bool IsCompanyOwner { get; set; }
    public bool IsCompanyWorker { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsStorageWorker { get; set; }

    public string? CompanyName { get; set; }
    public Guid CompanyId { get; set; }


    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(NewPassword),
             ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

}

