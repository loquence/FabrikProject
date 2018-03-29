using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

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
        public ActionResult _StockTable(FabrikProject.Models.ApplicationDbContext context)
        {
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(r => r.Email == email).ToList();
            foreach (var item in list)
            {
                Console.WriteLine(item.Stock);
            }
            return PartialView(list);
        }
    }
}