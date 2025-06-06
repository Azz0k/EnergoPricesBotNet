﻿using Microsoft.EntityFrameworkCore;
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
using EnergoPricesBotNet;
using Microsoft.EntityFrameworkCore.Storage;

namespace PricesBotWorkerService
{
    public class BotService
    {

        private IServiceProvider _serviceProvider;
        private ILogger<WindowsBackgroundService> _logger;
        //private HashSet<string> _brandNames = new HashSet<string>();
        private AppSettings _settings;
        public bool isDataReceived = false;
        private readonly string[] _menuNames = new string[] { "Посмотреть каталог продукции", "Акции, распродажи", "Новинки" };
        private readonly string[] _foodOrNonFoodMenu = new string[] { "Продукты питания", "Товары нон-фуд" };
        private FileStore _fileStore;
        public BotService(IOptions<AppSettings> settings, ILogger<WindowsBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            //InitializeMockBrandsSet();
            _fileStore = new FileStore(_settings.PriceFilesPath, _settings.CatalogFilesPath, _settings.Brands);

        }
        /*
        public void InitializeMockBrandsSet()
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
        */
        public string[][] CreateBrandsMarkup()
        {
            string[][] buttons = _fileStore.BrandNames.Select(item => new string[] { item }).ToArray();
            return buttons;
        }
        public string[][] CreateBrandsMarkupByType(byte type)
        {
            string[][] buttons = _fileStore.GetBrandNamesByType(type).Select(item => new string[] { item }).ToArray();
            return buttons;
        }
        public string[][] CreateFoodOrNonFoodMarkup()
        {
            return new string[][] { [_foodOrNonFoodMenu[0]], [_foodOrNonFoodMenu[1]] };
        }
        public bool IsCatalogMenuButtonPressed(string text)
        {
            return text.StartsWith("Получить каталог ");
        }
        public bool IsPriceMenuButtonPressed(string text)
        {
            return text.StartsWith("Получить Прайс-лист ");
        }
        public string GetBrandIfCatalogMenuButtonPressed(string text)
        {
            if (IsCatalogMenuButtonPressed(text))
            {
                string brandName = text.Substring(16).Trim();
                if (IsBrandSelected(brandName))
                {
                    return brandName;
                }
            }
            return "";
        }
        public string GetBrandIfPriceMenuButtonPressed(string text)
        {
            if (IsPriceMenuButtonPressed(text))
            {
                string brandName = text.Substring(20).Trim();
                if (IsBrandSelected(brandName))
                {
                    return brandName;
                }
            }
            return "";
        }
        public byte[] GetPriceFileBufferByBrand(string brandName)
        {
            string code = _fileStore.GetCodeByBrand(brandName);
            return _fileStore.CodesAndPrices[code];
        }
        public List<CatalogFile> GetCatalogFileListByBrandName(string brandName)
        {
            return _fileStore.BrandsAndCatalogsFiles[brandName];
        }

        public bool IsBrandSelected(string text)
        {
            return _fileStore.BrandNames.Contains(text);
        }
        public bool IsFirstMenuButtonPressed(string text)
        {
            return _menuNames.Contains(text);
        }
        public bool IsFoodSelected(string text)
        {
            return text.StartsWith(_foodOrNonFoodMenu[0]);
        }
        public bool IsNonFoodSelected(string text)
        {
            return text.StartsWith(_foodOrNonFoodMenu[1]);
        }
        public string [][] CreateFirstMenuLevel()
        {
             return new string[][] { [_menuNames[0]], [_menuNames[1], _menuNames[2]] };
        }

        public string[][] CreatePriceOrCatalogMarkup(string text)
        {
            return new string[][] { ["Получить каталог " + text], ["Получить Прайс-лист " + text] };
        }
        public BotResponse GenerateResponse(string text)
        {

            if (IsFirstMenuButtonPressed(text))
            {
                if (text == _menuNames[0])
                {
                    return new BotResponse(_settings.GoodsSectionGreeting, CreateFoodOrNonFoodMarkup());

                }
                return new BotResponse(_settings.NotImplementedGreeting, CreateFirstMenuLevel());
            }
            if (IsFoodSelected(text))
            {
                return new BotResponse(_settings.BrandSelectGreeting, CreateBrandsMarkupByType(0));
            }
            if (IsNonFoodSelected(text))
            {
                return new BotResponse(_settings.BrandSelectGreeting, CreateBrandsMarkupByType(1));
            }

            if (IsBrandSelected(text))
            {
                return new BotResponse(_settings.PriceOrCatalogGreeting, CreatePriceOrCatalogMarkup(text));
            }
            return new BotResponse(_settings.FirstMenuLevelGreeting, CreateFirstMenuLevel());
        }
        public void UpdateFiles()
        {
            if (!_fileStore.UpdatePrices())
            {
                _logger.LogCritical("Prices is NOT updated!!!");
            }
            if (!_fileStore.UpdateCatalogs())
            {
                _logger.LogCritical("Catalogs is NOT updated!!!");
            }
            
        }


    }
}

