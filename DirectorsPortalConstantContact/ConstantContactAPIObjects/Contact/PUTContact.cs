using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    //https://v3.developer.constantcontact.com/api_reference/index.html#!/Contacts/putContact

    public class PUTContact
    {
        public GETEmailAddress email_address = new GETEmailAddress();
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string company_name { get; set; }
        public int birthday_month { get; set; }
        public int birthday_day { get; set; }
        public string anniversary { get; set; }
        public string update_source { get; set; }

        public List<GETContactCustomField> custom_fields { get; set; }
        public List<POSTPhoneNumber> phone_numbers = new List<POSTPhoneNumber>();
        public List<POSTStreetAddress> street_addresses = new List<POSTStreetAddress>();
        public List<string> list_memberships = new List<string>();
    }

}
