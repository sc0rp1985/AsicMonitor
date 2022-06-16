using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Classes
{
    public class СonnectionConfig
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }

    public class TelegramConfig
    { 
        public string BotToken { get; set; }
    }

}
