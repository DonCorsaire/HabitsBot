using PrivickerBot.Models;
using PrivickerBot.Models.ViewModel;
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

        public List<HabitViewModel> GetHabitList(int UserId)
        {
            List<HabitViewModel> result = new List<HabitViewModel>();
            return result = _context.Habits.Where(h => h.UserId == UserId).Select(HabitViewModel.ProjectionExpression).ToList();
        }

        public string[] GetHabitStringsArray(int UserId)
        {
            return GetHabitList(UserId).Select(h => h.Name).ToArray();
        }

        public void AddHabit(HabitCreateModel model)
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

        public HabitViewModel GetHabit(int id)
        {
            Habit habit = _context.Habits.FirstOrDefault(h => h.Id == id);
            HabitViewModel viewModel = new HabitViewModel
            {
                Name = habit.Name,
                Description = habit.Description,
                NotificationTime = habit.NotificationTime,
                Period = habit.Period,
                Started = habit.Started
            };
            return viewModel;
        }

        public HabitEditModel GetEditHabitModel(int id)
        {
            Habit habit = _context.Habits.FirstOrDefault(h => h.Id == id);
            HabitEditModel result = new HabitEditModel
            {
                Id = habit.Id,
                Description = habit.Description,
                Name = habit.Name,
                NotificationTime = habit.NotificationTime,
                Period = habit.Period,
            };
            return result;
        }

        public void EditHabit(HabitEditModel model)
        {
            Habit habit = _context.Habits.FirstOrDefault(h => h.Id == model.Id);
            habit.Name = model.Name;
            habit.Description = model.Description;
            habit.NotificationTime = model.NotificationTime;
            _context.Entry(habit).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
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
