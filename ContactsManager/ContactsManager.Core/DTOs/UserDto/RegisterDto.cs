using ContactsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTOs.UserDto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Full Name")]
        [MinLength(3)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^[0-9]*$",ErrorMessage = "Phone should contains only numbers")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }
}
