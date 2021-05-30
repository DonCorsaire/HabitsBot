using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace PrivickerBot
{
    public static class Helpers
    {
        public static InlineKeyboardMarkup GenerateList(List<HabitViewModel> habitViewModels)
        {
            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();

            foreach (HabitViewModel habit in habitViewModels)
            {
                InlineKeyboardButton button = new InlineKeyboardButton
                {
                    Text = habit.Name,
                    CallbackData = "/Get " + habit.Id
                };
                keyboard.Add(new List<InlineKeyboardButton>() { button });
            }
            return new InlineKeyboardMarkup(keyboard);
        }

        public static ReplyKeyboardMarkup GetKeyboard(string[] values)
        {
            List<KeyboardButton> keyboardButtons = new List<KeyboardButton>();

            for (int i = 0; i < values.Length; i++)
            {
                keyboardButtons.Add(new KeyboardButton(values[i]));
            }

            ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            return replyKeyboard;
        }

        public static async Task ShowMainMenu(long UserFromId)
        {
            ReplyKeyboardMarkup replyKeyboard = GetKeyboard(new string[] { "Просмотреть список", "Добавить привычку" });


            await Program._botClient.SendTextMessageAsync(UserFromId, "Привет, выбери действие", replyMarkup: replyKeyboard);
        }


        public static async Task ShowMainEditMenu(long UserFromId, InputSession inputSession)
        {
            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();

            Dictionary<string, string> textCallbackPairs = new Dictionary<string, string>
            {
                {"Изменить название", "/edname"},
                {"Изменить описание", "/eddescription" },
                {"Изменить частоту", "/edperiod" },
                {"Изменить время напоминалки", "/ednotification" },
                {"Сохранить", "/save" },
                {"Отмена", "/Cancel" },
            };


            foreach (var item in textCallbackPairs)
            {
                InlineKeyboardButton button = new InlineKeyboardButton
                {
                    Text = item.Key,
                    CallbackData = item.Value
                };

                keyboard.Add(new List<InlineKeyboardButton>() { button });
            }

            string session = "Текущее имя привычки: " + inputSession.Name
                            + "\nТекущее описание: " + inputSession.Description
                            + "\nПериодичность: " + inputSession.Period.ToString()
                            + "\nВремя для напоминания: " + inputSession.NotificationTime.ToShortTimeString();

            await Program._botClient.SendTextMessageAsync(UserFromId, session, replyMarkup: new InlineKeyboardMarkup(keyboard));
        }


    }
}
