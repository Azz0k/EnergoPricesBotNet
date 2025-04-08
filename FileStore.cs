using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergoPricesBotNet
{
    public class FileStore
    {
        private Dictionary<string, BrandData> _brandsAndCodes = new();
        public Dictionary<string, byte[]> CodesAndPrices = new();
        public Dictionary<string, List<CatalogFile>> BrandsAndCatalogsFiles = new();
        private string _priceFilesPath = string.Empty;
        private string _catalogFilesPath = string.Empty;
        public HashSet<string> BrandNames = new HashSet<string>();
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
            BrandNames = GetFullBrandHashSet();
            UpdatePrices();
            UpdateCatalogs();
        }
        public string GetCodeByBrand(string brandName)
        {
            return _brandsAndCodes[brandName].PriceCode;
        }
        public void UpdatePrices() 
        {
            var temp = new Dictionary<string, byte[]> { };
            try
            {
                DirectoryInfo pricesDir = new DirectoryInfo(_priceFilesPath);
                foreach (FileInfo file in pricesDir.GetFiles())
                {
                    if (file.Name.EndsWith(".xlsx"))
                    {
                        string code = file.Name.Substring(0,file.Name.IndexOf('.'));
                        byte[] buffer = File.ReadAllBytes(file.FullName);
                        temp[code] = buffer;
                    }
                }
            }
            finally
            {
                CodesAndPrices = temp;
            }
            
        } 
        public void UpdateCatalogs()
        {
            var temp = new Dictionary<string, List<CatalogFile>> { };
            try
            {
                DirectoryInfo catalogsDir = new DirectoryInfo(_catalogFilesPath);
                foreach (var subDir  in catalogsDir.GetDirectories())
                {
                    if (BrandNames.Contains(subDir.Name))
                    {
                        temp[subDir.Name] = new List<CatalogFile> { };
                        foreach (FileInfo file in subDir.GetFiles())
                        {
                            if (file.Name.EndsWith(".pdf"))
                            {
                                byte[] buffer = File.ReadAllBytes(file.FullName);
                                var cf = new CatalogFile
                                {
                                    FileName = file.Name,
                                    Data = buffer
                                };
                                temp[subDir.Name].Add(cf);
                            }
                        }
                    }
                    
                }
            }
            finally
            {
                BrandsAndCatalogsFiles = temp;   
            }
        }
        public List<string> GetBrandNamesByType(byte type)
        {
            var res = new List<string>();
            foreach (var entry in _brandsAndCodes)
            {
                if (entry.Value.Type == type)
                {
                    res.Add(entry.Key);
                }
            }
            return res;
        }
        private HashSet<string> GetFullBrandHashSet()
        {
            return _brandsAndCodes.Keys.ToHashSet();
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
