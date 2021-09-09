using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    //https://v3.developer.constantcontact.com/api_reference/index.html#!/Contacts/createContact
    public class POSTEmailAddress
    {
        public string address { get; set; }
        public string permission_to_send { get; set; }
    }


    public class POSTPhoneNumber
    {
        public string phone_number { get; set; }
        public string kind { get; set; }
    }

    public class POSTStreetAddress
    {
        public string kind { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class POSTContact
    {
        public POSTEmailAddress email_address { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string company_name { get; set; }
        public string create_source { get; set; }
        public int? birthday_month { get; set; }
        public int? birthday_day { get; set; }
        public string anniversary { get; set; }
        public List<GETContactCustomField> custom_fields { get; set; }
        public List<POSTPhoneNumber> phone_numbers { get; set; }
        public List<POSTStreetAddress> street_addresses { get; set; }
        public List<string> list_memberships { get; set; }
    }

}
