using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PrivickerBot.Models.ViewModel
{
    public class HabitViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Started { get; set; }
        public int Period { get; set; }
        public DateTime NotificationTime { get; set; }

        public static Expression<Func<Habit, HabitViewModel>> ProjectionExpression = h => new HabitViewModel
        {
            Name = h.Name,
            Description = h.Description,
            NotificationTime = h.NotificationTime,
            Period = h.Period,
            Started = h.Started,
            Id = h.Id
        };


        public override string ToString()
        {
            return string.Format("Id:{5}. {0}. {1}. Repeats every {2} days. Notify at {3}, added at {4}", Name, Description, Period.ToString(), NotificationTime.ToShortTimeString(), Started.Date.ToShortDateString(), Id);
        }

    }
}
