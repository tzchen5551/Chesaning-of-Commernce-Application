using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DirectorsPortalConstantContact
{
    //https://v3.developer.constantcontact.com/api_reference/index.html#!/Contacts/getContact
    public class GETEmailAddress
    {
        public string address { get; set; }
        public string permission_to_send { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string opt_in_source { get; set; }
        public DateTime opt_in_date { get; set; }
        public string opt_out_source { get; set; }
        public DateTime opt_out_date { get; set; }
        public string opt_out_reason { get; set; }
        public string confirm_status { get; set; }
    }

    public class GETContactCustomField
    {
        public string custom_field_id { get; set; }
        public string value { get; set; }
    }

    public class GETPhoneNumber
    {
        public string phone_number_id { get; set; }
        public string phone_number { get; set; }
        public string kind { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string update_source { get; set; }
        public string create_source { get; set; }
    }

    public class GETStreetAddress
    {
        public string street_address_id { get; set; }
        public string kind { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class GETContact
    {
        public string contact_id { get; set; }
        public GETEmailAddress email_address { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string company_name { get; set; }
        public int birthday_month { get; set; }
        public int birthday_day { get; set; }
        public string anniversary { get; set; }
        public string update_source { get; set; }
        public string create_source { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string deleted_at { get; set; }
        public List<GETContactCustomField> custom_fields { get; set; }
        public List<GETPhoneNumber> phone_numbers { get; set; }
        public List<GETStreetAddress> street_addresses { get; set; }
        public List<string> list_memberships { get; set; }

    }



}
