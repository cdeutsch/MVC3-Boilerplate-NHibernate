using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Routing;
using Web.Infrastructure.FormsAuthenticationService;
using Web.Common;
using NHibernate;
using Web.Models.ViewModels;
using Web.Infrastructure.Repositories;

namespace Web.Controllers
{
    public class SessionController : Controller
    {
        public IFormsAuthenticationService FormsAuthService { get; set; }
        public IMembershipService MembershipService { get; set; }

        private ISession _session;
        private UserRepository _userRepository;
        UserActivity _log;

        public SessionController(ISession session, IFormsAuthenticationService FormsAuthService, IMembershipService MembershipService)
        {
            _session = session;
            _userRepository = new UserRepository(_session);
            _log = new UserActivity(_session);

            this.FormsAuthService = FormsAuthService;
            this.MembershipService = MembershipService;
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Default Create action is to do a Login.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ComboSignupLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserLogin.LoginName, model.UserLogin.Password))
                {
                    //get user.
                    User user = _userRepository.GetUser(model.UserLogin.LoginName);
                    if (user != null)
                    {
                        //log that the user logged in.
                        _log.LogIt(user.Id, "User Logged In");

                        FormsAuthService.SignIn(user.Id.ToString(), model.UserLogin.RememberMe);

                        //cache user data.
                        CacheHelper.CacheUserData(FormsAuthService, user);

                        return Redirect(user, returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User info could not be found.");
                        this.FlashValidationSummaryErrors();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    this.FlashValidationSummaryErrors();
                }
            }
            else
            {
                this.FlashValidationSummaryErrors();
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(ComboSignupLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserNew.UserName, model.UserNew.Password, model.UserNew.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    try
                    {
                        //finish the registration that the MembershipProvider did not handle.

                        User user = _userRepository.CompleteRegistration(model.UserNew.UserName, model.UserNew.FirstName, model.UserNew.LastName);
                        
                        //log that user registered.
                        _log.LogIt(user.Id, "User registered");

                        this.FlashInfo("Thank you for signing up!");

                        FormsAuthService.SignIn(user.Id.ToString(), false /* createPersistentCookie */);

                        //cache user data.
                        CacheHelper.CacheUserData(FormsAuthService, user);

                        return Redirect(user, returnUrl);
                    }
                    catch (Exception exp)
                    {
                        ModelState.AddModelError("", exp.Message);
                        this.FlashValidationSummaryErrors();
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    this.FlashValidationSummaryErrors();
                }
            }
            else
            {
                this.FlashValidationSummaryErrors();
            }

            // If we got this far, something failed, redisplay form
            return View("login", model);
        }

        //Logout
        public ActionResult Logout()
        {
            _log.LogIt(FormsAuthService.GetCurrentUserId(), "Logged out");
            FormsAuthService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //we need the username for this to work so get the user.
                User user = _userRepository.GetUser(FormsAuthService.GetCurrentUserId());
                if (MembershipService.ChangePassword(user.Username, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    this.FlashValidationSummaryErrors();
                }
            }
            else
            {
                this.FlashValidationSummaryErrors();
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected ActionResult Redirect(User user, string returnUrl)
        {
            //redirect to specified url or default.
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }

}
