using PrivickerBot.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using PrivickerBot.Models;
using PrivickerBot.Enums;

namespace PrivickerBot
{
    public class CommandHandler
    {
        private readonly HabitRepository _habitRepository;
        private readonly UserRepository _userRepository;
        CreateHabitModel habitModel;

        public CommandHandler(HabitRepository habitRepository, UserRepository userRepository)
        {
            _habitRepository = habitRepository;
            _userRepository = userRepository;
        }

        public async Task ParseAndHandleAsync(Message msg)
        {
            Models.User user;
            if (!_userRepository.UserExist(msg.From.Id))
            {
                _userRepository.CreateUser(msg.From.Id, msg.From.FirstName + " " + msg.From.LastName);
            }

            user = _userRepository.GetUser(msg.From.Id);

            switch (user.ChatState)
            {
                case ChatState.Main:

                    if (msg.Text == "Просмотреть список")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendJoin('\n', _habitRepository.GetList(user.Id));

                        ReplyKeyboardMarkup replyKeyboard = GetKeyboard(new string[] { "Просмотреть список", "Добавить новую привычку" });

                        await Program.client.SendTextMessageAsync(msg.Chat.Id, "Список привычек:\n" + sb.ToString() + "\n", replyMarkup: replyKeyboard);
                    }
                    else if (msg.Text == "Добавить новую привычку")
                    {
                        user.ChatState = ChatState.AddingNewHabit;
                        user.AddingHabitState = AddingHabitState.NameInput;
                        _userRepository.UpdateUser(user);
                        habitModel = new CreateHabitModel();

                        await Program.client.SendTextMessageAsync(msg.Chat.Id, "Для создания привычки следуйте инструкциям:\nВведите название привычки", replyMarkup: GetKeyboard(new string[] { "Отмена" }));
                    }
                    else
                    {
                        ShowMainMenu(msg.Chat.Id);
                    }

                    break;
                case ChatState.AddingNewHabit:

                    if (msg.Text == "Отмена")
                    {
                        habitModel = new CreateHabitModel();
                        user.ChatState = ChatState.Main;
                        _userRepository.UpdateUser(user);
                        ShowMainMenu(msg.Chat.Id);
                        return;
                    }
                    switch (user.AddingHabitState)
                    {
                        case AddingHabitState.NameInput:
                            habitModel.Name = msg.Text;
                            user.AddingHabitState = AddingHabitState.DescriptionInput;
                            await Program.client.SendTextMessageAsync(msg.Chat.Id,
                                "Введите описание привычки",
                                replyMarkup: GetKeyboard(new string[] { "Отмена" }));
                            break;
                        case AddingHabitState.DescriptionInput:
                            habitModel.Description = msg.Text;
                            user.AddingHabitState = AddingHabitState.PeriodInput;
                            await Program.client.SendTextMessageAsync(msg.Chat.Id,
                                "Введите периодичность занятия(в днях)",
                                replyMarkup: GetKeyboard(new string[] { "Отмена" }));
                            break;
                        case AddingHabitState.PeriodInput:
                            if (int.TryParse(msg.Text, out int value))
                            {
                                habitModel.Period = value;
                                user.AddingHabitState = AddingHabitState.NotificationTimeInput;
                                await Program.client.SendTextMessageAsync(msg.Chat.Id,
                                    "Введите время для уведомления",
                                    replyMarkup: GetKeyboard(new string[] { "Отмена" }));
                            }
                            else
                            {
                                await Program.client.SendTextMessageAsync(msg.Chat.Id,
                                    "Ошибка чтения, введите периодичность занятия(в днях)",
                                    replyMarkup: GetKeyboard(new string[] { "Отмена" }));
                            }
                            break;
                        case AddingHabitState.NotificationTimeInput:
                            habitModel.NotificationTime = DateTime.Now;
                            user.AddingHabitState = AddingHabitState.Accepting;
                            await Program.client.SendTextMessageAsync(msg.Chat.Id,
                                "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                                replyMarkup: GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                            break;
                        case AddingHabitState.Accepting:
                            if(msg.Text == "Подтвердить")
                            {
                                habitModel.UserId = user.Id;
                                _habitRepository.AddHabit(habitModel);
                                user.ChatState = ChatState.Main;
                                ShowMainMenu(msg.Chat.Id);
                            }
                            else
                            {
                                await Program.client.SendTextMessageAsync(msg.Chat.Id,
                               "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                               replyMarkup: GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                            }

                            
                            break;
                        default:
                            _userRepository.UpdateUser(user);
                            break;

                    }

                    break;
                case ChatState.EditingNewHabit:
                    break;
                default:
                    break;
            }
        }

        public ReplyKeyboardMarkup GetKeyboard(string[] values)
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

        public async void ShowMainMenu(long id)
        {
            ReplyKeyboardMarkup replyKeyboard = GetKeyboard(new string[] { "Просмотреть список", "Добавить новую привычку" });


            await Program.client.SendTextMessageAsync(id, "Привет, выбери действие", replyMarkup: replyKeyboard);
        }

        public string ParseCommand(string msg)
        {
            if (msg.StartsWith("/delete"))
            {
                _habitRepository.DeleteHabit(int.Parse(msg.Split(' ')[1]));
                return "Habit " + msg.Split(' ')[1] + " removed.";
            }
            else if (msg.StartsWith("/check"))
            {
                var result = _habitRepository.GetHabit(int.Parse(msg.Split(' ')[1]));
                return result.ToString();
            }
            else if (msg.StartsWith("/edit"))
            {
                return "not implemented";
            }
            else if (msg.StartsWith("/help"))
            {
                return "/add Habit_name habit_description Repeat_days Notification_time \n/list \n/delete Habit_id";
            }
            else
            {
                return "not implemented";
            }
        }
    }
}
