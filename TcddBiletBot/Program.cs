using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TcddBiletBot.TelegramBot;
using TcddBiletBot.Helper;

namespace TcddBiletBot
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var bot = new HandleBot ();

            CancellationTokenSource cts = new CancellationTokenSource();
            var botClient = new TelegramBotClient("5194916816:AAHFHM-qULbrsZvsAokjgkbKVwgLgiQ8rhM");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
                ThrowPendingUpdates = true
            };

            botClient.StartReceiving(
            bot.HandleUpdateAsync,
            bot.HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);
            
        
            Timer timer = new Timer(Helper.TimerTask.SearchTicket, null, 0, 300000);
           
            Console.WriteLine("Bot Başladı." + Environment.NewLine + "Durdurmak İçin Bir Tuşa Bas");
            var line = Console.ReadLine();
        }
    }
}
