using Microsoft.EntityFrameworkCore;
using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivickerBot.Repositories
{
    class SessionRepository
    {
        private readonly HabitContext _context;

        public SessionRepository(HabitContext dbContext)
        {
            _context = dbContext;
        }

        public async Task CreateInputSession(User user)
        {
            InputSession model = new InputSession {  UserId = user.FromId};
            _context.Sessions.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<InputSession> GetEditSession(User user, HabitEditModel editModel)
        {
            InputSession session = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);

            session.HabitId = editModel.Id;
            session.Name = editModel.Name;
            session.LastExercise = editModel.LastExercise;
            session.Description = editModel.Description;
            session.NotificationTime = session.NotificationTime;
            session.Period = session.Period;
            session.Started = session.Started;

            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return session;
        }


        public async Task SetHabitName(string name, User user, bool adding = false)
        {
            InputSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (adding)
            {
                model.HabitId = default;
                model.Description = default;
                model.LastExercise = default;
                model.NotificationTime = default;
                model.Period = default;
                model.Started = default;
            }
            model.Name = name;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitDescription(string description, User user)
        {
            InputSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.Description = description;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitPeriod(int period, User user)
        {
            InputSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.Period = period;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SetHabitNotificationTime(DateTime notificationTime, User user)
        {
            InputSession model =  await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            model.NotificationTime = notificationTime;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<HabitCreateModel> GetHabitCreateModel(User user)
        {
            InputSession model = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
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
