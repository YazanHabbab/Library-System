using System.Data;
using data_access.Helpers;
using data_access.Interfaces;
using data_access.Models;
using data_access.Models.Other;
using Microsoft.Data.SqlClient;

namespace data_access.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnectionHelper _connectionHelper;

        public UserRepository(SqlConnectionHelper connectionHelper)
        {
            _connectionHelper = connectionHelper;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Users", connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            UserId = (int)reader["UserId"],
                            Name = (string)reader["Name"],
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
                await connection.CloseAsync();
            }
            return users;
        }

        public async Task<User> GetUserById(int Id)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Users WHERE UserId = @UserId", connection);

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
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            UserId = (int)reader["UserId"],
                            Name = (string)reader["Name"],
                            Password = (string)reader["HashedPassword"],
                            IsActive = (bool)reader["IsActive"]
                        };
                    }
                }

                await connection.CloseAsync();

                return null!;
            }
        }

        public async Task<User> GetUserByName(string Name)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                var command = new SqlCommand("SELECT * FROM Users WHERE Name = @Name", connection);

                SqlParameter IdParameter = new()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = Name
                };
                command.Parameters.Add(IdParameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            UserId = (int)reader["UserId"],
                            Name = (string)reader["Name"],
                            Password = (string)reader["HashedPassword"],
                            IsActive = (bool)reader["IsActive"]
                        };
                    }
                }

                await connection.CloseAsync();

                return null!;
            }
        }

        public async Task<ResultModel> CreateUser(User user)
        {
            using (var connection = _connectionHelper.CreateConnection())
            {
                // Check if user name exists
                if (await GetUserByName(user.Name) is not null)
                    return new ResultModel { Result = false, Message = "This name is taken!" };

                // Else if the user name does not exist then proceed to create a new user
                var command = new SqlCommand("INSERT INTO Users (Name, HashedPassword, IsActive) VALUES (@Name, @HashedPassword, '1')", connection);

                SqlParameter NameParameter = new()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = user.Name
                };

                SqlParameter PasswordParameter = new()
                {
                    ParameterName = "@HashedPassword",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = PasswordHasher.Hash(user.Password)
                };

                command.Parameters.Add(NameParameter);
                command.Parameters.Add(PasswordParameter);

                await connection.OpenAsync();

                // Execute the command and check if it succeded
                if (await command.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "User created successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = false, Message = "Could not create the user!" };
            }
        }

        public async Task<ResultModel> UpdateUser(UpdatedUser updatedUser, int UserId)
        {
            // check if the user exists
            var currentUser = await GetUserById(UserId);
            if (currentUser is null)
                return new ResultModel { Result = false, Message = "UserId is not correct!" };
            else
            {
                // Try to update user name
                var nameResult = await UpdateUserName(updatedUser.Name!, currentUser);

                // Try to update user password
                var passwordResult = await UpdateUserPassword(updatedUser.CurrentPassword!, updatedUser.NewPassword!, currentUser);

                // Check if both updates failed
                if (nameResult.Result is false && passwordResult.Result is false)
                    return new ResultModel { Result = false, Message = nameResult.Message + ", " + passwordResult.Message };

                // Check if both updates succeded
                else if (nameResult.Result is true && passwordResult.Result is true)
                    return new ResultModel { Result = true, Message = nameResult.Message + ", " + passwordResult.Message };

                // Check if one of them failed
                else
                    return new ResultModel { Result = false, Message = nameResult.Message + ", " + passwordResult.Message };
            }
        }

        private async Task<ResultModel> UpdateUserName(string newName, User currentUser)
        {
            // Check new user name if empty
            if (string.IsNullOrWhiteSpace(newName))
                return new ResultModel { Result = true };

            // Check if current user name and new name are the same
            else if (currentUser.Name == newName)
                return new ResultModel { Result = false, Message = "Your current name and new name cannot be the same!" };

            // Check if new user name is not taken
            else if (await GetUserByName(newName) is not null)
                return new ResultModel { Result = false, Message = "This name does exist!" };

            else
                currentUser.Name = newName;

            // Else update user name in the database
            using (var connection = _connectionHelper.CreateConnection())
            {
                var UpdateNamecommand = new SqlCommand("UPDATE Users SET Name = @Name WHERE UserId = @UserId", connection);

                SqlParameter IdParameter = new()
                {
                    ParameterName = "@UserId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = currentUser.UserId
                };
                SqlParameter NameParameter = new()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = newName
                };
                UpdateNamecommand.Parameters.Add(NameParameter);
                UpdateNamecommand.Parameters.Add(IdParameter);

                await connection.OpenAsync();

                // Execute command and check if succeded
                if (await UpdateNamecommand.ExecuteNonQueryAsync() > 0)
                {
                    await connection.CloseAsync();
                    return new ResultModel { Result = true, Message = "Name updated successfully!" };
                }

                await connection.CloseAsync();
                return new ResultModel { Result = true };
            }
        }

        private async Task<ResultModel> UpdateUserPassword(string currentPassword, string newPassword, User currentUser)
        {
            // Check current password and new password if both empty then do not update anything
            if (string.IsNullOrWhiteSpace(currentPassword) && string.IsNullOrWhiteSpace(newPassword))
                return new ResultModel { Result = true };

            // Check current password and new password if one of them is empty
            else if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                return new ResultModel { Result = false, Message = "Please enter current password and new password" };

            // Check if current password and new password are the same
            else if (currentPassword == newPassword)
                return new ResultModel { Result = false, Message = "New password should not be the same as the current password!" };

            // Check if the current password is correct
            else if (!PasswordHasher.Verify(currentPassword, currentUser.Password))
                return new ResultModel { Result = false, Message = "Current password is not correct!" };

            else
            {
                using (var connection = _connectionHelper.CreateConnection())
                {
                    // Update the user password in the database
                    var UpdatePasswordcommand = new SqlCommand("UPDATE Users SET HashedPassword = @Password WHERE UserId = @UserId", connection);

                    SqlParameter IdParameter = new()
                    {
                        ParameterName = "@UserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = currentUser.UserId
                    };
                    SqlParameter PasswordParameter = new()
                    {
                        ParameterName = "@Password",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = ParameterDirection.Input,
                        Value = PasswordHasher.Hash(newPassword)
                    };
                    UpdatePasswordcommand.Parameters.Add(IdParameter);
                    UpdatePasswordcommand.Parameters.Add(PasswordParameter);

                    await connection.OpenAsync();

                    // Execute command and check if succeded
                    if (await UpdatePasswordcommand.ExecuteNonQueryAsync() > 0)
                    {
                        await connection.CloseAsync();
                        return new ResultModel { Result = true, Message = "Password updated successfully!" };
                    }

                    await connection.CloseAsync();
                    return new ResultModel { Result = true };
                }
            }
        }

        // Soft delete the user
        public async Task<ResultModel> DeleteUser(int UserId)
        {
            // check if the user exists
            var currentUser = await GetUserById(UserId);
            if (currentUser is null)
                return new ResultModel { Result = false, Message = "User is not found!" };
            else
            {
                using (var connection = _connectionHelper.CreateConnection())
                {
                    var checkCommand = new SqlCommand("SELECT * FROM Users WHERE IsActive = 0 AND UserId = @UserId", connection);

                    SqlParameter IdParameter = new()
                    {
                        ParameterName = "@UserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = UserId
                    };
                    checkCommand.Parameters.Add(IdParameter);

                    await connection.OpenAsync();

                    // Execute command and check if user is deleted before
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return new ResultModel { Result = false, Message = "User is already deleted!" };

                    }

                    // Else delete the user
                    var deleteCommand = new SqlCommand("UPDATE Users SET IsActive = 0 WHERE UserId = @UserId", connection);
                    IdParameter = new()
                    {
                        ParameterName = "@UserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = UserId
                    };
                    deleteCommand.Parameters.Add(IdParameter);

                    // Execute the command and check if delete succeded
                    if (await deleteCommand.ExecuteNonQueryAsync() > 0)
                    {
                        await connection.CloseAsync();
                        return new ResultModel { Result = true, Message = "User deleted successfully!" };
                    }

                    await connection.CloseAsync();
                    return new ResultModel { Result = false, Message = "Could not delete user!" };
                }
            }
        }

        public async Task<ResultModel> ActivateUser(int UserId)
        {
            var currentUser = await GetUserById(UserId);
            if (currentUser is null)
                return new ResultModel { Result = false, Message = "User is not found!" };
            else
            {
                using (var connection = _connectionHelper.CreateConnection())
                {
                    // Check if user not active
                    var checkCommand = new SqlCommand("SELECT * FROM Users WHERE IsActive = 1 AND UserId = @UserId", connection);

                    SqlParameter IdParameter = new()
                    {
                        ParameterName = "@UserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = UserId
                    };
                    checkCommand.Parameters.Add(IdParameter);

                    await connection.OpenAsync();

                    // Execute the command and check if user already active
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return new ResultModel { Result = false, Message = "User is already activated!" };

                    }

                    // Else activate the user
                    var deleteCommand = new SqlCommand("UPDATE Users SET IsActive = 1 WHERE UserId = @UserId", connection);
                    IdParameter = new()
                    {
                        ParameterName = "@UserId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = UserId
                    };
                    deleteCommand.Parameters.Add(IdParameter);

                    // Execute the command and check if activation succeded
                    if (await deleteCommand.ExecuteNonQueryAsync() > 0)
                    {
                        await connection.CloseAsync();
                        return new ResultModel { Result = true, Message = "User activated successfully!" };
                    }

                    await connection.CloseAsync();
                    return new ResultModel { Result = false, Message = "Could not activate user!" };
                }
            }
        }
    }
}