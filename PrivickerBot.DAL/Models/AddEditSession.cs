using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.DAL.Models
{
    public class AddEditSession
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Started { get; set; }
        public int Period { get; set; }
        public DateTime LastExercise { get; set; }
        public DateTime NotificationTime { get; set; }
        public int UserId { get; set; }
    }
}
