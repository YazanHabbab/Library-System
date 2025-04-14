using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class BookDto
    {
        [Required, MinLength(10, ErrorMessage = "Book ISBN must be 10 letters at least")]
        public string ISBN { get; set; }

        [Required, MinLength(2, ErrorMessage = "Book title must be 2 letters at least")]
        public string Title { get; set; }

        [Required, MinLength(3, ErrorMessage = "Book author name must be 3 letters at least")]
        public string Author { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}