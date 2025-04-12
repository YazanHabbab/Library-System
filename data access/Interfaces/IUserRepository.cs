using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data_access.Models;
using data_access.Models.Other;
using Testing.Models;

namespace data_access.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int Id);
        Task<User> GetUserByName(string Name);
        Task<ResultModel> CreateUser(User user);
        Task<ResultModel> CheckCredentials(User user);
        Task<ResultModel> UpdateUser(UpdatedUser updatedUser, int UserId);
        Task<ResultModel> DeleteUser(int UserId);
    }
}