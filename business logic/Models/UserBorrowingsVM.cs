using data_access.Models;

namespace business_logic.Models
{
    public class UserBorrowingsVM
    {
        public List<Book>? books { get; set; }
        public List<BookBorrowing>? borrowings { get; set; }
    }
}