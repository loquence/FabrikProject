using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
            
            
            string apiKey = ConfigurationManager.AppSettings["AvapiKey"];
            string tickers= "";
            string prev = "";
            foreach (var s in SortedList)
            {
                if (prev.Equals(""))
                {
                    tickers = tickers + s.AssetTicker;
                }
                else
                {
                    if (prev.Equals(s.AssetTicker))
                    {

                    }
                    else
                    {
                        tickers = tickers + s.AssetTicker;
                    }
                }
                prev = s.AssetTicker;
            }
            HttpClient client = new HttpClient();
            Uri uri = new Uri("https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols=" + tickers + "&apikey=" + apiKey);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                dynamic result = await response.Content.ReadAsAsync<object>();
                var jobject = JsonConvert.DeserializeObject<dynamic>(result);
                var batchstock = jobject["Stock Quotes"];
                foreach (var s in SortedList)
                {
                    
                }

            }

            


            foreach (var item in list)
            {
                Console.WriteLine(item.AssetTicker);
            }
            return PartialView(list);
        }

        
        
    }
}