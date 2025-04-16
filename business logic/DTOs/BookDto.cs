using System.ComponentModel.DataAnnotations;

namespace business_logic.DTOs
{
    public class BookDto
    {
        [Required, MinLength(10, ErrorMessage = "Book ISBN must be 10 letters at least"), MaxLength(20, ErrorMessage = "Book ISBN must be 20 letters at max."), RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public string ISBN { get; set; }

        [Required, MinLength(2, ErrorMessage = "Book title must be 2 letters at least"), MaxLength(50, ErrorMessage = "Book title must be 50 letters at max.")]
        public string Title { get; set; }

        [Required, MinLength(3, ErrorMessage = "Book author name must be 3 letters at least"), MaxLength(30, ErrorMessage = "Book author name must be 30 letters at max.")]
        public string Author { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}