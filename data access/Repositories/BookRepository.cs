using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data_access.Helpers;
using data_access.Interfaces;
using data_access.Models;
using data_access.Models.Other;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace data_access.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly SqlConnectionHelper _connectionHelper;
        private readonly IUserRepository _userRepository;
        public BookRepository(SqlConnectionHelper connectionHelper, IUserRepository userRepository)
        {
            _connectionHelper = connectionHelper;
            _userRepository = userRepository;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            var books = new List<Book>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books", connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new Book
                        {
                            ISBN = (string)reader["ISBN"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            IsAvailable = (bool)reader["IsAvailable"],
                        });
                    }
                }
                await connection.CloseAsync();
            }
            return books;
        }

        public async Task<Book> GetBookByISBN(string ISBN)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books WHERE ISBN = " + $"@ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };
                command.Parameters.Add(ISBNParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Book
                        {
                            ISBN = (string)reader["ISBN"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            IsAvailable = (bool)reader["IsAvailable"],
                        };
                    }
                }

                await connection.CloseAsync();

                return null!;
            }
        }

        public async Task<Book> GetBookByTitle(string title)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books WHERE Title = " + $"@Title", connection);

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = title
                };
                command.Parameters.Add(TitleParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Book
                        {
                            ISBN = (string)reader["ISBN"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            IsAvailable = (bool)reader["IsAvailable"],
                        };
                    }
                }

                await connection.CloseAsync();

                return null!;
            }
        }

        public async Task<Book> GetBookByAuthor(string author)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books WHERE Author = " + $"@Author", connection);

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = author
                };
                command.Parameters.Add(TitleParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Book
                        {
                            ISBN = (string)reader["ISBN"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            IsAvailable = (bool)reader["IsAvailable"],
                        };
                    }
                }

                await connection.CloseAsync();

                return null!;
            }
        }

        public async Task<List<Book>> GetBooksByISBNOrTitleOrAuthor(string searchTerm)
        {
            var books = new List<Book>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books WHERE ISBN LIKE @Term OR Title LIKE @Term OR Author LIKE @Term", connection);

                SqlParameter TermParameter = new()
                {
                    ParameterName = "@Term",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = $"%{searchTerm}%"
                };
                command.Parameters.Add(TermParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        books.Add(new Book
                        {
                            ISBN = (string)reader["ISBN"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            IsAvailable = (bool)reader["IsAvailable"],
                        });
                    }
                }

                await connection.CloseAsync();

                return books;
            }
        }

        public async Task<List<BookBorrowing>> GetAllBorrowings()
        {
            var borrowings = new List<BookBorrowing>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Borrowings", connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        borrowings.Add(new BookBorrowing
                        {
                            BorrowingId = (int)reader["BorrowingId"],
                            ISBN = (string)reader["ISBN"],
                            UserId = (int)reader["UserId"],
                            BorrowDate = (DateTime)reader["BorrowDate"],
                            ReturnDate = reader["ReturnDate"] as DateTime?
                        });
                    }
                }
                await connection.CloseAsync();
            }
            return borrowings;
        }

        public async Task<List<BookBorrowing>> GetAllBorrowingsByUserId(int Id)
        {
            var borrowings = new List<BookBorrowing>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Borrowings WHERE UserId = " + $"@UserId", connection);

                SqlParameter IdParameter = new()
                {
                    ParameterName = "@UserId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = Id
                };
                command.Parameters.Add(IdParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        borrowings.Add(new BookBorrowing
                        {
                            BorrowingId = (int)reader["BorrowingId"],
                            ISBN = (string)reader["ISBN"],
                            UserId = (int)reader["UserId"],
                            BorrowDate = (DateTime)reader["BorrowDate"],
                            ReturnDate = reader["ReturnDate"] as DateTime?
                        });
                    }
                }
                await connection.CloseAsync();
            }
            return borrowings;
        }

        public async Task<List<BookBorrowing>> GetAllBorrowingsByISBN(string ISBN)
        {
            var borrowings = new List<BookBorrowing>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Borrowings WHERE ISBN = " + $"@ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };
                command.Parameters.Add(ISBNParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        borrowings.Add(new BookBorrowing
                        {
                            BorrowingId = (int)reader["BorrowingId"],
                            ISBN = (string)reader["ISBN"],
                            UserId = (int)reader["UserId"],
                            BorrowDate = (DateTime)reader["BorrowDate"],
                            ReturnDate = reader["ReturnDate"] as DateTime?
                        });
                    }
                }
                await connection.CloseAsync();
            }
            return borrowings;
        }

        public async Task<ResultModel> AddBook(Book book)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                // Check if book ISBN exists
                if (await GetBookByISBN(book.ISBN) is not null)
                    return new ResultModel { Result = false, Message = "This book ISBN is already added!" };

                // Check if book title exists
                if (await GetBookByTitle(book.Title) is not null)
                    return new ResultModel { Result = false, Message = "This book title is already added!" };

                // Else proceed to add a new book
                var command = new SqlCommand("INSERT INTO Books (ISBN, Title, Author, IsAvailable) VALUES " + $"(@ISBN, @Title, @Author, @IsAvailable)", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = book.ISBN
                };

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = book.Title
                };

                SqlParameter AuthorParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = book.Author
                };

                SqlParameter IsAvailableParameter = new()
                {
                    ParameterName = "@IsAvailable",
                    SqlDbType = System.Data.SqlDbType.Bit,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = book.IsAvailable
                };

                command.Parameters.AddRange(new[] { ISBNParameter, TitleParameter, AuthorParameter, IsAvailableParameter });

                await connection.OpenAsync();

                if (await command.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Book added successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = false, Message = "Could not add the book!" };
            }
        }

        public async Task<ResultModel> UpdateBook(UpdatedBook updatedBook, string ISBN)
        {
            // Check if the book exists
            var currentBook = await GetBookByISBN(ISBN);
            if (currentBook is null)
                return new ResultModel { Result = false, Message = "ISBN is not correct!" };

            else
            {
                var titleResult = await UpdateBookTitle(updatedBook.Title!, currentBook);
                var authorResult = await UpdateBookAuthor(updatedBook.Author!, currentBook);

                if (titleResult.Result is false && authorResult.Result is false)
                    return new ResultModel { Result = false, Message = titleResult.Message + ", " + authorResult.Message };

                else if (titleResult.Result is true && authorResult.Result is true)
                    return new ResultModel { Result = true, Message = titleResult.Message + ", " + authorResult.Message };

                else
                    return new ResultModel { Result = false, Message = titleResult.Message + ", " + authorResult.Message };
            }
        }

        private async Task<ResultModel> UpdateBookTitle(string newTitle, Book currentBook)
        {
            // Check title
            if (string.IsNullOrWhiteSpace(newTitle))
                return new ResultModel { Result = true };

            else if (currentBook.Title == newTitle)
                return new ResultModel { Result = false, Message = "The current book title and new title cannot be the same!" };

            else if (await GetBookByTitle(newTitle) is not null)
                return new ResultModel { Result = false, Message = "This title does exist!" };

            else
                currentBook.Title = newTitle;

            // Else update the book title in the database
            using (var connection = _connectionHelper.CreateConnection())
            {
                var UpdateTitlecommand = new SqlCommand("UPDATE Books SET Title = @Title WHERE ISBN = @ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = currentBook.ISBN
                };
                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = newTitle
                };
                UpdateTitlecommand.Parameters.Add(ISBNParameter);
                UpdateTitlecommand.Parameters.Add(TitleParameter);

                await connection.OpenAsync();

                if (await UpdateTitlecommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Title updated successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = true };
            }
        }

        private async Task<ResultModel> UpdateBookAuthor(string newAuthor, Book currentBook)
        {
            // Check Author
            if (string.IsNullOrWhiteSpace(newAuthor))
                return new ResultModel { Result = true };

            else if (currentBook.Author == newAuthor)
                return new ResultModel { Result = false, Message = "The current book author name and new author name cannot be the same!" };

            else
                currentBook.Author = newAuthor;

            // Else update the author in the database
            using (var connection = _connectionHelper.CreateConnection())
            {
                var UpdateTitlecommand = new SqlCommand("UPDATE Books SET Author = @Author WHERE ISBN = @ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = currentBook.ISBN
                };
                SqlParameter AuthorParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = newAuthor
                };
                UpdateTitlecommand.Parameters.Add(ISBNParameter);
                UpdateTitlecommand.Parameters.Add(AuthorParameter);

                await connection.OpenAsync();

                if (await UpdateTitlecommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Author updated successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = true };
            }
        }

        public async Task<ResultModel> BorrowBook(string ISBN, int UserId)
        {
            // Check if the book exists
            if (await GetBookByISBN(ISBN) is null)
                return new ResultModel { Result = false, Message = "ISBN is not correct!" };

            // Check if the book is borrowed before
            if (!await IsBookAvailable(ISBN))
                return new ResultModel { Result = false, Message = "Cannot borrow book because it is borrowed!" };

            // Check if the user exists
            if (await _userRepository.GetUserById(UserId) is null)
                return new ResultModel { Result = false, Message = "UserId is not correct" };

            // Else proceed to borrow the book
            using (var connection = _connectionHelper.CreateConnection())
            {
                var AvailableCommand = new SqlCommand("UPDATE Books SET IsAvailable = 0 WHERE ISBN = @ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };
                AvailableCommand.Parameters.Add(ISBNParameter);

                var BorrowingCommand = new SqlCommand("INSERT INTO Borrowings (ISBN, UserId, BorrowDate) VALUES " + $"(@ISBN, @UserId, @BorrowDate)", connection);

                ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };

                SqlParameter UserIdParameter = new()
                {
                    ParameterName = "@UserId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = UserId
                };

                SqlParameter BorrowDateParameter = new()
                {
                    ParameterName = "@BorrowDate",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = DateTime.Now
                };
                BorrowingCommand.Parameters.AddRange(new[] { ISBNParameter, UserIdParameter, BorrowDateParameter });

                await connection.OpenAsync();

                if (await AvailableCommand.ExecuteNonQueryAsync() > 0 && await BorrowingCommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Book borrowed successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = false, Message = "Could not borrow the book!" };
            }
        }

        private async Task<bool> IsBookAvailable(string ISBN)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var Command = new SqlCommand("SELECT IsAvailable FROM Books WHERE ISBN = " + $"@ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };
                Command.Parameters.Add(ISBNParameter);

                await connection.OpenAsync();

                using (var reader = await Command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader.GetBoolean(reader.GetOrdinal("IsAvailable"));
                    }
                }
                await connection.CloseAsync();
            }
            return false;
        }

        public async Task<ResultModel> ReturnBook(string ISBN)
        {
            // Check if the book exists
            if (await GetBookByISBN(ISBN) is null)
                return new ResultModel { Result = false, Message = "ISBN is not correct!" };

            // Check if the book is already returned
            if (await IsBookAvailable(ISBN))
                return new ResultModel { Result = false, Message = "Cannot return book because it is already returned!" };

            // Else proceed to borrow the book
            using (var connection = _connectionHelper.CreateConnection())
            {
                var AvailableCommand = new SqlCommand("UPDATE Books SET IsAvailable = 1 WHERE ISBN = @ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };
                AvailableCommand.Parameters.Add(ISBNParameter);

                var ReturningCommand = new SqlCommand("UPDATE Borrowings SET ReturnDate = " + $"@ReturnDate WHERE ISBN = @ISBN", connection);

                ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = ISBN
                };

                SqlParameter ReturnDateParameter = new()
                {
                    ParameterName = "@ReturnDate",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = DateTime.Now
                };
                ReturningCommand.Parameters.AddRange(new[] { ISBNParameter, ReturnDateParameter });

                await connection.OpenAsync();

                if (await AvailableCommand.ExecuteNonQueryAsync() > 0 && await ReturningCommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Book returned successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = false, Message = "Could not return the book!" };
            }
        }
    }
}