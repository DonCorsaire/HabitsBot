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
        private readonly SessionRepository _sessionRepository;

        

        public MessageService(ITelegramBotClient botClient, HabitContext dbContext)
        {
            _botClient = botClient;
            _userRepository = new UserRepository(dbContext);
            _habitRepository = new HabitRepository(dbContext);
            _sessionRepository = new SessionRepository(dbContext);
        }

        public async Task HandleMessage(Message message)
        {
            DAL.Models.User user;
            if (!_userRepository.UserExist(message.From.Id).Result)
            {
                user = await _userRepository.CreateUser(message.From.Id, message.From.FirstName + " " + message.From.LastName);
                await _sessionRepository.CreateInputSession(user);
            }
            else
            {
                user = await _userRepository.GetUser(message.From.Id);
            }

            switch (user.ChatState)
            {
                case ChatState.Main:
                    await MainMenuStateHandle(message, user);
                    break;
                case ChatState.AddingNewHabit:
                    await AddingHabitStateHandle(message, user);
                    break;
                case ChatState.EditingNewHabit:
                    await EditHabitStateHandle(message, user);
                    break;
                default:
                    break;
            }
        }

        async Task MainMenuStateHandle(Message message, DAL.Models.User user)
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
                user.AddingHabitState = AddingHabitState.NameInput;
                await _userRepository.UpdateUser(user);
                await _botClient.SendTextMessageAsync(message.From.Id,
                                                        "Введите название привычки:",
                                                        replyMarkup: Helpers.GetKeyboard(new string[] {"Отмена" }));
            }
            else
            {
                await Helpers.ShowMainMenu(message.From.Id);
            }
        }

        async Task EditHabitStateHandle(Message message, DAL.Models.User user)
        {
            if (message.Text == "Отмена")
            {
                user.AddingHabitState = 0;
                user.ChatState = 0;
                user.EditingHabitState = 0;

                await _userRepository.UpdateUser(user);

                await Helpers.ShowMainMenu(message.From.Id);
                return;
            }

            switch (user.EditingHabitState)
            {
                case EditingHabitState.EditingName:
                    await _sessionRepository.SetHabitName(message.Text, user);
                    break;
                case EditingHabitState.EditingDescription:
                    await _sessionRepository.SetHabitDescription(message.Text, user);
                    break;
                case EditingHabitState.EditingNotificationTime:
                    await _sessionRepository.SetHabitNotificationTime(default, user);//TO-DO
                    break;
                case EditingHabitState.EditingPeriod:
                    if(int.TryParse(message.Text, out int period))
                    {
                        await _sessionRepository.SetHabitPeriod(period, user);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(message.From.Id, "Ошибка распознавания, введите периодичность в днях:");
                        return;
                    }
                    break;
                default:
                    await _botClient.SendTextMessageAsync(message.From.Id, "Используй кнопки последнего меню!");
                    return;
            }

            user.EditingHabitState = EditingHabitState.Main;
            await Helpers.ShowMainEditMenu(user.FromId, await _sessionRepository.GetEditSession(user));
        }

        async Task AddingHabitStateHandle(Message message, DAL.Models.User user)
        {
            if (message.Text == "Отмена")
            {
                user.AddingHabitState = 0;
                user.ChatState = 0;
                user.EditingHabitState = 0;

                await _userRepository.UpdateUser(user);

                await Helpers.ShowMainMenu(message.From.Id);
                return;
            }

            switch (user.AddingHabitState)
            {
                case AddingHabitState.NameInput:
                    await _sessionRepository.SetHabitName(message.Text, user, true);
                    user.AddingHabitState = AddingHabitState.DescriptionInput;
                    await _botClient.SendTextMessageAsync(message.From.Id,
                                                            "Введите описание привычки:",
                                                            replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    break;
                case AddingHabitState.DescriptionInput:
                    await _sessionRepository.SetHabitDescription(message.Text, user);
                    user.AddingHabitState = AddingHabitState.PeriodInput;
                    await _botClient.SendTextMessageAsync(message.From.Id,
                                                            "Введите периодичность(в днях):",
                                                            replyMarkup: Helpers.GetKeyboard(new string[] { "Отмена" }));
                    break;
                case AddingHabitState.PeriodInput:
                    if (int.TryParse(message.Text, out int value))
                    {
                        await _sessionRepository.SetHabitPeriod(value, user);
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
                    await _sessionRepository.SetHabitNotificationTime(DateTime.Now, user); //TO-DO normal parser for time.
                    user.AddingHabitState = AddingHabitState.Accepting;
                    await Program._botClient.SendTextMessageAsync(message.From.Id,
                                                                "Подвердите или отмените добавление привычки (можно добавить вывод описания привычки",
                                                                replyMarkup: Helpers.GetKeyboard(new string[] { "Подтвердить", "Отмена" }));
                    break;
                case AddingHabitState.Accepting:
                    if (message.Text == "Подтвердить")
                    {
                        user.ChatState = ChatState.Main;
                        await _habitRepository.AddHabit(await _sessionRepository.GetHabitCreateModel(user));
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
            await _userRepository.UpdateUser(user);
        }
    }
}
