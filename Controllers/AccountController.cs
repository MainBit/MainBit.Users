﻿using System;
using System.Text.RegularExpressions; 
using System.Diagnostics.CodeAnalysis;
using Orchard.Localization;
using System.Web.Mvc;
using System.Web.Security;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Services;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Utility.Extensions;
using Orchard;

namespace MainBit.Users.Controllers
{
    [HandleError, Themed]
    public class AccountController : Controller {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _orchardServices;
        private readonly IUserEventHandler _userEventHandler;

        public AccountController(
            IAuthenticationService authenticationService, 
            IMembershipService membershipService,
            IUserService userService, 
            IOrchardServices orchardServices,
            IUserEventHandler userEventHandler) {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userService = userService;
            _orchardServices = orchardServices;
            _userEventHandler = userEventHandler;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        int MinPasswordLength {
            get {
                return _membershipService.GetSettings().MinRequiredPasswordLength;
            }
        }

        [AlwaysAccessible]
        public ActionResult Register() {
            // ensure users can register
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
            if ( !registrationSettings.UsersCanRegister ) {
                return HttpNotFound();
            }

            ViewData["PasswordLength"] = MinPasswordLength;

            var shape = _orchardServices.New.Register();
            return new ShapeResult(this, shape); 
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword, string returnUrl = null) {
            // ensure users can register
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
            if ( !registrationSettings.UsersCanRegister ) {
                return HttpNotFound();
            }

            ViewData["PasswordLength"] = MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword)) {
                // Attempt to register the user
                // No need to report this to IUserEventHandler because _membershipService does that for us
                var user = _membershipService.CreateUser(new CreateUserParams(userName, password, email, null, null, false));

                if (user != null) {
                    if ( user.As<UserPart>().EmailStatus == UserStatus.Pending ) {
                        var siteUrl = _orchardServices.WorkContext.CurrentSite.BaseUrl;
                        if(String.IsNullOrWhiteSpace(siteUrl)) {
                            siteUrl = HttpContext.Request.ToRootUrlString();
                        }

                        _userService.SendChallengeEmail(user.As<UserPart>(), nonce => Url.MakeAbsolute(Url.Action("ChallengeEmail", "Account", new {Area = "Orchard.Users", nonce = nonce}), siteUrl));

                        _userEventHandler.SentChallengeEmail(user);
                        return RedirectToAction("ChallengeEmailSent");
                    }

                    if (user.As<UserPart>().RegistrationStatus == UserStatus.Pending) {
                        return RedirectToAction("RegistrationPending");
                    }

                    _authenticationService.SignIn(user, false /* createPersistentCookie */);
                    return this.RedirectLocal(returnUrl);
                }
                
                ModelState.AddModelError("_FORM", T(ErrorCodeToString(/*createStatus*/MembershipCreateStatus.ProviderError)));
            }

            // If we got this far, something failed, redisplay form
            var shape = _orchardServices.New.Register();
            return new ShapeResult(this, shape); 
        }

        [AlwaysAccessible]
        public ActionResult RequestLostPassword()
        {
            // ensure users can request lost password
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
            if (!registrationSettings.EnableLostPassword)
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        [AlwaysAccessible]
        public ActionResult RequestLostPassword(string username)
        {
            // ensure users can request lost password
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
            if (!registrationSettings.EnableLostPassword)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("userNameOrEmail", T("Invalid username or E-mail."));
                return View();
            }

            if (_userService.VerifyUserUnicity(username, username))
            {
                ModelState.AddModelError("userExists", T("User with that username and/or email doesn't exists."));
                return View();
            }

            var siteUrl = _orchardServices.WorkContext.CurrentSite.BaseUrl;
            if (String.IsNullOrWhiteSpace(siteUrl))
            {
                siteUrl = HttpContext.Request.ToRootUrlString();
            }

            _userService.SendLostPasswordEmail(username, nonce => Url.MakeAbsolute(Url.Action("LostPassword", "Account", new { Area = "Orchard.Users", nonce = nonce }), siteUrl));

            _orchardServices.Notifier.Information(T("Check your e-mail for the recovery password link."));

            return RedirectToAction("LogOn", "Account", new { area = "Orchard.Users" });
        }

        #region Validation Methods

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword) {
            bool validate = true;

            if (String.IsNullOrEmpty(userName)) {
                ModelState.AddModelError("username", T("You must specify a username."));
                validate = false;
            }
            else {
                if (userName.Length >= 255) {
                    ModelState.AddModelError("username", T("The username you provided is too long."));
                    validate = false;
                }
            }

            if (String.IsNullOrEmpty(email)) {
                ModelState.AddModelError("email", T("You must specify an email address."));
                validate = false;
            }
            else if (email.Length >= 255) {
                ModelState.AddModelError("email", T("The email address you provided is too long."));
                validate = false;
            }
            else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase)) {
                // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                ModelState.AddModelError("email", T("You must specify a valid email address."));
                validate = false;
            }

            if (!validate)
                return false;

            if (!_userService.VerifyUserUnicity(userName, email)) {
                ModelState.AddModelError("userExists", T("User with that username and/or email already exists."));
            }
            if (password == null || password.Length < MinPasswordLength) {
                ModelState.AddModelError("password", T("You must specify a password of {0} or more characters.", MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal)) {
                ModelState.AddModelError("_FORM", T("The new password and confirmation password do not match."));
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}