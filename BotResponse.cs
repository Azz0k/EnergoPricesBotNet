using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergoPricesBotNet
{
    public class BotResponse
    {
        public string messageText { get; init; } = string.Empty;
        public string[][] replayMarkup { get; init; } = new string[][] { };
        public BotResponse(string mt, string[][] rm) 
        {
            messageText = mt;
            replayMarkup = rm;
        }

    }
}
