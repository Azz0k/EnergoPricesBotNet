using EnergoPricesBotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesBotWorkerService
{
    public class AppSettings
    {
        public string TelegramBotToken { get; init; } = string.Empty;
        public string BackSymbol { get; init; } = string.Empty;
        public string HomeSymbol { get; init; } = string.Empty;
        public string CallBackDataPrefix { get; init; } = string.Empty;
        public string FirstGreetingText { get; init; } = string.Empty;
        public string FirstMenuLevelGreeting { get; init; } = string.Empty;
        public string BrandSelectGreeting { get; init; } = string.Empty;
        public string NotImplementedGreeting { get; init; } = string.Empty;
        public string PriceOrCatalogGreeting { get; init; } = string.Empty;
        public string GoodsSectionGreeting { get; init; } = string.Empty;
        public string CatalogFilesPath { get; init; } = string.Empty;
        public string PriceFilesPath { get; init; } = string.Empty;
        public string BrandsConfigFile { get; init; } = string.Empty;
        public Dictionary<string, BrandData> Brands { get; init; } = new() { };
    }


}
