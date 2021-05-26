using System;
using System.Data.Entity.Migrations;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using PrivickerBot.Repositories;
using PrivickerBot.DAL.Models;

namespace PrivickerBot
{
    class Program
    {
        public static TelegramBotClient client;
        private static CommandHandler commandHandler;

        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<UserRepository>()
                .AddScoped<HabitRepository>()
                .AddScoped<HabitContext>()
                .BuildServiceProvider();



            var configuration = new DAL.Migration.Configuration();
            DbMigrator migrator = new DbMigrator(configuration);
            var migrations = migrator.GetPendingMigrations();

            foreach (var migration in migrations)
            {
                migrator.Update(migration);
            }


            client = new TelegramBotClient("youradmaybehere");
            commandHandler = new CommandHandler( serviceProvider.GetService<HabitRepository>(), serviceProvider.GetService<UserRepository>() );
            client.OnMessage += BotOnMessageReceived;

            client.OnMessageEdited += BotOnMessageReceived;

            client.OnCallbackQuery += BotOnCallbackReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void BotOnCallbackReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var callback = e.CallbackQuery;
            await commandHandler.HandleCallbackAsync(callback);
        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message?.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                await commandHandler.ParseAndHandleAsync(e.Message);
            }
        }
    }
}
