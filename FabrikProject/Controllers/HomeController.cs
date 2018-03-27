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
        public ActionResult Index(FabrikProject.Models.ApplicationDbContext context)
        {
            
            var email = User.Identity.GetUserName();
            var list = context.UserStock.Where(r => r.Email == email).ToList();
            
            return View(list);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}