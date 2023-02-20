using System;

namespace LetsParse.Company
{
    public class Denticom : ICompany
    {
        public string Name { get; set; } = nameof(Denticom);
        public string baseUrl { get; set; } = "https://dentikom.ru";
        public string CategoryNode { get; set; } = "//nav[@class='no_m']//a[@href]";// "//div[@class='col']/a[@class='has-sub']";     //"//a[@class='has-sub' and not(ancestor::ul[@class='sub'])]"; //Full "//div[@class='col']//a[@class='has-sub']";
        public CompanyType type { get; set; } = CompanyType.Denticom;
    }
}