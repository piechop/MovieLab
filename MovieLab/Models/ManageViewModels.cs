﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MovieLab.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

        [Display(Name = "Reviews Written")]
        public int ReviewCount { get; set; }

        [Display(Name = "Average rating for user")]
        public int UserRating { get; set; }

        [Display(Name = "Your favorite movie")]
        public string FavoriteMovie { get; set; }

        [Display(Name = "Your favorite genre")]
        public string FavoriteGenre { get; set; }
    }

    public class EditProfileView
    {
        [Display(Name = "Your favorite movie")]
        public string FavoriteMovie { get; set; }

        [Display(Name = "Your favorite genre")]
        public string FavoriteGenre { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password Confirmation")]
        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password Confirmation")]
        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Telephone")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class ReviewViewModel
    {
        public ReviewViewModel()
        {
            MovieID = 0;
        }
        public int ID { get; set; }

        [Display(Name = "Review Title")]
        [MaxLength(50, ErrorMessage = "Title must be 50 characters or less.")]
        public string ReviewTitle { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Content")]
        public string ReviewText { get; set; }

        [Display(Name = "Movie Rating")]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 100.")]
        public byte MovieRating { get; set; }

        [Display(Name = "Review Rating")]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 100.")]
        public byte ReviewRating { get; set; }

        public int? Ratings { get; set; }

        [Display(Name = "Date Created")]
        public System.DateTime ReviewTime { get; set; }

        public string Author { get; set; }

        [Display(Name = "Movie")]
        public int MovieID { get; set; }

        [Display(Name = "Movie")]
        public string MovieTitle { get; set; }

    }
}