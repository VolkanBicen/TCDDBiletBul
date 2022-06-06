using Telegram.Bot;
using Telegram.Bot.Types;

namespace TcddBiletBot.Model
{
    public class Bilet
    {
      public Sefer Sefer { get; set; }
      public Update Update { get; set; }
      public ITelegramBotClient Client { get; set; }
    }
}
