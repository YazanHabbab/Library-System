using data_access.Models;

namespace business_logic.Models
{
    public class BorrowingsVM
    {
        public List<User>? users { get; set; }
        public List<Book>? books { get; set; }
        public List<BookBorrowing>? borrowings { get; set; }
    }
}