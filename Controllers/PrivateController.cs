using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Core.Title.Models;

namespace MainBit.Users.Controllers
{
    [ValidateInput(false), Themed]
    public class PrivateController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IMembershipService _membershipService;

        public PrivateController(
            IContentManager contentManager,
            IOrchardServices services,
            IMembershipService membershipService)
        {
            _contentManager = contentManager;
            _services = services;
            _membershipService = membershipService;

             T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [Authorize]
        [AlwaysAccessible]
        public ActionResult Index() 
        {
            if (_services.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            IUser user = _services.WorkContext.CurrentUser;

            var viewModel = _services.New.ViewModel();
            viewModel.Editor =_contentManager.BuildEditor(user.ContentItem);
            viewModel.Title = _services.New.Parts_Title(Title: T("Personal data").Text);  // viewModel.Title = _services.New.Parts_Title(Title: T("Public profile").Text);
            return View((object)viewModel);
        }

        [Authorize]
        [AlwaysAccessible]
        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost()
        {
            IUser user = _services.WorkContext.CurrentUser;

            dynamic shape = _services.ContentManager.UpdateEditor(user.ContentItem, this);
            if (!ModelState.IsValid)
            {
                _services.TransactionManager.Cancel();
                return View("Index", (object)shape);
            }

            _services.Notifier.Information(T("Your profile has been saved."));

            return RedirectToAction("Index");
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