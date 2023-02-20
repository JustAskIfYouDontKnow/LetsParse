using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LetsParse.Parser;

public static class SaveResults
{
    public static async Task SaveResultsTxt(List<Product.Product> products)
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
        }
    }
}