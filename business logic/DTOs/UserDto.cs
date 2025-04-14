using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class UserDto
    {
        [Required,MinLength(3, ErrorMessage = "User name must be 3 letters at least")]
        public string Name { get; set; }

        [Required, DataType(DataType.Password), MinLength(4, ErrorMessage = "User password must be 4 letters at least")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password"), MinLength(4, ErrorMessage = "User password must be 4 letters at least")]
        public string ConfirmPassword { get; set; }
    }
}