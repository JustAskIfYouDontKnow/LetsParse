using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LetsParse.Company;

namespace LetsParse.Parser;

public static class TryParseProducts
{
    public static async Task<Product.Product> GetProduct(HtmlNode RawProduct, Category.Category category, ICompany company)
    {
        try
        {
            var parsedProduct = company.type switch
            {
                CompanyType.Denticom => new Product.Product()
                {
                    Name = RawProduct.SelectSingleNode(".//a[@class='name']").InnerText.Trim().ToLower(),
                    Price = RawProduct.SelectSingleNode(".//div[@class='price']/text()[1]")?.InnerText.Replace(" ", "").Trim().ToLower(),
                    Url = company.baseUrl + RawProduct.SelectSingleNode(".//a[@class='name']").GetAttributeValue("href", "").ToLower(),
                    Company = company.Name.ToLower(),
                    Category = category.Name.ToLower()
                },
                CompanyType.Greendent => new Product.Product()
                {
                    Name = RawProduct.SelectSingleNode(".//a[contains(@class, 'intec-cl-text-hover')]").InnerText.Trim().ToLower(),
                    Price = RawProduct.SelectSingleNode(".//div[contains(@class, 'catalog-section-item-price-base')]").InnerText.Trim().ToLower(),
                    Category = category.Name.ToLower()
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
            return parsedProduct;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка парсинга элементов{RawProduct}" + e);
            throw;
        }
    }
}