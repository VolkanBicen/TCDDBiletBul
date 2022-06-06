using System;
using TcddBiletBot.Selenium;


namespace TcddBiletBot.Helper
{
    public static class TimerTask
    {

        public static async void SearchTicket(object state)
        {
            try
            {

                var instance = TicketListSingleton.Instance;
                TCDDTicket ticket = new TCDDTicket();

                if (instance.TicketList.Count > 0)
                {
                    foreach (var item in instance.TicketList)
                    {
                        await ticket.Search(item);
                    }
                }
                return;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Hata => " + ex.Message);
            }
        }

    }
}
