using System.ComponentModel.DataAnnotations;

namespace business_logic.DTOs
{
    public class UpdatedUserDto
    {
        [MinLength(3, ErrorMessage = "User new name must be 3 letters at least"), MaxLength(30, ErrorMessage = "New user name must be 30 letters at max.")]
        public string? NewName { get; set; }

        [DataType(DataType.Password), MinLength(6, ErrorMessage = "User password must be 6 letters at least"), RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase and 1 number"),
        MaxLength(30, ErrorMessage = "Password must be 30 letters at max.")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(6, ErrorMessage = "User password must be 6 letters at least"), MaxLength(30, ErrorMessage = "Password must be 30 letters at max.")]
        public string? NewPassword { get; set; }
    }
}