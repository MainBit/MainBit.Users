using Orchard;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Users.Events
{
    public class UserEventHandler : IUserEventHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthenticationService _authenticationService;

        public UserEventHandler(IOrchardServices orchardServices,
            IAuthenticationService authenticationService)
        {
            _orchardServices = orchardServices;
            _authenticationService = authenticationService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Creating(UserContext context)
        {
        }

        public void Created(UserContext context)
        {
        }

        public void LoggingIn(string userNameOrEmail, string password)
        {

        }

        public void LoggedIn(Orchard.Security.IUser user)
        {

        }

        public void LogInFailed(string userNameOrEmail, string password)
        {

        }

        public void LoggedOut(Orchard.Security.IUser user)
        {

        }

        public void AccessDenied(Orchard.Security.IUser user)
        {

        }

        public void ChangedPassword(Orchard.Security.IUser user)
        {
            // not needed because after change password ChangePasswordSuccess action executes
            //_orchardServices.Notifier.Information(T("Your password successfully changed."));

            // it's not safe? If you know recovery password link you may don't know email or login.
            //if (_authenticationService.GetAuthenticatedUser() == null)
            //{
            //    _authenticationService.SignIn(user, false /* createPersistentCookie */);
            //}
        }

        public void SentChallengeEmail(Orchard.Security.IUser user)
        {

        }

        public void ConfirmedEmail(Orchard.Security.IUser user)
        {

        }

        public void Approved(Orchard.Security.IUser user)
        {

        }
    }
}