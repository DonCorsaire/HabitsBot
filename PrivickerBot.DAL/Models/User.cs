using PrivickerBot.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public int FromId { get; set; }
        public string Name { get; set; }
        public AddingHabitState AddingHabitState { get; set; }
        public EditingHabitState EditingHabitState { get; set; }
        public ChatState ChatState { get; set; }
    }
}
