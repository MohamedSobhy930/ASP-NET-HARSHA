using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTOs.UserDto
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Email can`t be blank")]
        [EmailAddress(ErrorMessage ="Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password can`t be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
