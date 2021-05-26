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
        HabitCreateModel habitModel;

        public CommandHandler(HabitRepository habitRepository, UserRepository userRepository)
        {
            _habitRepository = habitRepository;
            _userRepository = userRepository;
        }

        public async Task ParseAndHandleAsync(Message msg)
        {
            DAL.Models.User user;
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
                        //StringBuilder sb = new StringBuilder();
                        //sb.AppendJoin('\n', _habitRepository.GetHabitStringsArray(user.Id));
                        //ReplyKeyboardMarkup replyKeyboard = GetKeyboard(new string[] { "Просмотреть список", "Добавить новую привычку" });
                        //await Program.client.SendTextMessageAsync(msg.Chat.Id, "Список привычек:\n" + sb.ToString() + "\n", replyMarkup: replyKeyboard);

                        await Program.client.SendTextMessageAsync(msg.Chat.Id, "Список привычек:", replyMarkup: GenerateList(_habitRepository.GetHabitList(user.Id)));
                    }
                    else if (msg.Text == "Добавить новую привычку")
                    {
                        user.ChatState = ChatState.AddingNewHabit;
                        user.AddingHabitState = AddingHabitState.NameInput;
                        _userRepository.UpdateUser(user);
                        habitModel = new HabitCreateModel();

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
                        habitModel = new HabitCreateModel();
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
                            if (msg.Text == "Подтвердить")
                            {
                                user.ChatState = ChatState.Main;
                                habitModel.UserId = user.Id;
                                _habitRepository.AddHabit(habitModel);
                                ShowMainMenu(msg.Chat.Id);
                            }
                            else
                            {
                                user.ChatState = ChatState.Main;
                                await Program.client.SendTextMessageAsync(msg.Chat.Id,
                               "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                               replyMarkup: GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ChatState.EditingNewHabit:

                    switch (user.EditingHabitState)
                    {
                        case EditingHabitState.Main:
                            ReplyKeyboardMarkup keyboard = GetKeyboard(new string[] { "Название", "Описание", "Время уведомления", "Периодичность", "Сохранить", "Отмена" });
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

        public async Task HandleCallbackAsync(CallbackQuery callback)
        {
            if (callback.Data.StartsWith("/Get"))
            {
                int id = int.Parse(callback.Data.Replace("/Get ", ""));

                InlineKeyboardButton editBtn = new InlineKeyboardButton
                {
                    CallbackData = "/Edit " + id,
                    Text = "Изменить"
                };

                InlineKeyboardButton deleteBtn = new InlineKeyboardButton
                {
                    CallbackData = "/Delete " + id,
                    Text = "Удалить"
                };

                await Program.client.SendTextMessageAsync(callback.From.Id,
                                                          _habitRepository.GetHabit(id).ToString(),
                                                          replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { editBtn, deleteBtn }));
            }
            else if (callback.Data.StartsWith("/Delete"))
            {
                int id = int.Parse(callback.Data.Replace("/Delete ", ""));

                InlineKeyboardButton yesBtn = new InlineKeyboardButton
                {
                    CallbackData = "/Remove " + id,
                    Text = "Да"
                };

                InlineKeyboardButton noBtn = new InlineKeyboardButton
                {
                    CallbackData = "/Get " + id,
                    Text = "Нет"
                };

                await Program.client.SendTextMessageAsync(callback.From.Id,
                                                          "Вы действительно хотите удалить привычку " + _habitRepository.GetHabit(id).Name + "?",
                                                          replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { yesBtn, noBtn }));

            }
            else if (callback.Data.StartsWith("/Remove"))
            {
                int id = int.Parse(callback.Data.Replace("/Remove ", ""));
                _habitRepository.DeleteHabit(id);
                ShowMainMenu(callback.From.Id);
            }

            else if (callback.Data.StartsWith("/Edit"))
            {
                int id = int.Parse(callback.Data.Replace("/Edit ", ""));

                await Program.client.SendTextMessageAsync(callback.From.Id,
                                                          "Редактируем " + _habitRepository.GetHabit(id).ToString());

            }
        }

        public InlineKeyboardMarkup GenerateList(List<HabitViewModel> habitViewModels)
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
           if (msg.StartsWith("/edit"))
            {
                return "not implemented";
            }
            else
            {
                return "not implemented";
            }
        }
    }
}
