using System;

namespace LetsParse.Company
{
    public class Denticom : ICompany
    {
        public string Name { get; set; } = nameof(Denticom);
        public string baseUrl { get; set; } = "https://dentikom.ru";
        public string CategoryNode { get; set; } = "//div[@class='col']/a"; //Full "//div[@class='col']//a[@class='has-sub']";
        public CompanyType type { get; set; } = CompanyType.Denticom;
    }
}