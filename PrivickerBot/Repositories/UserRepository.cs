using PrivickerBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PrivickerBot.Repositories
{
    public class UserRepository
    {
        private readonly HabitContext _context;

        public UserRepository()
        {
            _context = new HabitContext(); //rework with DI, right know dk best way to pass serviceProvider deeper;
        }

        public User GetUser(int ChatId)
        {
            return _context.Users.FirstOrDefault(u => u.ChatId == ChatId);
            
        }

        public bool UserExist(int ChatId)
        {
            User result = _context.Users.FirstOrDefault(u => u.ChatId == ChatId);
            return result != null;
        }

        public void CreateUser(int ChatId, string name)
        {
            User user = new User
            {
                ChatId = ChatId,
                Name = name,
                AddingHabitState = Enums.AddingHabitState.NameInput,
                ChatState = Enums.ChatState.Main
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User userUpdated)
        {
            User oldUser = _context.Users.FirstOrDefault(u => u.ChatId == userUpdated.ChatId);
            oldUser.Name = userUpdated.Name;
            oldUser.ChatState = userUpdated.ChatState;
            oldUser.AddingHabitState = userUpdated.AddingHabitState;
            _context.Entry(oldUser).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            //implement Db and set entrystate modified for user. 
        }
    }
}
