using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using business_logic.DTOs;
using data_access.Models;
using data_access.Models.Other;
using data_access.Repositories;
using Testing.Models;

namespace business_logic.Services
{
    public class AccountService
    {
        private readonly UserRepository _userRepo;

        public AccountService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepo.GetAllUsers();
        }

        public async Task<User> SearchUserById(int Id)
        {
            return await _userRepo.GetUserById(Id);
        }

        public async Task<User> SearchUserByName(string Name)
        {
            return await _userRepo.GetUserByName(Name);
        }

        public async Task<ResultModel> CreateNewAccount(UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Password))
                return new ResultModel { Result = false, Message = "Please input both name and password!" };

            var createResult = await _userRepo.CreateUser(new User { Name = userDto.Name, Password = userDto.Password });
            if (createResult.Result)
                return new ResultModel { Result = true, Message = "Created successfully!" };

            return new ResultModel { Result = false, Message = createResult.Message };
        }

        public async Task<ResultModel> LoginAccount(UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Password))
                return new ResultModel { Result = false, Message = "Name and Password are required!" };

            var checkResult = await _userRepo.CheckCredentials(new User { Name = userDto.Name, Password = userDto.Password });
            if (checkResult.Result)
                return new ResultModel { Result = true, Message = "Logged in successfully!" };

            return new ResultModel { Result = false, Message = checkResult.Message };
        }

        public async Task<ResultModel> UpdateUserInfo(UpdatedUserDto updatedUser, int UserId)
        {
            if (string.IsNullOrWhiteSpace(updatedUser.NewName) && string.IsNullOrWhiteSpace(updatedUser.CurrentPassword) && string.IsNullOrWhiteSpace(updatedUser.NewPassword))
                return new ResultModel { Result = true, Message = "Nothing updated!" };

            var updateResult = await _userRepo.UpdateUser(new UpdatedUser{ Name = updatedUser.NewName, CurrentPassword = updatedUser.CurrentPassword, NewPassword = updatedUser.NewPassword}, UserId);
            if (updateResult.Result)
                return new ResultModel { Result = true, Message = "User updated successfully!" };

            return new ResultModel { Result = false, Message = updateResult.Message };
        }

        public async Task<ResultModel> DeleteAccount(int UserId)
        {
            var deleteResult = await _userRepo.DeleteUser(UserId);
            if (deleteResult.Result)
                return new ResultModel { Result = true, Message = "User with Id (" + UserId + ") deleted successfully!" };

            return new ResultModel { Result = false, Message = deleteResult.Message };
        }
    }
}