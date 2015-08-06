using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Security;
using Orchard.Users.Models;

namespace MainBit.Users.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ProfileEditSocial(this UrlHelper urlHelper) {
            return urlHelper.Action("Edit", "Home", new { area = "Opt.Profiles" });
        }

        // view profile
        public static string ProfileOwnerView(this UrlHelper urlHelper, ContentItem contentItem) {
            
            return ProfileView(urlHelper, contentItem.As<CommonPart>().Owner);
        }
        public static string ProfileOwnerView(this UrlHelper urlHelper, CommonPart commonPart) {
            return ProfileView(urlHelper, commonPart.Owner);
        }
        public static string ProfileView(this UrlHelper urlHelper, IUser userPart) {
            return urlHelper.Action("Index", "Home", new { id = userPart.Id, area = "Opt.Profiles" });
        }
    }
}