﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FabrikProject.Models;

namespace FabrikProject.Controllers
{
    public class ApplicationBaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(User != null)
            {
                var context = new ApplicationDbContext();
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = context.Users.SingleOrDefault(u => u.UserName == username);
                    string fullName = string.Concat(new String[] { user.FirstName, " ", user.LastName });
                    ViewData.Add("FullName", fullName);
                }

            }
            base.OnActionExecuted(filterContext);
        }
        public ApplicationBaseController()
        {

        }

    }

}