using business_logic.DTOs;
using business_logic.Models;
using data_access.Interfaces;
using data_access.Models;
using data_access.Models.Other;

namespace business_logic.Services
{
    public class LibraryService
    {
        private readonly IBookRepository _bookRepo;
        private readonly AccountService _accountService;
        public LibraryService(IBookRepository bookRepo, AccountService accountService)
        {
            _bookRepo = bookRepo;
            _accountService = accountService;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookRepo.GetAllBooks();
        }

        public async Task<List<Book>> SearchBooksByISBNOrTitleOrAuthor(string searchTerm, bool availableOnly)
        {
            List<Book> books = new();

            if (string.IsNullOrWhiteSpace(searchTerm))
                books = await GetAllBooks();

            // Filter books if search is not empty
            else if (!string.IsNullOrWhiteSpace(searchTerm))
                books = await _bookRepo.GetBooksByISBNOrTitleOrAuthor(searchTerm);

            // Get available books only
            if (availableOnly)
                books = books.Where(b => b.IsAvailable == availableOnly).ToList();

            // Order books by title
            return books.OrderBy(b => b.Title).ToList();
        }

        public async Task<List<BookBorrowing>> GetAllBorrowings()
        {
            return await _bookRepo.GetAllBorrowings();
        }

        public async Task<List<BookBorrowing>> GetAllBorrowingsByUserId(int UserId)
        {
            return await _bookRepo.GetAllBorrowingsByUserId(UserId);
        }

        public async Task<List<BookBorrowing>> GetAllBorrowingsByISBN(string ISBN)
        {
            return await _bookRepo.GetAllBorrowingsByISBN(ISBN);
        }

        // Get all the books borrowings with details (users, books)
        public async Task<BorrowingsVM> GetAllBorrowingsWithDetails()
        {
            BorrowingsVM borrowingsVM = new()
            {
                users = await _accountService.GetAllUsers(),
                books = await GetAllBooks(),
                borrowings = await GetAllBorrowings()
            };

            return borrowingsVM;
        }

        // Get all the books borrowings for specific user with details
        public async Task<UserBorrowingsVM> GetAllBorrowingsWithDetailsByUser(int UserId)
        {
            UserBorrowingsVM borrowingsVM = new()
            {
                books = await GetAllBooks(),
                borrowings = await GetAllBorrowingsByUserId(UserId)
            };

            return borrowingsVM;
        }

        public async Task<ResultModel> AddNewBook(BookDto bookDto)
        {
            // Check if the book details are empty
            if (string.IsNullOrWhiteSpace(bookDto.ISBN) || string.IsNullOrWhiteSpace(bookDto.Title) || string.IsNullOrWhiteSpace(bookDto.Author))
                return new ResultModel { Result = false, Message = "Please input ISBN, Title and Author!" };

            var addResult = await _bookRepo.AddBook(new Book { ISBN = bookDto.ISBN, Title = bookDto.Title, Author = bookDto.Author, IsAvailable = true });
            if (addResult.Result)
                return new ResultModel { Result = true, Message = "Added successfully!" };

            return new ResultModel { Result = false, Message = addResult.Message };
        }

        public async Task<ResultModel> UpdateBookInfo(UpdatedBookDto updatedBookDto)
        {
            // Check if the book details are empty then do not update anything
            if (string.IsNullOrWhiteSpace(updatedBookDto.Title) && string.IsNullOrWhiteSpace(updatedBookDto.Author))
                return new ResultModel { Result = true, Message = "Nothing updated!" };

            var updateResult = await _bookRepo.UpdateBook(new UpdatedBook { Title = updatedBookDto.Title, Author = updatedBookDto.Author }, updatedBookDto.ISBN!);
            if (updateResult.Result)
                return new ResultModel { Result = true, Message = "Book Info updated successfully!" };

            return new ResultModel { Result = false, Message = updateResult.Message };
        }

        public async Task<ResultModel> BorrowBooks(List<string> ISBNs, int UserId)
        {
            // Check if the there are any sent books, if not return false
            if (!ISBNs.Any())
                return new ResultModel { Result = false, Message = "at least one book is required!" };

            var borrowResult = await _bookRepo.BorrowBooks(ISBNs, UserId);
            if (borrowResult.Result)
                return new ResultModel { Result = true, Message = "Books borrowed successfully!" };

            return new ResultModel { Result = false, Message = borrowResult.Message };
        }

        public async Task<ResultModel> ReturnBooks(List<string> ISBNs)
        {
            // Check if the there are any sent books, if not return false
            if (!ISBNs.Any())
                return new ResultModel { Result = false, Message = "at least one book is required!" };

            var returnResult = await _bookRepo.ReturnBooks(ISBNs);
            if (returnResult.Result)
                return new ResultModel { Result = true, Message = "Books returned successfully!" };

            return new ResultModel { Result = false, Message = returnResult.Message };
        }
    }
}