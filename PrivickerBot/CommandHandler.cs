using PrivickerBot.Repositories;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using PrivickerBot.DAL.Enums;
using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;

namespace PrivickerBot
{
    public class CommandHandler
    {
        private readonly HabitRepository _habitRepository;
        private readonly UserRepository _userRepository;
        HabitEditModel habitEditModel;

        public CommandHandler()
        {
            _habitRepository = new HabitRepository(new HabitContext());
            _userRepository = new UserRepository(new HabitContext());
        }

        public async Task ParseAndHandleAsync(Message msg)
        {
            DAL.Models.User user;
            if (!_userRepository.UserExist(msg.From.Id).Result)
            {
                await _userRepository.CreateUser(msg.From.Id, msg.From.FirstName + " " + msg.From.LastName);
            }

            user = await _userRepository.GetUser(msg.From.Id);

            if(msg.Text == "Просмотреть список")
            {
                await Program._botClient.SendTextMessageAsync(msg.Chat.Id, "Список привычек:", replyMarkup: Helpers.GenerateList(_habitRepository.GetHabitList(user.Id)));
            }


            switch (user.ChatState)
            {
                case ChatState.EditingNewHabit:
                    if (msg.Text == "Отмена")
                    {
                        user.ChatState = ChatState.Main;
                        _userRepository.UpdateUser(user);
                        await Helpers.ShowMainMenu(msg.Chat.Id);
                        return;
                    }
                    //делать if else проверку на название кнопок и сразу переключать switchcase, или же лучше вынести это в callback вызовы?
                    switch (user.EditingHabitState)
                    {
                        case EditingHabitState.Main:
                            ShowMainEditMenu(msg.Chat.Id);
                            break;
                        case EditingHabitState.EditingName:
                            break;
                        case EditingHabitState.EditingDescription:
                            break;
                        case EditingHabitState.EditingNotificationTime:
                            break;
                        case EditingHabitState.EditingPeriod:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            _userRepository.UpdateUser(user);
        }

        public async void ShowMainEditMenu(long id)
        {
            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();

            Dictionary<string, string> textCallbackPairs = new Dictionary<string, string>
            {
                {"Изменить название", "/EdName"},
                {"Изменить описание", "/EdDescription" },
                {"Изменить частоту", "/EdPeriod" },
                {"Изменить время напоминалки", "/EdNotification" },
                {"Сохранить", "/Save" },
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

            await Program._botClient.SendTextMessageAsync(id, habitEditModel.ToString(), replyMarkup: new InlineKeyboardMarkup(keyboard));
        }
    }
}
