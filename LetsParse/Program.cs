using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LetsParse.Company;

namespace LetsParse
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var companyList = new List<ICompany>()
            {
                new Denticom(),
                new GreenDent(),
                
            };

            foreach (var company in companyList)
            {
                await TryParse(company);
            }
          
        }
        
        private static async Task TryParse(ICompany company)
        {
            var parsedProducts = new List<Product>();
            var categoryList = await TryGetAllCategory(company);
            foreach (var category in categoryList)
            {
                var web = new HtmlWeb();
                var maxPage = await GetMaxPageInCategory(category.Url);
                var count = 0;
                var pageNumber = 1;
            
                while (true)
                {
                    if (pageNumber >= maxPage+1)
                    {
                        break;
                    }

                    var url = company.type switch {
                        CompanyType.Denticom  =>  $"{category.Url}?PAGEN_2={pageNumber}",
                        CompanyType.Greendent =>  $"{category.Url}?PAGEN_1={pageNumber}",
                        // CompanyType.Dentex => expr,
                        // CompanyType.StomShop => expr,
                        // CompanyType.GoldiDent => expr,
                        // CompanyType.OMKMedical => expr,
                        // CompanyType.Amfodent => expr,
                        // CompanyType.KupiDenatal => expr,
                        // CompanyType.StomDevice => expr,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var page = await web.LoadFromWebAsync(url);
                
                    Thread.Sleep(10);

                    var products = company.type switch
                    {
                        CompanyType.Denticom => page.DocumentNode.SelectNodes("//div[@class='item']"),
                        CompanyType.Greendent => page.DocumentNode.SelectNodes("//div[contains(@class, 'catalog-section-item-wrapper')]"),
                        // CompanyType.Dentex => expr,
                        // CompanyType.StomShop => expr,
                        // CompanyType.GoldiDent => expr,
                        // CompanyType.OMKMedical => expr,
                        // CompanyType.Amfodent => expr,
                        // CompanyType.KupiDenatal => expr,
                        // CompanyType.StomDevice => expr,
                        // _ => throw new ArgumentOutOfRangeException()
                    };
                        
                    if (products is null || products.Count is 0)
                        break;
                    

                    foreach (var product in products)
                    {
                        var parsedProduct = company.type switch
                        {
                            CompanyType.Denticom => new Product()
                            {
                                Name = product.SelectSingleNode(".//a[@class='name']").InnerText.Trim(),
                                Price = product.SelectSingleNode(".//div[@class='price']/text()[1]")?.InnerText.Replace(" ", "").Trim(),
                                Url = company.baseUrl + product.SelectSingleNode(".//a[@class='name']").GetAttributeValue("href", ""),
                                Company = company.Name,
                                Category = category.Name
                            },
                            CompanyType.Greendent =>  new Product()
                            {
                                Name = product.SelectSingleNode(".//a[contains(@class, 'intec-cl-text-hover')]").InnerText.Trim(),
                                Price = product.SelectSingleNode(".//div[contains(@class, 'catalog-section-item-price-base')]").InnerText.Trim(),
                                Category = category.Name
                            },
                            
                            // CompanyType.Dentex => expr,
                            // CompanyType.StomShop => expr,
                            // CompanyType.GoldiDent => expr,
                            // CompanyType.OMKMedical => expr,
                            // CompanyType.Amfodent => expr,
                            // CompanyType.KupiDenatal => expr,
                            // CompanyType.StomDevice => expr,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        parsedProducts.Add(parsedProduct);
                        count++;
                    }

                    Console.WriteLine($"Parsing page... {pageNumber}");
                    pageNumber++;
                  
                }
                Console.WriteLine($"Found {count} products in {category.Name}\n");
               
            }
            // await SaveResults(parsedProducts);
        }
        
        
        private static async Task<List<Category>> TryGetAllCategory(ICompany company)
        { 
            var web = new HtmlWeb();
            
            var categoryList = new List<Category>();
            var document = await web.LoadFromWebAsync(company.baseUrl);
            var categories = document.DocumentNode.SelectNodes(company.CategoryNode);

            foreach (var category in categories)
            {
                categoryList.Add(
                    new Category()
                    {
                        Name =  company.type switch
                        {
                            CompanyType.Denticom => category.InnerText.Replace("+", "").Trim(),
                            CompanyType.Greendent => category.SelectSingleNode("a").InnerText.Trim(),
                            // CompanyType.Dentex => expr,
                            // CompanyType.StomShop => expr,
                            // CompanyType.GoldiDent => expr,
                            // CompanyType.OMKMedical => expr,
                            // CompanyType.Amfodent => expr,
                            // CompanyType.KupiDenatal => expr,
                            // CompanyType.StomDevice => expr,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        
                        Url = company.type switch {
                            CompanyType.Denticom => company.baseUrl + category.GetAttributeValue("href", ""),
                            CompanyType.Greendent => company.baseUrl + category.SelectSingleNode("a").Attributes["href"].Value,
                            // CompanyType.Dentex => expr,
                            // CompanyType.StomShop => expr,
                            // CompanyType.GoldiDent => expr,
                            // CompanyType.OMKMedical => expr,
                            // CompanyType.Amfodent => expr,
                            // CompanyType.KupiDenatal => expr,
                            // CompanyType.StomDevice => expr,
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    }
                );
            }
            return categoryList;
        }

        private static async Task<int> GetMaxPageInCategory(string baseUrl)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(baseUrl);
            var items = doc.DocumentNode.SelectNodes("//div[contains(@class, 'system-pagenavigation-item')]");

            if (items == null || items.Count == 0)
            {
                return 1;
            }

            var hrefs = items.Select(i => i.SelectSingleNode("a")?.Attributes["href"].Value).ToList();

            var maxPagen = hrefs.Select(href =>
            {
                var parts = href?.Split('=');
                return int.TryParse(parts?.LastOrDefault(), out var pagen) ? pagen : 1;
            }).Max();

            return maxPagen;
        }
        
        private static async Task SaveResults(List<Product> products)
        {
            using (var writer = new StreamWriter($"{products[0].Company}.txt"))
            {
                foreach (var product in products)
                {
                    writer.WriteLine("Name: "     + (product.Name??"null"));
                    writer.WriteLine("Price: "    + (product.Price??"null"));
                    writer.WriteLine("Category: " + (product.Category??"null"));
                    writer.WriteLine("URL: "      + (product.Url??"null"));
                    writer.WriteLine("Company:  " + (product.Company??"null"));
                    writer.WriteLine();
                }
                writer.WriteLine("Next Company");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
            }
        }
        
        public void MedTorg()
        {
            var url = "https://region-medtorg.ru/stomatologicheskoe-oborudovanie";
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var products = new List<Product>();

            var productNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='product-thumb']");
            foreach (var productNode in productNodes)
            {
                var product = new Product();

                var linkNode = productNode.SelectSingleNode(".//p[@class='desk']/a");
                product.Name = linkNode.InnerText.Trim();
                product.Url = linkNode.GetAttributeValue("href", "").Trim();

                var skuNode = productNode.SelectSingleNode(".//p[@class='sku']");
                product.Sku = skuNode.InnerText.Trim();

                var priceNode = productNode.SelectSingleNode(".//span[@class='rub']");
                if (priceNode != null)
                {
                    product.Price = priceNode.InnerText.Trim();
                }

                products.Add(product);
            }

            using (var writer = new StreamWriter("products.txt"))
            {
                foreach (var product in products)
                {
                    writer.WriteLine("Name: " + product.Name);
                    writer.WriteLine("URL: " + product.Url);
                    writer.WriteLine("SKU: " + product.Sku);
                    writer.WriteLine("Price: " + product.Price);
                    writer.WriteLine();
                }
            }
        }
      
    }

   
    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
        
        public string Company { get; set; }
        public string Url { get; set; }
        public string Sku { get; set; }
       
    }

    public class Category
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}