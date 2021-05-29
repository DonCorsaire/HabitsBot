using PrivickerBot.DAL.Enums;
using PrivickerBot.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PrivickerBot.Repositories
{
    public class UserRepository
    {
        private readonly HabitContext _context;
        public UserRepository(HabitContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<User> GetUser(int ChatId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FromId == ChatId);
            
        }

        public async Task<bool> UserExist(int ChatId)
        {
            User result = await _context.Users.FirstOrDefaultAsync(u => u.FromId == ChatId);
            return result != null;
        }

        public async Task<User> CreateUser(int ChatId, string name)
        {
            User user = new User
            {
                FromId = ChatId,
                Name = name,
                AddingHabitState = AddingHabitState.NameInput,
                EditingHabitState = EditingHabitState.Main,
                ChatState = ChatState.Main
            };

            User result = _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return result;
        }

        public void UpdateUser(User userUpdated)
        {
            User oldUser = _context.Users.FirstOrDefault(u => u.FromId == userUpdated.FromId);
            oldUser.Name = userUpdated.Name;
            oldUser.ChatState = userUpdated.ChatState;
            oldUser.AddingHabitState = userUpdated.AddingHabitState;
            oldUser.EditingHabitState = userUpdated.EditingHabitState;
            _context.Entry(oldUser).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
