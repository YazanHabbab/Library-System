using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using business_logic.DTOs;
using business_logic.Models;
using data_access.Helpers;
using data_access.Interfaces;
using data_access.Models;
using data_access.Models.Other;
using data_access.Repositories;

namespace business_logic.Services
{
    public class AccountService
    {
        private readonly IUserRepository _userRepo;

        public AccountService(IUserRepository userRepo)
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

        public async Task<List<User>> SearchUsers(string searchTerm, bool activeOnly)
        {
            var users = await GetAllUsers();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u =>
                    u.UserId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (activeOnly)
                users = users.Where(u => u.IsActive == activeOnly).ToList();

            return users.OrderBy(u => u.UserId).ToList();
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

        public async Task<LoginAccountVM> LoginAccount(UserLoginDto userLoginDto)
        {
            if (string.IsNullOrWhiteSpace(userLoginDto.Name) || string.IsNullOrWhiteSpace(userLoginDto.Password))
                return new LoginAccountVM { Result = false, Message = "Name and Password are required!" };

            var user = await _userRepo.GetUserByName(userLoginDto.Name);
            if (user is null)
                return new LoginAccountVM { Result = false, Message = "Your Credentilas are not correct!" };

            if (!user.IsActive)
                return new LoginAccountVM { Result = false, Message = "Account is disabled!" };

            var checkResult = PasswordHasher.Verify(userLoginDto.Password, user.Password);
            if (checkResult)
                return new LoginAccountVM { Result = true, Message = "Logged in successfully!", Name = user.Name, UserId = user.UserId };

            return new LoginAccountVM { Result = false, Message = "Your Credentilas are not correct!" };
        }

        public async Task<ResultModel> UpdateUserInfo(UpdatedUserDto updatedUser, int UserId)
        {
            if (string.IsNullOrWhiteSpace(updatedUser.NewName) && string.IsNullOrWhiteSpace(updatedUser.CurrentPassword) && string.IsNullOrWhiteSpace(updatedUser.NewPassword))
                return new ResultModel { Result = true, Message = "Nothing updated!" };

            var updateResult = await _userRepo.UpdateUser(new UpdatedUser { Name = updatedUser.NewName, CurrentPassword = updatedUser.CurrentPassword, NewPassword = updatedUser.NewPassword }, UserId);
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

        public async Task<ResultModel> ActivateAccount(int UserId)
        {
            var activateResult = await _userRepo.ActivateUser(UserId);
            if (activateResult.Result)
                return new ResultModel { Result = true, Message = "User with Id (" + UserId + ") activated successfully!" };

            return new ResultModel { Result = false, Message = activateResult.Message };
        }
    }
}