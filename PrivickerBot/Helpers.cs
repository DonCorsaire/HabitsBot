﻿using PrivickerBot.Models.ViewModel;
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

        public static async Task ShowMainMenu(long id)
        {
            ReplyKeyboardMarkup replyKeyboard = GetKeyboard(new string[] { "Просмотреть список", "Добавить новую привычку" });


            await Program._botClient.SendTextMessageAsync(id, "Привет, выбери действие", replyMarkup: replyKeyboard);
        }


    }
}