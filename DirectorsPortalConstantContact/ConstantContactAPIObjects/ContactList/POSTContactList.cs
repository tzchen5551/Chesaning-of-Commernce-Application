using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    //https://v3.developer.constantcontact.com/api_reference/index.html#!/Contact_Lists/createList
    public class POSTContactList
    {
        public string name { get; set; }
        public bool favorite { get; set; }
        public string description { get; set; }
    }
}