﻿using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using MainBit.Users.Models;

namespace MainBit.Users.Handlers {
    public class UserPersonalDataPartHandler : ContentHandler {
        public UserPersonalDataPartHandler(IRepository<UserPersonalDataPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<UserPersonalDataPart>("User"));
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<UserPersonalDataPart>();

            //if (part != null) {
            //    context.Metadata.Identity.Add("User.UserName", part.UserName);
            //    context.Metadata.DisplayText = part.UserName;
            //}
        }
    }
}