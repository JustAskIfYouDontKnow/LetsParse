using System;

namespace LetsParse.Company
{
    public interface ICompany
    {
        string Name { get; set; } 
        string baseUrl { get; set; }
        string CategoryNode { get; set; }
        CompanyType type { get; set; }
    }
}