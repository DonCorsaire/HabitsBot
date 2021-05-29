using System;
using System.Collections.Generic;
using System.Text;

namespace PrivickerBot.Models.ViewModel
{
    public class HabitEditModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Period { get; set; }
        public DateTime NotificationTime { get; set; }
        
        public DateTime LastExercise { get; set; }

        public override string ToString()
        {
            string result = "Текущее имя привычки: " + Name 
                            + "\nТекущее описание: " + Description
                            + "\nПериодичность: " + Period.ToString()
                            + "\nВремя для напоминания: " + NotificationTime.ToShortTimeString();
            return result;
        }
    }
}
