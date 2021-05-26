using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;

namespace PrivickerBot.DAL.Models
{
    public class HabitContext : DbContext
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<User> Users { get; set; }

        public HabitContext() : base("HabitContext")
        {

        }
    }
}
