using PrivickerBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrivickerBot.Repositories
{
    public class HabitRepository
    {
        private readonly HabitContext _context;

        public HabitRepository()
        {
            _context = new HabitContext(); //rework with DI, right know dk best way to pass serviceProvider deeper;
        }

        public string[] GetList(int UserId)
        {
            return _context.Habits.Where(h => h.UserId == UserId).Select(h => h.Id.ToString() + " " + h.Name).ToArray();
        }

        public void AddHabit(CreateHabitModel model)
        {
            Habit habit = new Habit
            {
                Name = model.Name,
                Description = model.Description,
                Period = model.Period,
                NotificationTime = model.NotificationTime,
                Started = DateTime.Now.Date,
                UserId = model.UserId
                
            };
            _context.Habits.Add(habit);
            _context.SaveChanges();
        }

        public Habit GetHabit(int id)
        {
            return _context.Habits.FirstOrDefault(h => h.Id == id);
        }

        public bool DeleteHabit(int id)
        {
            Habit habit = _context.Habits.First(h => h.Id == id);
            _context.Habits.Remove(habit);
            _context.SaveChanges();
            return true;
        }
    }
}
