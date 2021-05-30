using Microsoft.EntityFrameworkCore;
using System;

namespace PrivickerBot.DAL.Models
{
    public class HabitContext : DbContext
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<InputSession> Sessions { get; set; }

        public HabitContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=HabitContext;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { FromId = 192064694, AddingHabitState = 0, ChatState = 0, Name = "Safarali Aslanbekov", Id = 1});
            modelBuilder.Entity<InputSession>().HasData(new InputSession { UserId = 1, Id = 1});
            modelBuilder.Entity<Habit>().HasData
                (
                    new Habit[]
                    {
                        new Habit{  Name = "Физ-подготовка",
                                Description = "Заниматься регулярно, иначе в один день не хватит сил встать со стула",
                                NotificationTime = DateTime.Today,
                                Period = 2,
                                Started = DateTime.Today,
                                UserId = 1,
                                Id = 1},
                        new Habit{  Name = "Гитара",
                                Description = "Хватит только слушать, надо бы еще и играть научиться",
                                NotificationTime = DateTime.Today,
                                Period = 52,
                                Started = DateTime.Today,
                                UserId = 1,
                                Id = 2},
                        new Habit{  Name = "Французский",
                                Description = "Чтобы на вопрос 'Парле ву франсе' ты мог ответить не 'Нон'",
                                NotificationTime = DateTime.Today,
                                Period = 1,
                                Started = DateTime.Today,
                                UserId = 1,
                                Id = 3},
                    }
                );




        }


    }
}
