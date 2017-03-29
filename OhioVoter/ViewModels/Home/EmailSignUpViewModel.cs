using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Home
{
    public class EmailSignUpViewModel
    {
        public EmailSignUpViewModel() { }

        public EmailSignUpViewModel(string controllerName)
        {
            ControllerName = controllerName;
        }


        public string ControllerName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsValidated { get; set; }

        public bool IsActive { get; set; }
    }
}