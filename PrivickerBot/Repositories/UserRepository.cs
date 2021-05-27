using PrivickerBot.DAL.Enums;
using PrivickerBot.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace PrivickerBot.Repositories
{
    public class UserRepository
    {
        private readonly HabitContext _context;
        public UserRepository(HabitContext dbContext)
        {
            _context = dbContext;
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
                AddingHabitState = AddingHabitState.NameInput,
                EditingHabitState = EditingHabitState.Main,
                ChatState = ChatState.Main
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
            oldUser.EditingHabitState = userUpdated.EditingHabitState;
            _context.Entry(oldUser).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
