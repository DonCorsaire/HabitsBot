using Microsoft.EntityFrameworkCore;
using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivickerBot.Repositories
{
    public class HabitRepository
    {
        private readonly HabitContext _context;

        public HabitRepository(HabitContext dbContext)
        {
            _context = dbContext;
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

        public async Task AddHabit(HabitCreateModel model)
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
            await _context.SaveChangesAsync();
        }

        public async Task<HabitViewModel> GetHabit(int id)
        {
            Habit habit = await _context.Habits.FirstOrDefaultAsync(h => h.Id == id);
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

        public async Task<HabitEditModel> GetEditHabitModel(int id)
        {
            Habit habit = await _context.Habits.FirstOrDefaultAsync(h => h.Id == id);
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

        public async Task EditHabit(HabitEditModel model)
        {
            Habit habit = _context.Habits.FirstOrDefault(h => h.Id == model.Id);
            habit.Name = model.Name;
            habit.Description = model.Description;
            habit.NotificationTime = model.NotificationTime;
            _context.Entry(habit).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteHabit(int id)
        {
            Habit habit = await _context.Habits.FirstAsync(h => h.Id == id);
            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
