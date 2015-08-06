using System;
using System.Web.Security;
using Orchard.ContentManagement.Drivers;
using Orchard.Users.Models;
using MainBit.Users.Models;
using Orchard.ContentManagement;

namespace Orchard.Users.Drivers {
    /// <summary>
    /// This class intentionnaly has no Display method to prevent external access to this information through standard 
    /// Content Item display methods.
    /// </summary>
    public class UserPersonalDataPartDriver : ContentPartDriver<UserPersonalDataPart>
    {
        protected override string Prefix
        {
            get { return "UserPersonalDataPart"; }
        }

        protected override DriverResult Display(UserPersonalDataPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_UserPersonalData",
                    () => shapeHelper.Parts_UserPersonalData())
                );
        }

        protected override DriverResult Editor(UserPersonalDataPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_UserPersonalData",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts/UserPersonalData", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(UserPersonalDataPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(UserPersonalDataPart part, ContentManagement.Handlers.ImportContentContext context)
        {
            part.FirstName = context.Attribute(part.PartDefinition.Name, "FirstName");
            part.MiddleName = context.Attribute(part.PartDefinition.Name, "MiddleName");
            part.LastName = context.Attribute(part.PartDefinition.Name, "LastName");
        }

        protected override void Exporting(UserPersonalDataPart part, ContentManagement.Handlers.ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FirstName", part.FirstName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MiddleName", part.MiddleName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("LastName", part.LastName);
        }
    }
}