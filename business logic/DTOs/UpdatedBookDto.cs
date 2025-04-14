using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class UpdatedBookDto
    {
        public string? ISBN { get; set; }
        [MinLength(2, ErrorMessage = "Book title must be 2 letters at least")]
        public string? Title { get; set; }

        [MinLength(3, ErrorMessage = "Book author name must be 3 letters at least")]
        public string? Author { get; set; }
    }
}