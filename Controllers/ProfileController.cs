using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace MainBit.Users.Controllers
{
    [ValidateInput(false), Themed]
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IMembershipService _membershipService;

        public ProfileController(IOrchardServices services,
            IMembershipService membershipService)
        {

            _membershipService = membershipService;

            Services = services;
        }

        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index(int id)
        {
            var user = Services.ContentManager.Get<IUser>(id);
            if (user == null || !Services.Authorizer.Authorize(Permissions.ViewProfiles, user, null))
            {
                return HttpNotFound();
            }

            dynamic shape = Services.ContentManager.BuildDisplay(user.ContentItem);

            return View((object)shape);
        }
        
        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}