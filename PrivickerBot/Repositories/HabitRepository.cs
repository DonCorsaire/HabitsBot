using PrivickerBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PrivickerBot.Repositories
{
    class HabitRepository
    {
        readonly List<Habit> habits = new List<Habit>(); //replace with DB;

        static int currendId = 0;

        public string[] GetList()
        {
            string[] result = habits.Select(h =>h.Id.ToString()+ " " + h.Name).ToArray();
            return result;
        }

        public void AddHabit(CreateHabitModel model)
        {
            Habit habit = new Habit
            {
                Id = currendId,
                Name = model.Name,
                Description = model.Description,
                Period = model.Period,
                NotificationTime = model.NotificationTime,
                Started = DateTime.Now.Date,
                UserId = model.UserId
                
            };

            habits.Add(habit);
            currendId++;
        }

        public Habit GetHabit(int id)
        {
            return habits.FirstOrDefault(h => h.Id == id);
        }

        public bool DeleteHabit(int id)
        {
            Habit habit = habits.First(h => h.Id == id);
            habits.Remove(habit);
            return true;
        }

    }
}
