using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using business_logic.DTOs;
using data_access.Models;
using data_access.Models.Other;
using data_access.Repositories;

namespace business_logic.Services
{
    public class LibraryService
    {
        private readonly BookRepository _bookRepo;
        private readonly UserRepository _userRepo;

        public LibraryService(BookRepository bookRepo, UserRepository userRepo)
        {
            _bookRepo = bookRepo;
            _userRepo = userRepo;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookRepo.GetAllBooks();
        }

        public async Task<List<Book>> SearchBooksByISBNOrTitleOrAuthor(string searchTerm = "")
        {
            return await _bookRepo.GetBooksByISBNOrTitleOrAuthor(searchTerm);
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

        public async Task<ResultModel> AddNewBook(BookDto bookDto)
        {
            if (string.IsNullOrWhiteSpace(bookDto.ISBN) || string.IsNullOrWhiteSpace(bookDto.Title) || string.IsNullOrWhiteSpace(bookDto.Author))
                return new ResultModel { Result = false, Message = "Please input ISBN, Title and Author!" };

            var addResult = await _bookRepo.AddBook(new Book { ISBN = bookDto.ISBN, Title = bookDto.Title, Author = bookDto.Author, IsAvailable = true });
            if (addResult.Result)
                return new ResultModel { Result = true, Message = "Added successfully!" };

            return new ResultModel { Result = false, Message = addResult.Message };
        }

        public async Task<ResultModel> UpdateBookInfo(UpdatedBookDto updatedBookDto, string ISBN)
        {
            if (string.IsNullOrWhiteSpace(updatedBookDto.Title) && string.IsNullOrWhiteSpace(updatedBookDto.Author))
                return new ResultModel { Result = true, Message = "Nothing updated!" };

            var updateResult = await _bookRepo.UpdateBook(new UpdatedBook { Title = updatedBookDto.Title, Author = updatedBookDto.Author }, ISBN);
            if (updateResult.Result)
                return new ResultModel { Result = true, Message = "Book Info updated successfully!" };

            return new ResultModel { Result = false, Message = updateResult.Message };
        }

        public async Task<ResultModel> BorrowBook(string ISBN, int UserId)
        {
            if(string.IsNullOrWhiteSpace(ISBN))
                return new ResultModel { Result = false, Message = "ISBN is required!"};

            var borrowResult = await _bookRepo.BorrowBook(ISBN, UserId);
            if (borrowResult.Result)
                return new ResultModel { Result = true, Message = "Book is borrowed successfully!" };

            return new ResultModel { Result = false, Message = borrowResult.Message };
        }

        public async Task<ResultModel> ReturnBook(string ISBN)
        {
            if(string.IsNullOrWhiteSpace(ISBN))
                return new ResultModel { Result = false, Message = "ISBN is required!"};

            var returnResult = await _bookRepo.ReturnBook(ISBN);
            if (returnResult.Result)
                return new ResultModel { Result = true, Message = "Book is returned successfully!" };

            return new ResultModel { Result = false, Message = returnResult.Message };
        }
    }
}