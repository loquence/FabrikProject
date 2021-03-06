﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FabrikProject.Models;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FabrikProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, FabrikProject.Models.ApplicationDbContext context)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var portfolio = context.PortfolioMeta.Find(model.Email);
                        var list = context.UserStock.Where(r => r.Email == model.Email).ToList();
                        double II = 0;
                        foreach (var s in list)
                        {
                            
                            if (s.AssetType.Equals("Stock", StringComparison.InvariantCultureIgnoreCase))
                            {
                                s.Sector = GetSector(s.AssetTicker);
                            }
                            II += s.InitialInvestment;
                        }
                        int size = list.Count();
                        if (portfolio == null)
                        {
                            PortfolioMeta pm = new PortfolioMeta { Email = model.Email, InitialInvestmen = II, TotalAssets = size, StartDate = DateTime.Now };
                            context.PortfolioMeta.Add(pm);
                        }
                        else
                        {
                            var p = context.PortfolioMeta.Find(model.Email);
                            p.InitialInvestmen = II;
                            p.TotalAssets = size;
                        }
                        await context.SaveChangesAsync();
                        
                       
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

       

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, FabrikProject.Models.ApplicationDbContext context)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
                    var st = DateTime.Now;
                    PortfolioMeta pm = new PortfolioMeta { Email = model.Email, InitialInvestmen = 0, TotalAssets = 0, StartDate = st };
                    context.PortfolioMeta.Add(pm);
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    Debug.WriteLine("About to call the SendEmailAsync function ");
                    
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("ConfirmEmail", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Register");
        }

        private AddStockViewModel GetStockInfo(string type, string ticker)
        {
            WebRequest request;
            AddStockViewModel toadd = new AddStockViewModel();
            
            WebResponse response;
            Stream dataStream;//= response.GetResponseStream();
            StreamReader reader; //= new StreamReader(dataStream);

            if (type.Equals("Stock", StringComparison.InvariantCultureIgnoreCase))
            {
                string IEX = "https://api.iextrading.com/1.0/stock/" + ticker + "/price";
                request = WebRequest.Create(IEX);
                request.Credentials = CredentialCache.DefaultCredentials;
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                var responseFromServer = reader.ReadToEnd();
                double currentprice = Convert.ToDouble(responseFromServer);
                toadd.CurrentPrice = currentprice;
                string comp = "https://api.iextrading.com/1.0/stock/" + ticker + "/company";
                request = WebRequest.Create(comp);
                request.Credentials = CredentialCache.DefaultCredentials;
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                toadd.Company = new CompanyInfo { Symbol = json["symbol"], CompanyName = json["companyName"], Exchange = json["exchange"], Industry = json["industry"], Website = json["website"], CEO = json["CEO"], Description = json["description"], Sector = json["sector"] };
                

                //count++;
            }
            if (type.Equals("Crypto", StringComparison.InvariantCultureIgnoreCase))
            {
                string crypto = "https://min-api.cryptocompare.com/data/price?fsym=" + ticker + "&tsyms=USD";
                request = WebRequest.Create(crypto);
                request.Credentials = CredentialCache.DefaultCredentials;
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                var responseFromServer = reader.ReadToEnd();
                var cpt = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                var price = cpt["USD"];
                double currentprice = Convert.ToDouble(price);
                toadd.CurrentPrice = currentprice;
            }
            return toadd;
        }

        [AllowAnonymous]
        public ActionResult CompanyInfo(string ticker, string type)
        {
            var ret = GetStockInfo(type, ticker);
            return PartialView(ret);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddStock(string assetticker,string assetname, string assettype)
        {
            ViewBag.CompanyUrl = "/Account/CompanyInfo?ticker=" + assetticker + "&type=" + assettype;
            ViewBag.NameType =assettype + " - " + assetname;
            ViewBag.Name = assetname;
            ViewBag.Ticker = assetticker; 
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddStock(UserStockViewModel model, FabrikProject.Models.ApplicationDbContext context)
        {
            if (ModelState.IsValid)
            {
                var asset = model.AssetName.Split('-');
                asset[0] = asset[0].Trim();
                asset[1] = asset[1].Trim();
                var stock = new UserStock { Email = User.Identity.GetUserName(), AssetType = asset[0], AssetTicker = model.AssetTicker, AssetName = asset[1], DatePurchased = model.DatePurchased, Commissions = model.Commissions, InitialInvestment = model.InitialInvestment, Quantity = model.Quantity };
                string sector = "";
                if (stock.AssetType.Equals("stock", StringComparison.InvariantCultureIgnoreCase))
                {
                    sector = GetSector(stock.AssetTicker);
                }
                stock.Sector = sector;
                context.UserStock.Add(stock);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult UserAddStock()
        {
            
            return View();
        }

        [AllowAnonymous]
        public ActionResult StockList()
        {
            List<Csv> model = new List<Csv>();
            model = ReturnStockTable();
            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult _NamePartial()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var name = user.FirstName;
            ViewBag.Name = name;
            return PartialView();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserAddStock(ICollection<UserStockViewModel> model, FabrikProject.Models.ApplicationDbContext context)
        {
            
            if (ModelState.IsValid)
            {
                foreach (var m in model)
                {
                    var asset = m.AssetName.Split('-');
                    asset[0]=asset[0].Trim();
                    asset[1] = asset[1].Trim();
                    
                    
                    var stock = new UserStock { AssetType = asset[0], AssetName = asset[1], AssetTicker = m.AssetTicker, Email = User.Identity.GetUserName(), Quantity = m.Quantity, DatePurchased = m.DatePurchased, InitialInvestment = m.InitialInvestment, SharePrice = m.SharePrice, Commissions = m.Commissions };
                    string sector = "";
                    if(stock.AssetType.Equals("stock", StringComparison.InvariantCultureIgnoreCase))
                    {
                        sector = GetSector(stock.AssetTicker);
                    }
                    stock.Sector = sector;
                    
                    context.UserStock.Add(stock);
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch(System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach(var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach(var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                    validationErrors.Entry.Entity.ToString(),
                    validationError.ErrorMessage);
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                    
                }
                
                return RedirectToAction("Index", "Home");
                
            }
            return View("Home");
            
        }

        private string GetSector(string ticker)
        {
            WebRequest request;
            AddStockViewModel toadd = new AddStockViewModel();
            
            WebResponse response;
            Stream dataStream;//= response.GetResponseStream();
            StreamReader reader; //= new StreamReader(dataStream);
            string comp = "https://api.iextrading.com/1.0/stock/" + ticker + "/company";
            request = WebRequest.Create(comp);
            request.Credentials = CredentialCache.DefaultCredentials;
            response = request.GetResponse();
            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            var json = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            return json["sector"];
        }
        [HttpPost]
        public ActionResult AddStockP(AddStock model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.CompanyUrl = "/Account/CompanyInfo?ticker=" + model.AssetTicker + "&type=" + model.AssetType;
                ViewBag.NameType = model.AssetType + " - " + model.AssetName;
                ViewBag.Name = model.AssetName;
                ViewBag.Ticker = model.AssetName;

                return View("AddStock");
            }
            return View("Error");
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(FabrikProject.Models.ApplicationDbContext context)
        {
            string search = Request.Form["search"].ToLower();
            var list = ReturnStockTable();
            List<Models.Csv> slist = new List<Models.Csv>();
            foreach(var k in list)
            {
                string name = k.AssetName.ToLower();
                string ticker = k.AssetTicker.ToLower();
                if (k.AssetName.StartsWith(search,StringComparison.OrdinalIgnoreCase) || k.AssetTicker.StartsWith(search,StringComparison.OrdinalIgnoreCase))
                {
                    slist.Add(k);
                }
            }
            if (slist.Count() > 100)
            {
                return Json(new { String ="narrow your search" });

            }
            return Json(new { slist });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Searcher(FabrikProject.Models.ApplicationDbContext context)
        {
            string radio = Request.Form["asset"];
            if (radio == null)
                radio = "";
            else
                radio = radio.ToLower();
            string search = Request.Form["search"].ToLower();
            var list = ReturnStockTable();
            List<Models.Csv> slist = new List<Models.Csv>();
            
            foreach (var k in list)
            {
                string name = k.AssetName.ToLower();
                string ticker = k.AssetTicker.ToLower();
                string type = k.AssetType.ToLower();
                if ((k.AssetName.StartsWith(search, StringComparison.OrdinalIgnoreCase) || k.AssetTicker.StartsWith(search, StringComparison.OrdinalIgnoreCase)) && type.Contains(radio) )
                {
                    slist.Add(k);
                }
            }
            if (slist.Count() > 100)
            {
                return Json(new { String = "narrow your search" });

            }
            return Json(new { slist,radio });
        }
        private List<Models.Csv> ReturnStockTable()
        {
            string path = Server.MapPath("~/App_Data/assets.csv");
            using (var reader = new System.IO.StreamReader(path))
            {
                List<FabrikProject.Models.Csv> list = new List<Models.Csv>();
                var count = 0;
                while (!reader.EndOfStream)
                {


                    var lin = reader.ReadLine();
                    var values = lin.Split(',');
                    if (count != 0)
                    {
                        Models.Csv temp = new Models.Csv();
                        temp.AssetTicker = values[0];
                        temp.AssetName = values[1];
                        temp.AssetType = values[2];
                        list.Add(temp);
                    }
                    else
                    {
                        count++;
                    }
                    
                }
                return list;
            }

        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [AllowAnonymous]
        public ActionResult Edit(int id, FabrikProject.Models.ApplicationDbContext context)
        {
            UserStock stock = context.UserStock.Find(id);
            return View(stock);
        }

        [AllowAnonymous]
        public ActionResult Delete(int id, FabrikProject.Models.ApplicationDbContext context)
        {
            UserStock stock = context.UserStock.Find(id);
            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(UserStock model, FabrikProject.Models.ApplicationDbContext context)
        {
            if (!ModelState.IsValid)
            {
                var toDelete = context.UserStock.Find(model.ID);
                context.UserStock.Remove(toDelete);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
        [AllowAnonymous]
        public ActionResult Performance(FabrikProject.Models.ApplicationDbContext context)
        {
            PortfolioMeta pm = context.PortfolioMeta.Find(User.Identity.GetUserName());
            return View(pm);
        }
        
        [AllowAnonymous]
        public ActionResult Performancer(FabrikProject.Models.ApplicationDbContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.GetUserName();
                var list = context.UserStock.Where(r => r.Email == email).ToList();
                var SortedList = list.OrderBy(s => s.AssetTicker).ToList();
                return View(SortedList);
            }
            else
            {
                return View("Login", "Account");
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}