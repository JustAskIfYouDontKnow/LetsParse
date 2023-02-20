using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LetsParse.Company;

namespace LetsParse.Parser;

public static class Parser
{
    public static async Task TryParse(ICompany company)
    {
        
        var categoryList = await TryParseCategory.TryGetAllCategory(company);
        var parsedProducts = new List<Product.Product>();
    

        foreach (var category in categoryList)
        {
            var web = new HtmlWeb();
            
            var maxPage = await TryParseMaxPage.GetMaxPage(company.type, category.Url);
            var count = 0;
            var pageNumber = 1;

            while (true)
            {
                if (pageNumber > maxPage)
                    break;
                
                var url = company.type switch
                {
                    CompanyType.Denticom  => $"{category.Url}?PAGEN_2={pageNumber}",
                    CompanyType.Greendent => $"{category.Url}?PAGEN_1={pageNumber}",
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

              Thread.Sleep(150);

                var products = company.type switch
                {
                    CompanyType.Denticom => page.DocumentNode.SelectNodes("//div[@id='catalog-products']//div[@class='item']"),
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
                
                foreach (var RawProduct in products)
                {
                    var parsedProduct = await TryParseProducts.GetProduct(RawProduct, category, company); 
                    parsedProducts.Add(parsedProduct);
                    count++;
                }
                pageNumber++;
            }

            Console.WriteLine($"For {company.Name} Found {count} products in {category.Name}");
            
        }

        var findResult = parsedProducts.Where(x => x.Name.Contains("адгезив kerr") ||x.Name.Contains("гиалуроновый гель"));
        foreach (var item in findResult)
        {
            Console.WriteLine($"Find: \"{item.Name}\"\n" +
                              $"Category: \"{item.Category}\"\n" +
                              $"Price: {item.Price}\n" +
                              $"Url: {item.Url}\n");
        }
        Console.ReadKey();
    }
}