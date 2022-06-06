using System;
using System.Threading.Tasks;
using TcddBiletBot.Model;
using TcddBiletBot.Selenium;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TcddBiletBot.TelegramBot
{
    public class Commands
    {
        public async Task Run(Update update, ITelegramBotClient client)
        {
            Send send = new Send();
            try
            {
                var instance = TicketListSingleton.Instance;

                if (!(update.Message.Chat.Id == 521611981 || update.Message.Chat.Id == 1311218759 ))
                {
                    await send.Message(client, update, "Uygulamayı Kullanmak İçin Yetkiniz Yok. Lütfen  @Volki06 İle İletişime Geçiniz.");
                    return;
                }

                string nasilCalisir = "/biletAra komutunu yazdıktan sonra aralarında virgül olacak şekilde" + Environment.NewLine + Environment.NewLine +
                                       "Kalkış istaysonu(Ankara Gar), Varış istasyonu(Eskişehir),Gidis tarihi(01.01.2022),Minumum Gidiş Saati(09:00),Maximum Gidiş Saati(23:00)," +
                                       "Dönüş tarihi(01.01.2022),Minumum Dönüş Saati(09:00),Maxiumum Dönüş Saati(23:00)" + Environment.NewLine + Environment.NewLine + " Yazılır ve mesaj gönderilir!" + Environment.NewLine + Environment.NewLine +
                                       "ÖRN:" + Environment.NewLine + "/biletAra Ankara Gar,Eskişehir,01.01.2022,08:00,09:30,01.01.2022,15:00,19:00";

                string commands = "/nasilcalisir" + Environment.NewLine +
                                   "/biletAra" + Environment.NewLine +
                                   "/komutlar";

                string[] commandParameters = update.Message.Text.Split(" ");

                if (commandParameters.Length == 1)
                {
                    switch (commandParameters[0].ToLowerInvariant())
                    {
                        case "/start":
                        case "/nasilcalisir":
                            await send.Message(client, update, nasilCalisir);
                            return;
                        case "/komutlar":
                            await send.Message(client, update, commands);
                            return;
                        case "/iptal":
                            instance.TicketList.RemoveAll(x => x.Update.Message.Chat.Id == update.Message.Chat.Id);
                            await send.Message(client, update, "Tüm İşlemleriniz İptal Edildi!");
                            return;
                        default:
                            await send.Message(client, update, "Geçersiz Komut!" + Environment.NewLine + commands);
                            return;
                    }
                }

                var result = instance.TicketList.Find(x => x.Update.Message.Chat.Id == update.Message.Chat.Id);
                if (result != null)
                {
                    await send.Message(client, update, "Hala Devam Eden İşleminiz Var! Yeni Bir İşlem İçin Önceki İşlemi İptal Et !");
                    return;
                }

                update.Message.Text = update.Message.Text.Replace(commandParameters[0], "").Trim();
                string[] seferBilgileri = update.Message.Text.Split(",");

                Sefer seferModel = new Sefer();
                seferModel.Kalkis = seferBilgileri[0];
                seferModel.Varis = seferBilgileri[1];
                seferModel.GidisTarihi = seferBilgileri[2];
                seferModel.MinGidisSaat = seferBilgileri[3];
                seferModel.MaxGidisSaat = seferBilgileri[4];
                seferModel.DonusTarihi = seferBilgileri[5];
                seferModel.MinDonusSaat = seferBilgileri[6];
                seferModel.MaxDonusSaat = seferBilgileri[7];

                Bilet biletModel = new Bilet();
                biletModel.Client = client;
                biletModel.Sefer = seferModel;
                biletModel.Update = update;
                
                await send.Message(client, update, "Bot başlatıldı. Her 5 Dakikada Bir Bilet Arama İşlemi Tekrar Edilecek.");
                instance.TicketList.Add(biletModel);

                TCDDTicket ticket = new TCDDTicket();
                 await ticket.Search(biletModel);

            }
            catch (Exception ex)
            {
                await send.Message(client, update, "Bir Hata Oluştu Sefer Bilgilerini Kontrol Et.");
                Console.WriteLine("Hata => " + ex.Message);
            }

        }
    }
}
