using System.ComponentModel.DataAnnotations;

namespace business_logic.DTOs
{
    public class UserLoginDto
    {
        [Required, MinLength(3, ErrorMessage = "User name must be 3 letters at least")]
        public string Name { get; set; }

        [Required, DataType(DataType.Password), MinLength(6, ErrorMessage = "User password must be 6 letters at least")]
        public string Password { get; set; }
    }
}