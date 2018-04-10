using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FabrikProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FabrikProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Strategies()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Updates()
        {
            ViewBag.Message = "Our Updates Page";

            return View();
        }

        [ChildActionOnly]
        public async Task<ActionResult> _StockTable(FabrikProject.Models.ApplicationDbContext context)
        {
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(r => r.Email == email).ToList();
            var SortedList = list.OrderBy(s => s.AssetTicker).ToList();
            List<StockViewModel> SortedStock = new List<StockViewModel>();


            double totalInitialInvestment = 0;
            string apiKey = ConfigurationManager.AppSettings["AvapiKey"];
            string tickers= "";
            string cryptos = "";
            string prev = "";
            foreach (var s in SortedList)
            {
                totalInitialInvestment += s.InitialInvestment + s.Commissions;
                if (prev.Equals(""))
                {
                    if (s.AssetType.Equals("Stock") || true)
                    {
                        tickers = tickers + s.AssetTicker;
                    }
                    else
                    {
                        cryptos = cryptos + s.AssetTicker;
                    }
                }
                else
                {
                    if (prev.Equals(s.AssetTicker))
                    {

                    }
                    else
                    {
                        if (s.AssetType.Equals("Stock") || true)
                        {
                            tickers = tickers + s.AssetTicker;
                        }
                        else
                        {
                            cryptos = cryptos + s.AssetTicker;
                        }
                    }
                }
                prev = s.AssetTicker;
            }

            WebRequest request = WebRequest.Create("https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols=MSFT,FB,AAPL&apikey=demo");
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            var stock = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            var batchstock = stock["Stock Quotes"];
            int count = 0;
            double totalVal = 0;
            double totalReturns = 0;
            foreach (var s in SortedList)
            {
                if (s.AssetType.Equals("Stock") || true)
                {
                    double currentprice = Convert.ToDouble(batchstock[0]["2. price"]);
                    double value = s.Quantity * currentprice;
                    totalVal += value;
                    double prereturns = value / (s.InitialInvestment + s.Commissions);
                    double returns = (prereturns - 1) * 100;  
                    StockViewModel model = new StockViewModel { CurrentPrice = currentprice, userstock = s, Value = value, Returns = returns };
                    SortedStock.Add(model);
                    count++;
                }
            }

            double tPreReturns = (totalInitialInvestment > 0)? totalVal / totalInitialInvestment : 0;
            if (tPreReturns > 0)
            {
                totalReturns = (tPreReturns - 1) * 100; 
            }
            else
            {
                totalReturns = 0;
            }
            StockTableViewModel allInfo = new StockTableViewModel { list = SortedStock, Returns = totalReturns, InitialInvestment = totalInitialInvestment, TotalValue = totalVal };
           




            foreach (var item in list)
            {
                Console.WriteLine(item.AssetTicker);
            }
            return PartialView(allInfo);
        }

        
        
    }
}