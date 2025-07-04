using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Controllers
{
    public class UserController
    {
        private IUserRepository repository = new UserRepository();
        private ICryptologyRepository cryptRepository = new CryptologyRepository();

        public async Task<bool> Save(UserDtos userDtos)
        {
            userDtos.Username = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = userDtos.Username,
                UseHashing = true
            });

            userDtos.Password = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = userDtos.Password,
                UseHashing = true
            });

            return await repository.Save(userDtos);
        }

        public async Task<bool> UpdateAccount(UserDtos userDtos)
        {
            userDtos.Username = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = userDtos.Username,
                UseHashing = true
            });

            return await repository.UpdateAccount(userDtos);
        }

        public async Task<bool> EnableUser(int id, bool enable = true)
        {
            return await repository.EnableUser(id, enable);
        }

        public async Task<bool> ChangePassword(int id, string password)
        {
            password = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = password,
                UseHashing = true
            });

            return await repository.ChangePassword(id, password);
        }

        public async Task<UserDtos> FindUser(int id)
        {
            var userDtos = await repository.FindUserDtos(id);

            if (userDtos != null)
            {
                userDtos.Username = cryptRepository.DecryptString(new CryptographyDtos
                {
                    CipherString = userDtos.Username,
                    UseHashing = true
                });
            }

            return userDtos;
        }

        public async Task<UserDtos> FindUser(string username)
        {
            username = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = username,
                UseHashing = true
            });

            var userDtos = await repository.FindUserDtos(username);

            if (userDtos != null)
            {
                userDtos.Username = cryptRepository.DecryptString(new CryptographyDtos
                {
                    CipherString = userDtos.Username,
                    UseHashing = true
                });
            }

            return userDtos;
        }

        public async Task<bool> IsValidUsername(string newUsername, string currentUsername = null)
        {
            newUsername = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = newUsername,
                UseHashing = true
            });

            currentUsername = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = currentUsername,
                UseHashing = true
            });

            var users = await repository.GetAll();

            if (!string.IsNullOrWhiteSpace(currentUsername)) users = users.Where(user => user.Username != currentUsername);

            var exists = users.Any(user => user.Username == newUsername);

            return !exists;
        }

        public async Task<bool> IsValidPassword(int id, string password)
        {
            password = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = password,
                UseHashing = true
            });

            return await repository.PasswordExists(id, password);
        }

        public async Task<IEnumerable<UserDtos>> GetAll(string key = "")
        {
            var queryList = await repository.GetAll(key);

            if (queryList.Count() < 1) return queryList;

            queryList = queryList.Where(user => user.UserRole != "SystemDeveloper" && user.UserRole != "Admin");

            return queryList.Select(user => new UserDtos
            {
                Username = cryptRepository.DecryptString(new CryptographyDtos
                    {
                        CipherString = user.Username,
                        UseHashing = true
                    }),
                UserRole = user.UserRole,
                UserId = user.UserId,
                Address = user.Address,
                ContactNumber = user.ContactNumber,
                FirstName = user.FirstName,
                IsEnable = user.IsEnable,
                LastName = user.LastName
            });
        }

        public async Task<IEnumerable<UserRoleDtos>> GetAllRoles()
        {
            return await repository.GetAllRoles();
        }

        public async Task<UserDtos> LogIn(string username, string password)
        {
            username = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = username,
                UseHashing = true
            });

            password = cryptRepository.EncryptString(new CryptographyDtos
            {
                ToEncryptString = password,
                UseHashing = true
            });

            var userDtos = await repository.LogIn(username, password);

            if (userDtos != null)
            {
                userDtos.Username = cryptRepository.DecryptString(new CryptographyDtos
                {
                    CipherString = userDtos.Username,
                    UseHashing = true
                });
            }

            return userDtos;
        }

        #region User role
        public async Task<bool> SaveUserRole(UserRoleDtos userRoleDtos)
        {
            return await repository.SaveRole(userRoleDtos);
        }

        public async Task<IEnumerable<UserPrivilegeDtos>> GetPrivileges(int id)
        {
            return await repository.GetUserPrivileges(id);
        }

        public async Task<IEnumerable<UserPrivilegeDtos>> GetPrivileges(string name)
        {
            return await repository.GetUserPrivileges(name);
        }
        #endregion

        #region User Activities
        public async Task<bool> SaveItemActivity(UserActivityDtos userActivityDtos)
        {
            userActivityDtos.Transaction = true;

            return await repository.SaveActivity(userActivityDtos);
        }
        public async Task<bool> SaveActivity(string action, int userId)
        {
            var userActivityDtos = new UserActivityDtos
            {
                Action = action,
                Date = DateTime.Now,
                UserId = userId
            };

            return await repository.SaveActivity(userActivityDtos);
        }

        public async Task<IEnumerable<UserActivityDtos>> GetActivities(int itemId, DateTime from, DateTime to, string key = "", bool transaction = false)
        {
            var queryList = await repository.GetAllActivities(itemId, key);

            if (from > DateTime.MinValue && to > DateTime.MinValue)
                queryList = queryList.Where(item => item.Date.Date >= from.Date && item.Date.Date <= to.Date);

            if (transaction) queryList = queryList.Where(item => item.Transaction);

            return queryList;
        }
        #endregion
    }
}
