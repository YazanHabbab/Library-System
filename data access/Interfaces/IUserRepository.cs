using data_access.Models;
using data_access.Models.Other;

namespace data_access.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int Id);
        Task<User> GetUserByName(string Name);
        Task<ResultModel> CreateUser(User user);
        Task<ResultModel> UpdateUser(UpdatedUser updatedUser, int UserId);
        Task<ResultModel> DeleteUser(int UserId);
        Task<ResultModel> ActivateUser(int UserId);
    }
}