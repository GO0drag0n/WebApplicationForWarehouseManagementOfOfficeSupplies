using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The confirm password field is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name field is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name field is required.")]
        public string LastName { get; set; }
    }
}