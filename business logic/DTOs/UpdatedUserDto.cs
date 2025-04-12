using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class UpdatedUserDto
    {
        [MinLength(3)]
        public string? NewName { get; set; }

        [DataType(DataType.Password), MinLength(4)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(4)]
        public string? NewPassword { get; set; }
    }
}