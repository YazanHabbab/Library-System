using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data_access.Models;
using data_access.Models.Other;

namespace data_access.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBookByISBN(string ISBN);
        Task<Book> GetBookByTitle(string title);
        Task<Book> GetBookByAuthor(string author);
        Task<List<Book>> GetBooksByISBNOrTitleOrAuthor(string searchTerm);
        Task<List<BookBorrowing>> GetAllBorrowings();
        Task<List<BookBorrowing>> GetAllBorrowingsByUserId(int Id);
        Task<List<BookBorrowing>> GetAllBorrowingsByISBN(string ISBN);
        Task<ResultModel> AddBook(Book book);
        Task<ResultModel> UpdateBook(UpdatedBook updatedBook, string ISBN);
        Task<ResultModel> BorrowBook(string ISBN, int UserId);
        Task<ResultModel> ReturnBook(string ISBN);
    }
}