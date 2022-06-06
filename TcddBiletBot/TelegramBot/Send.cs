using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TcddBiletBot.TelegramBot
{
    public class Send
    {
        public async Task Message(ITelegramBotClient client, Update update,string messageText)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: messageText
                );
        }
    }
}
