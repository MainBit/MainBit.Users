using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Users.Models
{
    public class UserPersonalDataPart : ContentPart<UserPersonalDataPartRecord>
    {
        public string FirstName
        {
            get { return Retrieve(x => x.FirstName); }
            set { Store(x => x.FirstName, value); }
        }

        public string MiddleName
        {
            get { return Retrieve(x => x.MiddleName); }
            set { Store(x => x.MiddleName, value); }
        }

        public string LastName
        {
            get { return Retrieve(x => x.LastName); }
            set { Store(x => x.LastName, value); }
        }
    }
}