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
        public FileStore(string priceFilesPath, string catalogFilesPath, Dictionary<string, BrandData> brandsAndCodes)
        {
            _priceFilesPath = priceFilesPath;
            _catalogFilesPath = catalogFilesPath;
            _brandsAndCodes = brandsAndCodes;
            BrandNames = GetFullBrandHashSet();
            UpdateCatalogs();
            UpdatePrices();
        }
          public string GetCodeByBrand(string brandName)
        {
            return _brandsAndCodes[brandName].PriceCode;
        }
        public bool UpdatePrices() 
        {
            var temp = new Dictionary<string, byte[]> { };
            try
            {
                DirectoryInfo pricesDir = new DirectoryInfo(_priceFilesPath);
                foreach (FileInfo file in pricesDir.GetFiles())
                {
                    if (file.Name.EndsWith(".xlsx"))
                    {
                        string code = file.Name.Substring(0, file.Name.IndexOf('.'));
                        byte[] buffer = File.ReadAllBytes(file.FullName);
                        temp[code] = buffer;
                    }
                }
            }
            catch (Exception e)
            { 
                return false;
            }
            CodesAndPrices = temp;
            return true;    
            
        } 
        public bool UpdateCatalogs()
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
            catch (Exception e)
            {
                return false;
            }
            BrandsAndCatalogsFiles = temp;
            return true;
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
