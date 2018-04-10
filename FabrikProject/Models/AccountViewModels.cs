﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace FabrikProject.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class UserStock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        /*
        [ForeignKey("Email")]
        public virtual ApplicationUser User { get; set; }
        */
        [Required]
        [DataType(DataType.Text)]
        [Display(Name ="Asset Ticker")]
        public string AssetTicker { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name ="Asset Name")]
        public string AssetName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name ="Asset Type")]
        public string AssetType { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name ="Date Purchased")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DatePurchased { get; set; }

        [Required]
        [Display(Name ="Initial Investment")]
        [DataType(DataType.Currency)]
        public double InitialInvestment { get; set; }

        [Required]
        [Display(Name ="Share Price")]
        [DataType(DataType.Currency)]
        public double SharePrice { get; set; }

        [Required]
        [Display(Name ="Commissions")]
        [DataType(DataType.Currency)]
        public double Commissions { get; set; }
    }

    public class UserStockViewModel
    {
        [Required]
        public string AssetTicker { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime DatePurchased { get; set; }
        [Required]
        public double InitialInvestment { get; set; }
        [Required]
        public string AssetName { get; set; }
        [Required]
        public double SharePrice { get; set; }
        [Required]
        public double Commissions { get; set; }

    }

    public class StockViewModel
    {
        [Required]
        public UserStock userstock;

        [Required]
        [Display(Name ="Mkt Price")]
        [DataType(DataType.Currency)]
        public double CurrentPrice { get; set; }

        [Required]
        [Display(Name = "Value")]
        [DataType(DataType.Currency)]
        public double Value { get; set; }

        [Required]
        [Display(Name = "Returns")]
        [DisplayFormat(DataFormatString = "{0:#,##0.000#}", ApplyFormatInEditMode = true)]
        public double Returns { get; set; }
    }

    public class StockTableViewModel
    {
        [Required]
        public IEnumerable<StockViewModel> list;

        [Required]
        [Display(Name ="Value")]
        [DataType(DataType.Currency)]
        public double TotalValue { get; set; }

        [Required]
        [Display(Name ="Returns")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double Returns { get; set; }

        [Required]
        [Display(Name = "Initial Investment")]
        [DataType(DataType.Currency)]
        public double InitialInvestment { get; set; }
    }

    public class Csv
    {
        [Required]
        public string AssetName { get; set; }
        [Required]
        public string AssetTicker { get; set; }
        [Required]
        public string AssetType { get; set; }
    }

 



   
}
