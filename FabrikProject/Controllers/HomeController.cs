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
        public ActionResult Index(FabrikProject.Models.ApplicationDbContext context)
        {
            
            PortfolioMeta pm = context.PortfolioMeta.Find(User.Identity.GetUserName());
            return View(pm);
        }
        
        [ChildActionOnly]
        public PartialViewResult RegisterPartial()
        {
            return PartialView();
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

        [AllowAnonymous]
        public ActionResult _StockTable(FabrikProject.Models.ApplicationDbContext context)
        {
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(r => r.Email == email).ToList();
            var SortedList = list.OrderBy(s => s.AssetTicker).ToList();
            List<StockViewModel> SortedStock = new List<StockViewModel>();

            PortfolioMeta pm = context.PortfolioMeta.Find(email);
            
            List<string> cryptos = new List<string>();
            
            int size = SortedList.Count();
            double totalVal = 0;

            //string alphavantage = "https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols=" + tickers + "&apikey=BECFPZPO9R3JIDAN";
            SortedStock = GetPrices(SortedList,ref totalVal);
            
            foreach (var s in SortedStock)
            {
                s.Weight = (s.Value / totalVal) * 100;
            }
            
            StockTableViewModel allInfo = new StockTableViewModel { list = SortedStock};
           




            foreach (var item in list)
            {
                Console.WriteLine(item.AssetTicker);
            }
            return PartialView(allInfo);
        }
        

        public ActionResult TotalValue(FabrikProject.Models.ApplicationDbContext context)
        {
            string email = User.Identity.GetUserName();
            var list = context.UserStock.Where(s => s.Email == email);
            var SortedList = list.OrderBy(s => s.AssetTicker).ToList();
            double ttv = 0;
            var pricelist = GetPrices(SortedList,ref ttv);
            ViewBag.TotalValue = ttv;
            return PartialView();
        }
        public ActionResult Returns(FabrikProject.Models.ApplicationDbContext context)
        {
            string email = User.Identity.GetUserName();
            var list = context.UserStock.Where(s => s.Email == email);
            double returns;
            if (list.Count() == 0)
            {
                returns = 0;
                ViewBag.Returns = returns;
                return PartialView();
            }
            var portfolio = context.PortfolioMeta.Find(email);
            var SortedList = list.OrderBy(s => s.AssetTicker).ToList();
            double ttv = 0;
            var pricelist = GetPrices(SortedList, ref ttv);
           
            if (portfolio.InitialInvestmen == 0)
            {
                returns = 0;
            }
            else
            {
                returns = (ttv / portfolio.InitialInvestmen - 1) * 100;
            }
           
            ViewBag.Returns = returns;
            return PartialView();
        }

        public ActionResult Chart(FabrikProject.Models.ApplicationDbContext context)
        {
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(s => s.Email == email);
            var slist = list.OrderBy(s => s.AssetTicker).ToList();
            double ttv = 0;
            var pricelist = GetPrices(slist, ref ttv);
            double stockp = 0;
            double cryptop = 0;
            foreach(var s in pricelist)
            {
                if(s.userstock.AssetType.Equals("Stock", StringComparison.InvariantCultureIgnoreCase))
                {
                    stockp += s.Value;
                }
                else
                {
                    cryptop += s.Value;
                }
            }
            return Json(new { stocks = stockp, cryptos = cryptop });
        }

        public ActionResult PerformanceChartStacked(FabrikProject.Models.ApplicationDbContext context)
        {
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(s => s.Email == email);
            var slist = list.OrderBy(s => s.AssetTicker).ToList();
            double ttv = 0;
            var pricelist = GetPricesStacked(slist, ref ttv);
            
            return Json(new { pricelist });
        }

        private List<StockViewModel> GetPrices(List<UserStock> list, ref double ttv)
        {
            WebRequest request;
            List<StockViewModel> SortedStock = new List<StockViewModel>();

            WebResponse response;
            Stream dataStream;//= response.GetResponseStream();
            StreamReader reader; //= new StreamReader(dataStream);

            //var stock = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            //var batchstock = stock["Stock Quotes"];
            //int count = 0;
            double totalVal = 0;
            
            foreach (var s in list)
            {
                if (s.AssetType.Equals("Stock", StringComparison.InvariantCultureIgnoreCase))
                {
                    string IEX = "https://api.iextrading.com/1.0/stock/" + s.AssetTicker + "/price";
                    request = WebRequest.Create(IEX);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    reader = new StreamReader(dataStream);
                    var responseFromServer = reader.ReadToEnd();
                    double currentprice = Convert.ToDouble(responseFromServer);
                    double value = s.Quantity * currentprice;
                    totalVal += value;
                    double prereturns = value / (s.InitialInvestment + s.Commissions);
                    double returns = (prereturns - 1) * 100;
                    StockViewModel model = new StockViewModel { CurrentPrice = currentprice, userstock = s, Value = value, Returns = returns };
                    SortedStock.Add(model);
                    //count++;
                }
                if (s.AssetType.Equals("Crypto", StringComparison.InvariantCultureIgnoreCase))
                {
                    string crypto = "https://min-api.cryptocompare.com/data/price?fsym=" + s.AssetTicker + "&tsyms=USD";
                    request = WebRequest.Create(crypto);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    reader = new StreamReader(dataStream);
                    var responseFromServer = reader.ReadToEnd();
                    var cpt = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    var price = cpt["USD"];
                    double currentprice = Convert.ToDouble(price);
                    double value = s.Quantity * currentprice;
                    totalVal += value;
                    double prereturns = value / (s.InitialInvestment + s.Commissions);
                    double returns = (prereturns - 1) * 100;
                    StockViewModel model = new StockViewModel { CurrentPrice = currentprice, userstock = s, Value = value, Returns = returns };
                    SortedStock.Add(model);
                }
            }
            ttv = totalVal;
            return SortedStock;

        }
        private List<StackedChartReturn> GetPricesStacked(List<UserStock> list, ref double ttv)
        {
            WebRequest request;
            List<StackedChartReturn> SortedStock = new List<StackedChartReturn>();

            WebResponse response;
            Stream dataStream;//= response.GetResponseStream();
            StreamReader reader; //= new StreamReader(dataStream);

            //var stock = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            //var batchstock = stock["Stock Quotes"];
            //int count = 0;
            double totalVal = 0;

            foreach (var s in list)
            {
                if (s.AssetType.Equals("Stock", StringComparison.InvariantCultureIgnoreCase))
                {
                    string IEX = "https://api.iextrading.com/1.0/stock/" + s.AssetTicker + "/price";
                    request = WebRequest.Create(IEX);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    reader = new StreamReader(dataStream);
                    var responseFromServer = reader.ReadToEnd();
                    double currentprice = Convert.ToDouble(responseFromServer);
                    double value = s.Quantity * currentprice;
                    totalVal += value;
                    double prereturns = value / (s.InitialInvestment + s.Commissions);
                    double returns = (prereturns - 1) * 100;
                    StackedChartReturn model = new StackedChartReturn { Value = value, AssetName = s.AssetName };
                    SortedStock.Add(model);
                    //count++;
                }
                if (s.AssetType.Equals("Crypto", StringComparison.InvariantCultureIgnoreCase))
                {
                    string crypto = "https://min-api.cryptocompare.com/data/price?fsym=" + s.AssetTicker + "&tsyms=USD";
                    request = WebRequest.Create(crypto);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    reader = new StreamReader(dataStream);
                    var responseFromServer = reader.ReadToEnd();
                    var cpt = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    var price = cpt["USD"];
                    double currentprice = Convert.ToDouble(price);
                    double value = s.Quantity * currentprice;
                    totalVal += value;
                    double prereturns = value / (s.InitialInvestment + s.Commissions);
                    double returns = (prereturns - 1) * 100;
                    StackedChartReturn model = new StackedChartReturn { Value = value, AssetName = s.AssetName };
                    SortedStock.Add(model);
                }
            }
            ttv = totalVal;
            return SortedStock;

        }
    }
}