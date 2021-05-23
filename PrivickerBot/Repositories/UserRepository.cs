using PrivickerBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PrivickerBot.Repositories
{
    class UserRepository
    {
        readonly List<User> _users = new List<User>();

        public User GetUser(int ChatId)
        {
            return _users.FirstOrDefault(u => u.ChatId == ChatId);
            
        }

        public bool UserExist(int ChatId)
        {
            User result = _users.FirstOrDefault(u => u.ChatId == ChatId);
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

            _users.Add(user);
        }

        public void UpdateUser(User userUpdated)
        {
            User oldUser = _users.FirstOrDefault(u => u.ChatId == userUpdated.ChatId);
            int index = _users.IndexOf(oldUser);
            _users[index] = userUpdated;
            //implement Db and set entrystate modified for user. 
        }
    }
}
