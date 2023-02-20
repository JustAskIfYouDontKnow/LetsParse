using System;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LetsParse.Company;

namespace LetsParse.Parser;

public static class TryParseMaxPage
{
    public static async Task<int> GetMaxPage(CompanyType companyType, string baseUrl)
    {
        var result = companyType switch
        {
            CompanyType.Denticom =>  GetMaxPageDenticom(baseUrl),
            CompanyType.Greendent =>  GetMaxPageGreenDent(baseUrl),
            // CompanyType.Dentex => expr,
            // CompanyType.StomShop => expr,
            // CompanyType.GoldiDent => expr,
            // CompanyType.OMKMedical => expr,
            // CompanyType.Amfodent => expr,
            // CompanyType.KupiDenatal => expr,
            // CompanyType.StomDevice => expr,
            _ => throw new ArgumentOutOfRangeException(nameof(companyType), companyType, null)
        };
        
        return await result;
    }

    private static async Task<int> GetMaxPageDenticom(string baseUrl)
    {
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(baseUrl);
        try
        {
            var items = doc.DocumentNode.SelectSingleNode("//button[@class='bottom-button ripple']");
            return int.TryParse(items.GetAttributeValue("data-max", null), out var page) ? page : 1;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Ошибка, не удалось спарсить максимальное количество страниц в категории \"{baseUrl}\", возвращено значение 1 по умолчанию");
            Console.ForegroundColor = ConsoleColor.White;
            const int page = 1;
            return page;
          
        }

    }
    
    private static async Task<int> GetMaxPageGreenDent(string baseUrl)
    {
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(baseUrl);
       

        try
        {
            var links = doc.DocumentNode.SelectSingleNode("//div[@class='system-pagenavigation-items']").Descendants("a");
            return links.Select(link => { return int.TryParse(link.InnerText, out int page) ? page : 1; })
                .Where(page => page != null)
                .Max();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Ошибка, не удалось спарсить максимальное количество страниц в категории \"{baseUrl}\", возвращено значение 1 по умолчанию");
            Console.ForegroundColor = ConsoleColor.White;
            const int page = 1;
            return page;
        }
       
    }
}