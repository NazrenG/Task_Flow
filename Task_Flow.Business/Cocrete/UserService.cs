using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class UserService : IUserService
    {
        private readonly IUserDal dal;

        public UserService(IUserDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(CustomUser user)
        {
            await dal.Add(user);
        }

        public async Task Delete(CustomUser user)
        {
            await dal.Delete(user);
        }

        public async Task<CustomUser> GetUserById(string id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<CustomUser>> GetUsers()
        {
            return await dal.GetAll();
        }
 

        public async Task Update(CustomUser user)
        {
            await dal.Update(user);
        }

        public async Task<bool> UserExists(string username)
        {
            var hasExist = await dal.GetById(f => f.UserName == username);
            return hasExist != null ? true : false;
        }

        public async Task<int> GetAllUserCount()
        {
            var list = await dal.GetAll();
            return list.Count;
        }

        public async Task<bool> CheckUsernameOrEmail(string nameOrEmail)
        {
            var users=await dal.GetAll(c=>c.Email==nameOrEmail );
            return users.Any(); 
        }

        public async Task<CustomUser>GetOneUSerByUsername(string username)
        {
            var allUsers = await dal.GetById(u=>u.UserName==username);
            return allUsers;
        }

        public async Task<List<CustomUser>> GetUserByName(string name)
        {
            var allUsers = await dal.GetAll();
            var sorted = allUsers
                .Where(u => u.UserName.Contains(name))
                .OrderBy(u => !u.UserName.StartsWith(name)) 
                .ThenBy(u => u.UserName).ToList();
            return sorted;

        }

        //public async Task<string> GetProfilePhotoAsync(string id)
        //{return (await dal.GetById(u=>u.Id==id)).Image;
        //}
    }
}