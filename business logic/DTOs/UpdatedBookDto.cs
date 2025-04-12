using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class UpdatedBookDto
    {
        [MinLength(2)]
        public string? Title { get; set; }

        [MinLength(3)]
        public string? Author { get; set; }
    }
}