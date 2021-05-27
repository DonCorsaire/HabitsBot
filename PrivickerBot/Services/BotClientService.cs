using PrivickerBot.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace PrivickerBot.Services
{
    class BotClientService
    {
        private readonly ITelegramBotClient _botClient;
        public BotClientService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async void BotClientUpdateHandle(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            try
            {
                var update = e?.Update;
                if (update == null)
                {
                    return;
                }
                using HabitContext dbContext = new HabitContext();
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        await new MessageService(_botClient, dbContext).HandleMessage(update.Message);
                        break;
                    case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                        await new CallbackQueryService(_botClient, dbContext).HandleCallbackQuery(update.CallbackQuery);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - Ошибка {ex.Message}");
            }
        }
    }
}
