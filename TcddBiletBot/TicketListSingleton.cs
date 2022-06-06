using System;
using System.Collections.Generic;
using System.Text;
using TcddBiletBot.Model;

namespace TcddBiletBot
{
    public class TicketListSingleton
    {
        private TicketListSingleton() { }

        public static TicketListSingleton instance;

        public static TicketListSingleton Instance
        {
            get { return instance ?? (instance = new TicketListSingleton()); }
        }

        public List<Bilet> TicketList { get; set; } = new List<Bilet>();

    }
}
