using PrivickerBot.DAL.Enums;
using PrivickerBot.DAL.Models;
using PrivickerBot.Models.ViewModel;
using PrivickerBot.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PrivickerBot.Services
{
    class CallbackQueryService
    {
        private readonly Dictionary<string, Func<Task>> _commandFactory = new Dictionary<string, Func<Task>>();
        private readonly ITelegramBotClient _botClient;
        private readonly UserRepository _userRepository;
        private readonly HabitRepository _habitRepository;

        CallbackQuery _callback;
        int habitId;
        public CallbackQueryService(ITelegramBotClient botClient, HabitContext dbContext)
        {
            _botClient = botClient;
            _userRepository = new UserRepository(dbContext);
            _habitRepository = new HabitRepository(dbContext);

            _commandFactory.Add("/get", GetHabitView);
            _commandFactory.Add("/edit", EditHabitView);
            _commandFactory.Add("/delete", DeleteHabitView);
            _commandFactory.Add("/remove", RemoveHabitView);
            _commandFactory.Add("/cancel", CancelView);
        }

        public async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            _callback = callbackQuery;

            string[] callbackData = _callback.Data.Trim().ToLower().Split(' '); // [0] - task key [1] - habitId;

            if (callbackData.Length > 1)
            {
                int.TryParse(callbackData[1], out habitId);
            }
            //TO-DO better way to store and parse id?


            if (_commandFactory.ContainsKey(callbackData[0]))
            {
                var action = _commandFactory[callbackData[0]];
                if (action != null)
                {
                    await action();
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ответка через CallbackQueryService");
            }
        }

        private async Task GetHabitView()
        {
            HabitViewModel viewModel = await _habitRepository.GetHabit(1);

            InlineKeyboardButton editBtn = new InlineKeyboardButton
            {
                CallbackData = "/edit " + habitId,
                Text = "Изменить"
            };

            InlineKeyboardButton deleteBtn = new InlineKeyboardButton
            {
                CallbackData = "/delete " + habitId,
                Text = "Удалить"
            };

            //TO-DO add "back" btn, or set reply menu to dontHide
            await _botClient.SendTextMessageAsync(_callback.From.Id,
                                                      "GetHabitView через CallbackQueryService :\n" + viewModel.ToString(),
                                                      replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { editBtn, deleteBtn }));
        }

        private async Task EditHabitView()
        {
            //TO-DO
            DAL.Models.User user = _userRepository.GetUser(_callback.From.Id);

            var habitEditModel = _habitRepository.GetEditHabitModel(habitId);
            user.EditingHabitState = EditingHabitState.Main;
            user.ChatState = ChatState.EditingNewHabit;

            //ShowMainEditMenu(_callback.From.Id);

            _userRepository.UpdateUser(user);

            await _botClient.SendTextMessageAsync(_callback.From.Id, "EditHabitView через CallbackQueryService\n");
        }

        private async Task DeleteHabitView()
        {
            InlineKeyboardButton yesBtn = new InlineKeyboardButton
            {
                CallbackData = "/remove " + habitId,
                Text = "Да"
            };

            InlineKeyboardButton noBtn = new InlineKeyboardButton
            {
                CallbackData = "/get " + habitId,
                Text = "Нет"
            };

            await Program._botClient.SendTextMessageAsync(_callback.From.Id,
                                                      "DeleteHabitView через CallbackQueryService \nВы действительно хотите удалить привычку " +
                                                       _habitRepository.GetHabit(habitId).Result.Name + "?",
                                                       replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { yesBtn, noBtn }));

        }

        private async Task RemoveHabitView()
        {
            _habitRepository.DeleteHabit(habitId);

            //TO-DO return UI to mainMenu
            await _botClient.SendTextMessageAsync(_callback.From.Id, "RemoveHabitView через CallbackQueryService");
        }

        private async Task CancelView()
        {
            DAL.Models.User user = _userRepository.GetUser(_callback.From.Id);

            user.AddingHabitState = 0;
            user.ChatState = 0;
            user.EditingHabitState = 0;

            _userRepository.UpdateUser(user);

            //TO-DO return UI to mainMenu

            await _botClient.SendTextMessageAsync(_callback.From.Id, "CancelView через CallbackQueryService");
        }
    }
}
