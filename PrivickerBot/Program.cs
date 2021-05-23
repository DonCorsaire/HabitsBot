using System;
using System.Collections;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PrivickerBot
{
    class Program
    {
        public static TelegramBotClient client;
        private static CommandHandler commandHandler;

        static void Main(string[] args)
        {
            client = new TelegramBotClient("youradmaybehere");
            commandHandler = new CommandHandler();
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
