namespace data_access.Models
{
    public class BookBorrowing
    {
        public int BorrowingId { get; set; }
        public string ISBN { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}