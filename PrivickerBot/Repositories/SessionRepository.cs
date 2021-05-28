using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace PrivickerBot.Repositories
{
    class SessionRepository
    {
        HabitContext _context;

        public SessionRepository(HabitContext dbContext)
        {
            _context = dbContext;
        }

        public async Task SetHabitName(string name, User user, bool adding = false)
        {
            if (adding)
            {
                AddEditSession model = new AddEditSession();
                model.UserId = user.Id;
                model.Name = name;
                _context.Sessions.Add(model);
            }
            else
            {
                AddEditSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
                model.Name = name;
                _context.Entry(model).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitDescription(string description, User user)
        {
            AddEditSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.Description = description;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitPeriod(int period, User user)
        {
            AddEditSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.Period = period;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitNotificationTime(DateTime notificationTime, User user)
        {
            AddEditSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.NotificationTime = notificationTime;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<HabitCreateModel> GetHabitCreateModel(User user)
        {
            AddEditSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            HabitCreateModel result = new HabitCreateModel
            {
                Name = model.Name,
                Description = model.Description,
                Period = model.Period,
                NotificationTime = model.NotificationTime,
                UserId = user.Id,
                Started = DateTime.Now
            };
            return result;
        }

    }
}
