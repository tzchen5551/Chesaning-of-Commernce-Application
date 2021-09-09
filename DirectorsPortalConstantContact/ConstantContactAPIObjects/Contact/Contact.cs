using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    /// <summary>
    /// the main Contact object refrence point. This will have a few helper properties and meathods, but will 
    /// mainly be here for holding the data that has been retrieved from constant contact 
    /// as laid out in the GETContact.cs file. 
    /// </summary>
    public class Contact : GETContact
    {
        ///this will also have a toPUT and toPOST to return the object that be serialized for the coresponding request

        [Newtonsoft.Json.JsonIgnore]
        public List<ContactList> glstContactLists = new List<ContactList>();
        [Newtonsoft.Json.JsonIgnore]
        public List<GETCustomField> glstCustomFields = new List<GETCustomField>();

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, List<EmailCampaignActivity>> gdctTracking = new Dictionary<string, List<EmailCampaignActivity>>() 
            {
                {"em_sends", new List<EmailCampaignActivity>()},
                {"em_opens", new List<EmailCampaignActivity>()},
                {"em_clicks", new List<EmailCampaignActivity>()},
                {"em_bounces", new List<EmailCampaignActivity>()},
                {"em_optouts", new List<EmailCampaignActivity>()},
                {"em_forwards", new List<EmailCampaignActivity>()}
            };

        public int included_activities_count;
        public double open_rate;
        public double click_rate;


        [Newtonsoft.Json.JsonIgnore]
        public string strFullname => this.first_name + " " + this.last_name;


        //https://www.c-sharpcorner.com/article/encryption-and-decryption-using-a-symmetric-key-in-c-sharp/

        /// <summary>
        /// constuctor to init all needed feilds
        /// </summary>
        public Contact()
        {
            this.email_address = new GETEmailAddress()
            {
                address = null
            };
            this.first_name = null;
            this.last_name = null;
            this.custom_fields = new List<GETContactCustomField>();
            this.phone_numbers = new List<GETPhoneNumber>();
            this.street_addresses = new List<GETStreetAddress>();
            this.list_memberships = new List<string>();
        }

        /// <summary>
        /// optional paramaterized init
        /// </summary>
        /// <param name="strEmailAddress">email</param>
        /// <param name="strFirstName">first name</param>
        /// <param name="strLastName">last name</param>
        public Contact(string strEmailAddress = null, string strFirstName = null, string strLastName = null)
        {
            this.email_address = new GETEmailAddress() 
            { 
                address = strEmailAddress
            };
            this.first_name = strFirstName;
            this.last_name = strLastName;
            this.custom_fields = new List<GETContactCustomField>();
            this.phone_numbers = new List<GETPhoneNumber>();
            this.street_addresses = new List<GETStreetAddress>();
            this.list_memberships = new List<string>();

        }


        /// <summary>
        /// translates the current contact to a PUTContact for use with the API
        /// </summary>
        /// <returns></returns>
        public PUTContact Update()
        {
            PUTContact objUpdateContact = new PUTContact();

            objUpdateContact.email_address.address = this.email_address.address;
            objUpdateContact.email_address.permission_to_send = this.email_address.permission_to_send;
            objUpdateContact.email_address.opt_out_reason = this.email_address.opt_out_reason;

            objUpdateContact.first_name = this.first_name;
            objUpdateContact.last_name = this.last_name;
            objUpdateContact.job_title = this.job_title;
            objUpdateContact.company_name = this.company_name;
            objUpdateContact.birthday_month = this.birthday_month;
            objUpdateContact.birthday_day = this.birthday_day;
            objUpdateContact.anniversary = this.anniversary;
            objUpdateContact.update_source = "Account";
            objUpdateContact.list_memberships = this.list_memberships;

            foreach (GETPhoneNumber objPhone in this.phone_numbers)
            {
                POSTPhoneNumber objTemp = new POSTPhoneNumber();
                objTemp.phone_number = objPhone.phone_number;
                objTemp.kind = objPhone.kind ?? "";
                objUpdateContact.phone_numbers.Add(objTemp);
            }

            foreach (GETContactCustomField objField in this.custom_fields)
            {
                GETContactCustomField objTemp = new GETContactCustomField();
                objTemp.custom_field_id = objField.custom_field_id;
                objTemp.value = objField.value;
                objUpdateContact.custom_fields.Add(objTemp);
            }

            foreach (GETStreetAddress objAddress in this.street_addresses)
            {
                POSTStreetAddress objTemp = new POSTStreetAddress();
                objTemp.street = objAddress.street;
                objTemp.kind = objAddress.kind;
                objTemp.city = objAddress.city;
                objTemp.state = objAddress.state;
                objTemp.postal_code = objAddress.postal_code;
                objTemp.country = objAddress.country;
                objUpdateContact.street_addresses.Add(objTemp);
            }

            return objUpdateContact;

        }

        /// <summary>
        /// translates to a POSTContact for use with the API
        /// </summary>
        /// <returns></returns>
        public POSTContact Create()
        {
            if (string.IsNullOrEmpty(this.first_name) && string.IsNullOrEmpty(this.last_name) && string.IsNullOrEmpty(this.email_address.address))
            {
                Console.WriteLine("Contact missing fields");
                throw new Exception();

            }

            POSTContact objTempContact = new POSTContact()
            {
                first_name = this.first_name,
                last_name = this.last_name,
                job_title = this.job_title,
                company_name = this.company_name,
                create_source = "Account",
                anniversary = this.anniversary,
            };

            objTempContact.birthday_month = this.birthday_month != 0 ? this.birthday_month : new Nullable<int>();

            objTempContact.birthday_day = this.birthday_day != 0 ? this.birthday_day : new Nullable<int>();

            objTempContact.email_address = new POSTEmailAddress();
            objTempContact.email_address.address = this.email_address.address;
            objTempContact.email_address.permission_to_send = this.email_address.permission_to_send;

            if (this.custom_fields != null && this.custom_fields.Count > 0)
            {
                objTempContact.custom_fields = new List<GETContactCustomField>();
                foreach (GETContactCustomField tempField in this.custom_fields)
                {
                    objTempContact.custom_fields.Add(tempField);
                }
            }

            if (this.phone_numbers != null && this.phone_numbers.Count > 0)
            {
                objTempContact.phone_numbers = new List<POSTPhoneNumber>();
                foreach (GETPhoneNumber tempPhone in this.phone_numbers)
                {
                    POSTPhoneNumber tempNewPhone = new POSTPhoneNumber()
                    {
                        phone_number = tempPhone.phone_number,
                        kind = tempPhone.kind
                    };
                    objTempContact.phone_numbers.Add(tempNewPhone);
                }
            }
            
            if (this.street_addresses != null && this.street_addresses.Count > 0)
            {
                objTempContact.street_addresses = new List<POSTStreetAddress>();
                foreach (GETStreetAddress tempAddress in this.street_addresses)
                {
                    POSTStreetAddress tempNewAddress = new POSTStreetAddress()
                    {
                        kind = tempAddress.kind,
                        street = tempAddress.street,
                        city = tempAddress.city,
                        state = tempAddress.state,
                        postal_code = tempAddress.postal_code,
                        country = tempAddress.country
                    };
                    objTempContact.street_addresses.Add(tempNewAddress);
                }
            }
            
            if (this.list_memberships != null && this.list_memberships.Count > 0)
            {
                objTempContact.list_memberships = new List<string>();
                foreach (string strListId in this.list_memberships)
                {
                    objTempContact.list_memberships.Add(strListId);
                }
            }
            

            

            return objTempContact;
        }

    }
}
