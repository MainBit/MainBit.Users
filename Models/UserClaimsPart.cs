using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Users.Models
{
    public class UserClaimsPart : ContentPart
    {
        public string AccessToken
        {
            get { return this.Retrieve(x => x.AccessToken); }
            set { this.Store(x => x.AccessToken, value); }
        }

        public string ExpiresAt
        {
            get { return this.Retrieve(x => x.ExpiresAt); }
            set { this.Store(x => x.ExpiresAt, value); }
        }

        public string RefreshToken
        {
            get { return this.Retrieve(x => x.RefreshToken); }
            set { this.Store(x => x.RefreshToken, value); }
        }

        public string IdentityToken
        {
            get { return this.Retrieve(x => x.IdentityToken); }
            set { this.Store(x => x.IdentityToken, value); }
        }

        public string Subject
        {
            get { return this.Retrieve(x => x.Subject); }
            set { this.Store(x => x.Subject, value); }
        }

        public string SessionId
        {
            get { return this.Retrieve(x => x.SessionId); }
            set { this.Store(x => x.SessionId, value); }
        }

    }
}