using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using MainBit.Users.Models;

namespace MainBit.Users.Handlers {
    public class UserClaimsPartHandler : ContentHandler {
        public UserClaimsPartHandler(IRepository<UserPersonalDataPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<UserClaimsPart>("User"));
        }
    }
}