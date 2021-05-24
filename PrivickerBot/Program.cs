﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.DependencyInjection;
using PrivickerBot.Repositories;
using PrivickerBot.Models;

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



            var configuration = new PrivickerBot.Migration.Configuration();
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
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
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
