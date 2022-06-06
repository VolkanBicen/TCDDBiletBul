using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TcddBiletBot.TelegramBot
{
    public class HandleBot
    {
        public async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:" + Environment.NewLine + "[{apiRequestException.ErrorCode}]" + Environment.NewLine + "{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine("Hata => " + ErrorMessage);
            return;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message) return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text) return;

            var commands = new Commands();
            await commands.Run(update, client);
        }
    }
}
