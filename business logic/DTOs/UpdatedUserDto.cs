using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class UpdatedUserDto
    {
        [MinLength(3, ErrorMessage = "User new name must be 3 letters at least")]
        public string? NewName { get; set; }

        [DataType(DataType.Password), MinLength(6, ErrorMessage = "User password must be 6 letters at least"), RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase and 1 number")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(6, ErrorMessage = "User password must be 6 letters at least")]
        public string? NewPassword { get; set; }
    }
}