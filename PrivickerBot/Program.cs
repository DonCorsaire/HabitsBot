using System;
using Telegram.Bot;
using PrivickerBot.Services;

namespace PrivickerBot
{
    class Program
    {
        public static ITelegramBotClient _botClient;
        private static BotClientService _BotClientService;

        static void Main(string[] args)
        {
            _botClient = new TelegramBotClient("youadmaybehere");

            _BotClientService = new BotClientService(_botClient);
            _botClient.OnUpdate += _BotClientService.BotClientUpdateHandle;
            _botClient.StartReceiving();

            Console.ReadLine();
            _botClient.StopReceiving();
        }
    }
}
