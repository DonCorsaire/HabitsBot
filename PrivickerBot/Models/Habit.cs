using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.Models
{
    class Habit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime Started { get; set; }
        public TimeSpan Period { get; set; }
        public DateTime NotificationTime { get; set; }
        public int UserId { get; set; }

        public override string ToString()
        {
            return string.Format("Id:{5}. {0}. {1}. Repeats every {2} days. Notify at {3}, added at {4}", Name, Description, Period.TotalDays.ToString(), NotificationTime.ToShortTimeString(), Started.Date.ToShortDateString(), Id);
        }
    }
}
