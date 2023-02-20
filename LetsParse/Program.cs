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
                // new GreenDent(),
                
            };

            foreach (var company in companyList)
            {
                await Parser.Parser.TryParse(company);
            }
          
        }
        
        
        
       

     
        
       
    }
}