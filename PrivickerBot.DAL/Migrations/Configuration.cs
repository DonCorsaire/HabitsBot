using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Text;
using PrivickerBot.DAL.Models;

namespace PrivickerBot.DAL.Migration
{
    public class Configuration : DbMigrationsConfiguration<HabitContext>
    {
        public Configuration()
        {

        }

        protected override void Seed(HabitContext context)
        {
            User user = new User { FromId = 192064694, AddingHabitState = 0, ChatState = 0, Name = "Safarali Aslanbekov" };
            context.Users.Add(user);
            List<Habit> habits = new List<Habit>
            {
                new Habit{  Name = "Физ-подготовка",
                            Description = "Заниматься регулярно, иначе в один день не хватит сил встать со стула",
                            NotificationTime = DateTime.Today,
                            Period = 2,
                            Started = DateTime.Today,
                            UserId = 1},
                new Habit{  Name = "Гитара",
                            Description = "Хватит только слушать, надо бы еще и играть научиться",
                            NotificationTime = DateTime.Today,
                            Period = 52,
                            Started = DateTime.Today,
                            UserId = 1},
                new Habit{  Name = "Французский",
                            Description = "Чтобы на вопрос 'Парле ву франсе' ты мог ответить не 'Нон'",
                            NotificationTime = DateTime.Today,
                            Period = 1,
                            Started = DateTime.Today,
                            UserId = 1},
             };
            context.Habits.AddRange(habits);
            context.SaveChanges();
        }
    }
}