﻿using PrivickerBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.Models
{
    class User
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Name { get; set; }
        public AddingHabitState AddingHabitState { get; set; }

        public ChatState ChatState { get; set; }
    }
}