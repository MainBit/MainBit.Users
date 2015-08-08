using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Settings;
using Orchard.Users.Models;
using Orchard.Security;
using System.Xml.Linq;
using Orchard.Services;
using System.Globalization;
using System.Text;
using Orchard.Messaging.Services;
using Orchard.Environment.Configuration;
using Orchard.Users.Services;

namespace MainBit.Users.Services
{
    [UsedImplicitly]
    public class MainbitUserService : UserService, IUserService
    {
        private static readonly TimeSpan DelayToValidate = new TimeSpan(7, 0, 0, 0); // one week to validate email
        private static readonly TimeSpan DelayToResetPassword = new TimeSpan(1, 0, 0, 0); // 24 hours to reset password

        private readonly IContentManager _contentManager;
        private readonly IMembershipService _membershipService;
        private readonly IClock _clock;
        private readonly IMessageService _messageService;
        private readonly IEncryptionService _encryptionService;
        private readonly IShapeFactory _shapeFactory;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly ISiteService _siteService;

        public MainbitUserService(
            IContentManager contentManager, 
            IMembershipService membershipService, 
            IClock clock, 
            IMessageService messageService, 
            ShellSettings shellSettings, 
            IEncryptionService encryptionService,
            IShapeFactory shapeFactory,
            IShapeDisplay shapeDisplay,
            ISiteService siteService
            )
            : base(contentManager, membershipService, clock, messageService, shellSettings, encryptionService, shapeFactory, shapeDisplay, siteService)
        {
            _contentManager = contentManager;
            _membershipService = membershipService;
            _clock = clock;
            _messageService = messageService;
            _encryptionService = encryptionService;
            _shapeFactory = shapeFactory;
            _shapeDisplay = shapeDisplay;
            _siteService = siteService;
        }

        void IUserService.SendChallengeEmail(IUser user, Func<string, string> createUrl)
        {
            string nonce = CreateNonce(user, DelayToValidate);
            string url = createUrl(nonce);

            if (user != null) {
                var site = _siteService.GetSiteSettings();

                var template = _shapeFactory.Create("Template_User_Validated", Arguments.From(new {
                    RegisteredWebsite = site.As<RegistrationSettingsPart>().ValidateEmailRegisteredWebsite,
                    ContactEmail = site.As<RegistrationSettingsPart>().ValidateEmailContactEMail,
                    ChallengeUrl = url,
                    User = user
                }));
                template.Metadata.Wrappers.Add("Template_User_Wrapper");
                
                var parameters = new Dictionary<string, object> {
                            {"Subject", T("Verification E-Mail").Text},
                            {"Body", _shapeDisplay.Display(template)},
                            {"Recipients", user.Email}
                        };

                _messageService.Send("Email", parameters);
            }
        }
    }
}