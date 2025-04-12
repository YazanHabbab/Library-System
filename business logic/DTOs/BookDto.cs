using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace business_logic.DTOs
{
    public class BookDto
    {
        [Required, MinLength(10)]
        public string ISBN { get; set; }

        [Required, MinLength(2)]
        public string Title { get; set; }

        [Required, MinLength(3)]
        public string Author { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}