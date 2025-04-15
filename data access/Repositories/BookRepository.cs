using System.Data;
using data_access.Helpers;
using data_access.Interfaces;
using data_access.Models;
using data_access.Models.Other;
using Microsoft.Data.SqlClient;

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
                var command = new SqlCommand("SELECT * FROM Books WHERE ISBN = @ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
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
                var command = new SqlCommand("SELECT * FROM Books WHERE Title = @Title", connection);

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
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
                var command = new SqlCommand("SELECT * FROM Books WHERE Author = @Author", connection);

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
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

        // Search books with a serach term and return them
        public async Task<List<Book>> GetBooksByISBNOrTitleOrAuthor(string searchTerm)
        {
            var books = new List<Book>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Books WHERE ISBN LIKE @Term OR Title LIKE @Term OR Author LIKE @Term", connection);

                SqlParameter TermParameter = new()
                {
                    ParameterName = "@Term",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = $"%{searchTerm}%"
                };
                command.Parameters.Add(TermParameter);

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
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
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
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
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
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = book.ISBN
                };

                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = book.Title
                };

                SqlParameter AuthorParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = book.Author
                };

                SqlParameter IsAvailableParameter = new()
                {
                    ParameterName = "@IsAvailable",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
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
                // Update the title
                var titleResult = await UpdateBookTitle(updatedBook.Title!, currentBook);

                // Update the author name
                var authorResult = await UpdateBookAuthor(updatedBook.Author!, currentBook);

                // Chcek If updating the title and author name failed
                if (titleResult.Result is false && authorResult.Result is false)
                    return new ResultModel { Result = false, Message = titleResult.Message + ", " + authorResult.Message };

                // Check if updating the title and author name succeded
                else if (titleResult.Result is true && authorResult.Result is true)
                    return new ResultModel { Result = true, Message = titleResult.Message + ", " + authorResult.Message };

                // Else something went wrong, Return the error message
                else
                    return new ResultModel { Result = false, Message = titleResult.Message + ", " + authorResult.Message };
            }
        }

        private async Task<ResultModel> UpdateBookTitle(string newTitle, Book currentBook)
        {
            // Check title
            if (string.IsNullOrWhiteSpace(newTitle))
                return new ResultModel { Result = true };

            // Check if the current book title is the same as new title
            else if (currentBook.Title == newTitle)
                return new ResultModel { Result = false, Message = "The current book title and new title cannot be the same!" };

            // check if the book title does exist before
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
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = currentBook.ISBN
                };
                SqlParameter TitleParameter = new()
                {
                    ParameterName = "@Title",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = newTitle
                };
                UpdateTitlecommand.Parameters.Add(ISBNParameter);
                UpdateTitlecommand.Parameters.Add(TitleParameter);

                await connection.OpenAsync();

                // Execute the command and check if it succeded
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
            // Check Author name if empty
            if (string.IsNullOrWhiteSpace(newAuthor))
                return new ResultModel { Result = true };

            // Check Author name is the same as the new name
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
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = currentBook.ISBN
                };
                SqlParameter AuthorParameter = new()
                {
                    ParameterName = "@Author",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = newAuthor
                };
                UpdateTitlecommand.Parameters.Add(ISBNParameter);
                UpdateTitlecommand.Parameters.Add(AuthorParameter);

                await connection.OpenAsync();

                // Execute the command and check if it succeded
                if (await UpdateTitlecommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Author updated successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = true };
            }
        }

        public async Task<ResultModel> BorrowBooks(List<string> ISBNs, int UserId)
        {
            // Check if the user exists
            if (await _userRepository.GetUserById(UserId) is null)
                return new ResultModel { Result = false, Message = "UserId is not correct" };

            // Define the table of data
            var borrowingsTable = new DataTable();
            borrowingsTable.Columns.Add("ISBN", typeof(string));
            borrowingsTable.Columns.Add("UserId", typeof(int));
            borrowingsTable.Columns.Add("BorrowDate", typeof(DateTime));

            // Add rows to the book table
            foreach (var ISBN in ISBNs)
            {
                // Check if the book exists
                if (await GetBookByISBN(ISBN) is null)
                    return new ResultModel { Result = false, Message = "one of the ISBNs is not correct!" };

                // Check if the book is borrowed before
                if (!await IsBookAvailable(ISBN))
                    return new ResultModel { Result = false, Message = "Cannot borrow books because one of the books is not available!" };

                // Proceed to adding the row
                borrowingsTable.Rows.Add(ISBN, UserId, DateTime.Now);
            }

            // proceed to borrow the books
            using (var connection = _connectionHelper.CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = (SqlTransaction)await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Try to update books availability
                        var parameterPlaceholders = new List<string>();
                        var parameters = new List<SqlParameter>();

                        for (int i = 0; i < ISBNs.Count; i++)
                        {
                            parameterPlaceholders.Add($"@ISBN{i}");
                            parameters.Add(new SqlParameter($"@ISBN{i}", ISBNs[i]));
                        }

                        var updateCommand = new SqlCommand(
                            $"UPDATE Books SET IsAvailable = 0 WHERE ISBN IN ({string.Join(",", parameterPlaceholders)})",
                            connection,
                            transaction);
                        updateCommand.Parameters.AddRange(parameters.ToArray());

                        // Execute the command and check if it succeded, If not do not update anything
                        int updatedRows = await updateCommand.ExecuteNonQueryAsync();
                        if (updatedRows == 0)
                        {
                            await transaction.RollbackAsync();
                            await connection.CloseAsync();
                            return new ResultModel { Result = false, Message = "Could not update book availability!" };
                        }

                        // Insert all borrowings
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "Borrowings";
                            bulkCopy.ColumnMappings.Add("ISBN", "ISBN");
                            bulkCopy.ColumnMappings.Add("UserId", "UserId");
                            bulkCopy.ColumnMappings.Add("BorrowDate", "BorrowDate");

                            await bulkCopy.WriteToServerAsync(borrowingsTable);
                        }

                        await transaction.CommitAsync();
                        await connection.CloseAsync();
                        return new ResultModel { Result = true, Message = "Books borrowed successfully!" };
                    }
                    // If anything wrong happend rollback and do not update anything
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        await connection.CloseAsync();
                        return new ResultModel { Result = false, Message = $"Error borrowing books: {ex.Message}" };
                    }
                }
            }
        }

        // Medthod to check if the book is available
        private async Task<bool> IsBookAvailable(string ISBN)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var Command = new SqlCommand("SELECT IsAvailable FROM Books WHERE ISBN = " + $"@ISBN", connection);

                SqlParameter ISBNParameter = new()
                {
                    ParameterName = "@ISBN",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
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

        public async Task<ResultModel> ReturnBooks(List<string> ISBNs)
        {
            // Add rows to the book table
            foreach (var ISBN in ISBNs)
            {
                // Check if the book exists
                if (await GetBookByISBN(ISBN) is null)
                    return new ResultModel { Result = false, Message = "one of the ISBNs is not correct!" };

                // Check if the book is returned before
                if (await IsBookAvailable(ISBN))
                    return new ResultModel { Result = false, Message = "Cannot return books because one of the books is already returned!" };
            }

            // proceed to borrow the book
            using (var connection = _connectionHelper.CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = (SqlTransaction)await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Try to update books availability
                        var parameterPlaceholders = new List<string>();
                        var parameters = new List<SqlParameter>();

                        for (int i = 0; i < ISBNs.Count; i++)
                        {
                            parameterPlaceholders.Add($"@ISBN{i}");
                            parameters.Add(new SqlParameter($"@ISBN{i}", ISBNs[i]));
                        }

                        var Command = new SqlCommand($"UPDATE Books SET IsAvailable = 1 WHERE ISBN IN ({string.Join(",", parameterPlaceholders)});" +
                        "UPDATE Borrowings SET ReturnDate = " + $"@ReturnDate WHERE ReturnDate IS NULL AND ISBN IN ({string.Join(",", parameterPlaceholders)})", connection, transaction);

                        Command.Parameters.AddRange(parameters.ToArray());
                        Command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@ReturnDate",
                            SqlDbType = SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = DateTime.Now
                        });

                        parameterPlaceholders = new List<string>();
                        parameters = new List<SqlParameter>();

                        for (int i = 0; i < ISBNs.Count; i++)
                        {
                            parameterPlaceholders.Add($"@ISBN{i}");
                            parameters.Add(new SqlParameter($"@ISBN{i}", ISBNs[i]));
                        }

                        // Execute command and check if it succeded, Else rollback and do not update anything
                        int updatedRows = await Command.ExecuteNonQueryAsync();
                        if (updatedRows == 0)
                        {
                            await transaction.RollbackAsync();
                            await connection.CloseAsync();
                            return new ResultModel { Result = false, Message = "Could not return books!" };
                        }

                        await transaction.CommitAsync();
                        await connection.CloseAsync();
                        return new ResultModel { Result = true, Message = "Books returned successfully!" };
                    }

                    // If something went wrong then rollback and do not update anything
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        await connection.CloseAsync();
                        return new ResultModel { Result = false, Message = $"Error returning books: {ex.Message}" };
                    }
                }
            }
        }
    }
}