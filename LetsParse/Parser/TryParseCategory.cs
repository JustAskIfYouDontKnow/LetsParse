using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LetsParse.Company;

namespace LetsParse.Parser;

public static class TryParseCategory
{
     public static async Task<List<Category.Category>> TryGetAllCategory(ICompany company)
        { 
            var web = new HtmlWeb();
            
            var categoryList = new List<Category.Category>();
            var document = await web.LoadFromWebAsync(company.baseUrl);
            var categories = document.DocumentNode.SelectNodes(company.CategoryNode);

            foreach (var category in categories)
            {
                var parsedCategory = new Category.Category()
                {
                    Name = company.type switch
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

                    Url = company.type switch
                    {
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
                };
                
                    categoryList.Add(parsedCategory);

            }
            return categoryList
                .GroupBy(c => new { c.Name, c.Url })
                .Select(group => group.First())
                .ToList();;
        }
}