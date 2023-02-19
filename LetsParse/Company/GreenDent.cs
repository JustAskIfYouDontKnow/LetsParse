using System;

namespace LetsParse.Company
{
    public class GreenDent : ICompany
    {
        public string Name { get; set; } = nameof(GreenDent);
        public string baseUrl { get; set; } = "https://mos.greendent.ru";

        public string CategoryNode { get; set; } = "//div[@class='submenu']/div[@class='item']";

        public CompanyType type { get; set; } = CompanyType.Greendent;
        
    }
}