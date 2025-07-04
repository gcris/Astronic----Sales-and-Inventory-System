using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Save(UserDtos userDtos);
        Task<bool> UpdateAccount(UserDtos userDtos);
        Task<bool> PasswordExists(int id, string password);
        Task<bool> ChangePassword(int id, string password);
        Task<bool> EnableUser(int id = 0, bool enable = false);
        Task<UserDtos> LogIn(string username, string password);
        Task<IEnumerable<UserDtos>> GetAll(string key = "");
        Task<UserDtos> FindUserDtos(int id);
        Task<UserDtos> FindUserDtos(string username);

        Task<bool> SaveRole(UserRoleDtos userRoleDtos);
        Task<IEnumerable<UserRoleDtos>> GetAllRoles(); 
        Task<IEnumerable<UserPrivilegeDtos>> GetUserPrivileges(int id);
        Task<IEnumerable<UserPrivilegeDtos>> GetUserPrivileges(string name);

        Task<bool> SaveActivity(UserActivityDtos userActivityDtos);
        Task<IEnumerable<UserActivityDtos>> GetAllActivities(int itemId = 0, string key = "");
    }
}
