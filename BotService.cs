using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using TestWorkerService;
using static System.Formats.Asn1.AsnWriter;

namespace TestWorkerService
{
    public class BotService
    {

        private IServiceProvider _serviceProvider;
        private ILogger<WindowsBackgroundService> _logger;
        private HashSet<string> _brandNames = new HashSet<string>();
        private AppSettings _settings;
        public bool isDataReceived = false;
        public BotService(IOptions<AppSettings> settings, ILogger<WindowsBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            initializeMockBrandsSet();
        }

        public void initializeMockBrandsSet()
        {
            _brandNames = new HashSet<string>() 
            {
                "Сады Придонья",
                "Бунге",
                "Перфиса",
                "Валдайский погребок",
                "Знаток",
                "НМЖК",
                "Кубаночка",
                "Хайнц",
                "Казанский жировой комбинат",
                "Пиканта",
                "Мираторг",
                "Московская кофейня на паях",
                "Пиканта",
                "Норма жизни (Восход)",
                "РУМЯНОЧКА",
                "Русский продукт",
                "Сады Придонья",
                "Арматени",
                "МедЛен"
            };
            
        }
        public string[][] createBradsMarkup()
        {

            string[][] buttons = _brandNames.Select(item => new string[] { item }).ToArray();
            return buttons;

        }
        public bool isSelected(string text)
        {
            return _brandNames.Contains(text);
        }
    }
}

