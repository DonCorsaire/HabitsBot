using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using PrivickerBot.Repositories;
using PrivickerBot.DAL.Enums;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PrivickerBot.Services
{
    public class MessageService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserRepository _userRepository;
        private readonly HabitRepository _habitRepository;

        HabitCreateModel habitCreateModel;

        public MessageService(ITelegramBotClient botClient, HabitContext dbContext)
        {
            _botClient = botClient;
            _userRepository = new UserRepository(dbContext);
            _habitRepository = new HabitRepository(dbContext);
        }

        public async Task HandleMessage(Message message)
        {
            DAL.Models.User user = _userRepository.GetUser(message.From.Id);
            //TO-DO check user exist or not;

            switch (user.ChatState)
            {
                case ChatState.Main:
                    await MainMenuHandle(message, user);
                    break;
                case ChatState.AddingNewHabit:
                    await AddingHabitHandle(message, user);
                    break;
                case ChatState.EditingNewHabit:
                    break;
                default:
                    break;
            }


            /* если ChatState ==2
             *    идти по switchCase EditingState
             * 
             * как вариант habitCreateModel и habitEditModel хранить в User'e, либо добавить еще одну строчку как TestSessions.
             */

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ответка через MessageService");
        }

        async Task MainMenuHandle(Message message, DAL.Models.User user)
        {
            if (message.Text == "Просмотреть список")
            {
                await _botClient.SendTextMessageAsync(message.From.Id,
                                                        "Список привычек:",
                                                        replyMarkup: Helpers.GenerateList(_habitRepository.GetHabitList(user.Id)));
            }
            else if (message.Text == "Добавить привычку")
            {
                user.ChatState = ChatState.AddingNewHabit;
                habitCreateModel = new HabitCreateModel();
                _userRepository.UpdateUser(user);
                await _botClient.SendTextMessageAsync(message.From.Id,
                                                        "Введите название привычки:",
                                                        replyMarkup: Helpers.GetKeyboard(new string[] {"Отмена" }));
            }
            else
            {
                await Helpers.ShowMainMenu(message.From.Id);
            }
        }

        async Task AddingHabitHandle(Message message, DAL.Models.User user)
        {
            if (message.Text == "Отмена")
            {
                user.AddingHabitState = 0;
                user.ChatState = 0;
                user.EditingHabitState = 0;

                _userRepository.UpdateUser(user);

                await Helpers.ShowMainMenu(message.From.Id);
                return;
            }

            switch (user.AddingHabitState)
            {
                case AddingHabitState.NameInput:
                    habitCreateModel.Name = message.Text;
                    user.AddingHabitState = AddingHabitState.DescriptionInput;
                    await _botClient.SendTextMessageAsync(message.From.Id,
                                                            "Введите описание привычки:",
                                                            replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    break;
                case AddingHabitState.DescriptionInput:
                    habitCreateModel.Description = message.Text;
                    user.AddingHabitState = AddingHabitState.PeriodInput;
                    await _botClient.SendTextMessageAsync(message.From.Id,
                                                            "Введите периодичность(в днях):",
                                                            replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    break;
                case AddingHabitState.PeriodInput:
                    if (int.TryParse(message.Text, out int value))
                    {
                        habitCreateModel.Period = value;
                        user.AddingHabitState = AddingHabitState.NotificationTimeInput;
                        await _botClient.SendTextMessageAsync(message.From.Id,
                                                                "Введите время напоминания:", 
                                                                replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(message.From.Id,
                                                                "Ошибка чтения, введите периодичность(в днях):",
                                                                replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    }
                    break;
                case AddingHabitState.NotificationTimeInput:
                    habitCreateModel.NotificationTime = DateTime.Now; //TO-DO normal parser for time.
                    user.AddingHabitState = AddingHabitState.Accepting;
                    await Program._botClient.SendTextMessageAsync(message.From.Id,
                                                                "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                                                                replyMarkup: Helpers.GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                    break;
                case AddingHabitState.Accepting:
                    if (message.Text == "Подтвердить")
                    {
                        user.ChatState = ChatState.Main;
                        habitCreateModel.UserId = user.Id;
                        _habitRepository.AddHabit(habitCreateModel);
                        await Helpers.ShowMainMenu(message.From.Id);
                    }
                    else
                    {
                        user.ChatState = ChatState.Main;
                        await Program._botClient.SendTextMessageAsync(message.From.Id,
                                                                       "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                                                                       replyMarkup: Helpers.GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                    }
                    break;
                default:
                    break;
            }
            _userRepository.UpdateUser(user);
        }
    }
}
