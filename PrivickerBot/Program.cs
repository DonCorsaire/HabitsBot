using System;
using System.Data.Entity.Migrations;
using Telegram.Bot;
using PrivickerBot.Repositories;
using PrivickerBot.DAL.Models;
using PrivickerBot.Services;

namespace PrivickerBot
{
    class Program
    {
        public static ITelegramBotClient _botClient;
        private static BotClientService _BotClientService;

        static void Main(string[] args)
        {
            var configuration = new DAL.Migration.Configuration();
            DbMigrator migrator = new DbMigrator(configuration);
            var migrations = migrator.GetPendingMigrations();

            foreach (var migration in migrations)
            {
                migrator.Update(migration);
            }

            _botClient = new TelegramBotClient("youradmaybehere");

            _BotClientService = new BotClientService(_botClient);
            _botClient.OnUpdate += _BotClientService.BotClientUpdateHandle;
            _botClient.StartReceiving();

            Console.ReadLine();
            _botClient.StopReceiving();
        }
    }
}
