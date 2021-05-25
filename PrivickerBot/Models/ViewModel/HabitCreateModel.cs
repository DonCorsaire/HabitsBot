using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.Models
{
    public class HabitCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime Started { get; set; }
        public int Period { get; set; }
        public DateTime NotificationTime { get; set; }
        public int UserId { get; set; }
    }
}
