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
using PricesBotWorkerService;
using static System.Formats.Asn1.AsnWriter;

namespace PricesBotWorkerService
{
    public class BotService
    {

        private IServiceProvider _serviceProvider;
        private ILogger<WindowsBackgroundService> _logger;
        private HashSet<string> _brandNames = new HashSet<string>();
        private AppSettings _settings;
        public bool isDataReceived = false;
        private string[] _menuNames = new string[] { "Посмотреть каталог продукции", "Акции, распродажи", "Новинки" };
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
        public bool isCatalogMenuButtonPressed(string text) 
        {
            return text.StartsWith("Получить каталог ");
        }
        public bool isPriceMenuButtonPressed(string text)
        {
            return text.StartsWith("Получить Прайс-лист ");
        }
        public string getBrandIfCatalogMenuButtonPressed(string text)
        {
            if (isCatalogMenuButtonPressed(text))
            {
                string brandName = text.Substring(16).Trim();
                if (isSelected(brandName))
                {
                    return brandName;
                }
            }
            return "";
        }
        public string getBrandIfPriceMenuButtonPressed(string text)
        {
            if (isPriceMenuButtonPressed(text))
            {
                string brandName = text.Substring(20).Trim();
                if (isSelected(brandName))
                {
                    return brandName;
                }
            }
            return "";
        }

        public bool isSelected(string text)
        {
            return _brandNames.Contains(text);
        }
        public bool isMenuButtonPressed(string text)
        {
            return _menuNames.Contains(text);
        }
        public string generateReplyText(string text)
        {
            
            if (isMenuButtonPressed(text)){
                if (text == _menuNames[0])
                {
                    return _settings.BrandSelectGreeting;

                }
                return _settings.NotImplementedGreeting;
            }
            if (isSelected(text)) {
                return _settings.PriceOrCatalogGreeting;
            }
            return _settings.FirstMenuLevelGreeting; ;
        }

        public string [][] createFirstMenuLevel()
        {

                        return new string[][] { [_menuNames[0]], [_menuNames[1], _menuNames[2]] };
        }
        public string[][] generateReplyMarkup(string text) {

            if (isMenuButtonPressed(text)){
                if (text == _menuNames[0])
                {
                    return createBradsMarkup();
                }
                return createFirstMenuLevel(); 
            }
            if (isSelected(text))
            {
                return new string[][] { ["Получить каталог "+text], ["Получить Прайс-лист "+text] };
            }
            return createFirstMenuLevel();
        }

            
    }
}

