using System.Data.Entity;

namespace PrivickerBot.DAL.Models
{
    public class HabitContext : DbContext
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<InputSession> Sessions { get; set; }

        public HabitContext() : base("HabitContext")
        {

        }
    }
}
