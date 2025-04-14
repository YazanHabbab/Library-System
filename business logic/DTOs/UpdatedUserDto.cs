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

        [DataType(DataType.Password), MinLength(4, ErrorMessage = "User password must be 4 letters at least")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(4, ErrorMessage = "User password must be 4 letters at least")]
        public string? NewPassword { get; set; }
    }
}