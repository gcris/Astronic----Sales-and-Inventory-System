using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;
using System.Data.Entity;
using InventoryServices.Models;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<bool> Save(UserDtos userDtos)
        {
            var dbContext = new InventoryDbContext();

            var user = userDtos.AsUser();

            user.UserRole = await FindUserRole(userDtos.UserRole, dbContext);

            var queryUser = await FindUser(userDtos.UserId, dbContext);

            if (queryUser == null) dbContext.Users.Add(user);
            else
            {
                queryUser.FirstName = user.FirstName;

                queryUser.LastName = user.LastName;

                queryUser.Username = user.Username;

                queryUser.Password = user.Password;

                queryUser.ContactNumber = user.ContactNumber;

                queryUser.Address = user.Address;

                queryUser.IsEnable = user.IsEnable;

                queryUser.UserRole = user.UserRole;

                dbContext.Entry(queryUser).State = EntityState.Modified;
            }

            dbContext.PasswordHistories.Add(new Models.PasswordHistory
            {
                Date = DateTime.Now,
                Password = user.Password,
                User = queryUser == null ? user : queryUser
            });

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateAccount(UserDtos userDtos)
        {
            var dbContext = new InventoryDbContext();

            var user = userDtos.AsUser();

            user.UserRole = await FindUserRole(userDtos.UserRole, dbContext);

            var queryUser = await FindUser(userDtos.UserId, dbContext);

            if (queryUser != null)
            {
                queryUser.FirstName = user.FirstName;

                queryUser.LastName = user.LastName;

                queryUser.Username = user.Username;

                queryUser.ContactNumber = user.ContactNumber;

                queryUser.Address = user.Address;

                dbContext.Entry(queryUser).State = EntityState.Modified;
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ChangePassword(int id, string password)
        {
            var dbContext = new InventoryDbContext();

            var queryUser = await FindUser(id, dbContext);

            if (queryUser != null)
            {
                queryUser.Password = password;

                dbContext.Entry(queryUser).State = EntityState.Modified;

                dbContext.PasswordHistories.Add(new Models.PasswordHistory
                {
                    Date = DateTime.Now,
                    Password = password,
                    UserId = queryUser.Id
                });
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> PasswordExists(int id, string password)
        {
            var dbContext = new InventoryDbContext();

            var passwordHistories = await FindPasswordHistories(id, dbContext);

            return passwordHistories.Any(history => history.Password == password);
        }

        public async Task<bool> EnableUser(int id = 0, bool enable = false)
        {
            var dbContext = new InventoryDbContext();

            var user = await FindUser(id, dbContext);

            if (user != null)
            {
                user.IsEnable = enable;

                dbContext.Entry(user).State = EntityState.Modified;
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<UserDtos> FindUserDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var user = await FindUser(id, dbContext);

            if (user == null) return null;

            return user.AsUserDtos();
        }

        public async Task<UserDtos> FindUserDtos(string username)
        {
            var dbContext = new InventoryDbContext();

            var user = await FindUser(username, dbContext);

            if (user == null) return null;

            return user.AsUserDtos();
        }

        public async Task<UserDtos> LogIn(string username, string password)
        {
            var dbContext = new InventoryDbContext();

            var queryUser = await FindUser(username, password, dbContext);

            if (queryUser == null) return null;

            if (queryUser.UserRole.RoleName != "SystemDeveloper")
            {
                if (!queryUser.IsEnable) return null;

                var proceedLogIn = queryUser.UserRole.UserPrivilages.Any(user => user.Action == "Log In" && user.IsEnable);

                if (!proceedLogIn) return null;
            }

            return queryUser.AsUserDtos();
        }

        public async Task<IEnumerable<UserDtos>> GetAll(string key = "")
        {
            var dbContext = new InventoryDbContext();

            var users = await dbContext.Users
                .Include(user => user.UserRole)
                .Include(user => user.UserRole.UserPrivilages)
                .Include(user => user.PasswordHistories)
                .Where(user => user.Username.Contains(key) ||
                    user.LastName.Contains(key) ||
                    user.FirstName.Contains(key) ||
                    user.ContactNumber.Contains(key) ||
                    user.Address.Contains(key))
                .ToListAsync();

            return users.Select(user => user.AsUserDtos());
        }

        #region User Role
        public async Task<bool> SaveRole(UserRoleDtos userRoleDtos)
        {
            var dbContext = new InventoryDbContext();

            var role = userRoleDtos.AsUserRole();

            var queryRole = await FindUserRole(role.RoleName, dbContext);

            foreach (var privilegeDtos in userRoleDtos.UserPrivilegeDtosList)
            {
                var privilege = privilegeDtos.AsUserPrivilege();

                privilege.UserRole = queryRole == null ? role : queryRole;

                var queryPrivilege = await FindUserPrivilege(privilege.Id, dbContext);

                if (queryPrivilege != null)
                {
                    queryPrivilege.IsEnable = privilege.IsEnable;

                    dbContext.Entry(queryPrivilege).State = EntityState.Modified;
                }
                else dbContext.UserPrivileges.Add(privilege);
            }

            if (queryRole == null) dbContext.UserRoles.Add(role);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<UserRoleDtos>> GetAllRoles()
        {
            var dbContext = new InventoryDbContext();

            var queryList = await dbContext.UserRoles
                .Include(item => item.UserPrivilages)
                .Where(item => item.RoleName != "SystemDeveloper")
                .OrderByDescending(item => item.Id)
                .ToListAsync();

            return queryList.Select(item => item.AsUserRoleDtos());
        }

        public async Task<IEnumerable<UserPrivilegeDtos>> GetUserPrivileges(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryList = await dbContext.UserPrivileges
                .Include(item => item.UserRole)
                .Where(item => item.UserRoleId == id)
                .ToListAsync();

            return queryList.Select(item => item.AsUserPrivilegeDtos());
        }

        public async Task<IEnumerable<UserPrivilegeDtos>> GetUserPrivileges(string name)
        {
            var dbContext = new InventoryDbContext();

            var queryList = await dbContext.UserPrivileges
                .Include(item => item.UserRole)
                .Where(item => item.UserRole.RoleName == name)
                .ToListAsync();

            return queryList.Select(item => item.AsUserPrivilegeDtos());
        }
        #endregion

        #region User Activity
        public async Task<bool> SaveActivity(UserActivityDtos userActivityDtos)
        {
            var dbContext = new InventoryDbContext();

            var userActivity = userActivityDtos.AsUserActivity();

            userActivity.User = await FindUser(userActivityDtos.UserId, dbContext);

            if (userActivity.ItemId.HasValue)
            {
                if (userActivity.ItemId.Value > 0)
                {
                    userActivity.Item = await FindItem(userActivity.ItemId.Value, dbContext);

                    userActivity.CurrentStock = userActivity.Item.QuantityOnHand;
                }
                else userActivity.ItemId = null;
            }

            dbContext.UserActivities.Add(userActivity);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<UserActivityDtos>> GetAllActivities(int itemId = 0, string key = "") 
        {
            var dbContext = new InventoryDbContext();

            var query = await dbContext.UserActivities
                .Where(user => (user.User.FirstName.Contains(key) ||
                    user.User.LastName.Contains(key)) &&
                    itemId == 0 ? true : user.ItemId.HasValue && user.ItemId.Value == itemId)
                .OrderByDescending(user => user.Date)
                .ToListAsync();

            return query.Select(user => user.AsUserActivityDtos());
        }
        #endregion

        #region Helper Methods
        private async Task<User> FindUser(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Users
                .Include(user => user.UserRole)
                .Include(user => user.UserRole.UserPrivilages)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        private async Task<List<PasswordHistory>> FindPasswordHistories(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PasswordHistories
                .Where(history => history.UserId == id)
                .ToListAsync();
        }

        private async Task<User> FindUser(string username, InventoryDbContext dbContext)
        {
            return await dbContext.Users
                .Include(user => user.UserRole)
                .Include(user => user.UserRole.UserPrivilages)
                .FirstOrDefaultAsync(user => user.Username == username);
        }

        private async Task<User> FindUser(string username, string password, InventoryDbContext dbContext)
        {
            return await dbContext.Users
                .Include(user => user.UserRole)
                .Include(user => user.UserRole.UserPrivilages)
                .FirstOrDefaultAsync(user => user.Username == username && user.Password == password);
        }

        private async Task<UserPrivilege> FindPrivilege(string action, int id, InventoryDbContext dbContext)
        {
            return await dbContext.UserPrivileges
                .FirstOrDefaultAsync(item => item.Action == action && item.UserRoleId == id);
        }

        private async Task<UserRole> FindUserRole(string name, InventoryDbContext dbContext)
        {
            return await dbContext.UserRoles
                .Include(item => item.UserPrivilages)
                .FirstOrDefaultAsync(item => item.RoleName == name);
        }

        private async Task<UserPrivilege> FindUserPrivilege(int id, InventoryDbContext dbContext)
        {
            return await dbContext.UserPrivileges
                .Include(item => item.UserRole)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items.FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion  
    }
}
