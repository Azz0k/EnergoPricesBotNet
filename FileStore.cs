using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergoPricesBotNet
{
    public class FileStore
    {
        private Dictionary<string, BrandData>? _brandsAndCodes = null;
        private Dictionary<string, byte[]>? _codesAndPrices = null;
        private Dictionary<string, List<CatalogFile>>? _codesAndCatalogs = null;
        private string _priceFilesPath = string.Empty;
        private string _catalogFilesPath = string.Empty;
        public List<string> FoodBrandNames = new List<string>();
        public List<string> NonFoodBrandNames = new List<string>();
        public FileStore(string priceFilesPath, string catalogFilesPath)
        {
            _priceFilesPath = priceFilesPath;
            _catalogFilesPath = catalogFilesPath;
            Initilizer();
        }
        private void Initilizer()
        {
            _brandsAndCodes = new Dictionary<string, BrandData>()
            {
                { "Сады Придонья", new BrandData ("56801") },
                { "Бунге", new BrandData ("138346") },
                { "Перфиса", new BrandData ("236967", 1) },
                { "Валдайский погребок", new BrandData ("35379") },
                { "Знаток", new BrandData ("168157") },
                { "НМЖК", new BrandData ("139386") },
                { "Кубаночка", new BrandData ("232741") },
                { "Хайнц", new BrandData ("8395") },
                { "Казанский жировой комбинат", new BrandData ("237557") },
                { "Пиканта", new BrandData ("46497") },
                { "Мираторг", new BrandData ("226874") },
                { "Московская кофейня на паях", new BrandData ("224064") },
                { "Норма жизни (Восход)", new BrandData ("233579") },
                { "РУМЯНОЧКА", new BrandData ("237551") },
                { "Русский продукт", new BrandData ("233027") },
                { "Арматени", new BrandData ("233740") },
                { "МедЛен", new BrandData ("233615",1) },
            };
            FoodBrandNames = GetBrandNamesByType(0);
            NonFoodBrandNames = GetBrandNamesByType(1);
        }
        public List<string> GetBrandNamesByType(byte type)
        {
            var res = new List<string>();
            if (_brandsAndCodes == null)
                return res;
            foreach (var entry in _brandsAndCodes)
            {
                if (entry.Value.Type == type)
                {
                    res.Add(entry.Key);
                }
            }
            return res;
        }
        public HashSet<string>? GetFullBrandHashSet()
        {
            return _brandsAndCodes?.Keys.ToHashSet();
        }

    }

    public class CatalogFile
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Data { get; set; } = new byte[0];
    }
    public class BrandData
    {
        public string PriceCode { get; set; } = string.Empty;
        public byte Type { get; set; } = 0;
        public BrandData(string priceCode, byte type = 0)
        {
            this.PriceCode = priceCode;
            this.Type = type;
        }

    }
}
