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
            //var configuration = new DAL.Migration.Configuration();
            
            //DbMigrator migrator = new DbMigrator(configuration);
            //var migrations = migrator.GetPendingMigrations();

            //foreach (var migration in migrations)
            //{
            //    migrator.Update(migration);
            //}

            _botClient = new TelegramBotClient("993995326:AAEaZwvtWFPuRpVmGinGs5JkMvk8KfFIDyY");

            _BotClientService = new BotClientService(_botClient);
            _botClient.OnUpdate += _BotClientService.BotClientUpdateHandle;
            _botClient.StartReceiving();

            Console.ReadLine();
            _botClient.StopReceiving();
        }
    }
}
