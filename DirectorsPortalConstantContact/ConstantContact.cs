using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Globalization;

namespace DirectorsPortalConstantContact
{
    /// <summary>
    /// This is the main Constant Contact interface. after you give the .Authenticate() command, 
    ///     you will be able to access the attribiutes that have been queried from Constant Contact API
    /// </summary>
    public class ConstantContact
    {
        readonly private string gstrBaseURL = "https://api.cc.email/v3/";
        readonly private string gstrContactsUrl = "contacts?include=custom_fields,list_memberships,phone_numbers,street_addresses&limit=500";
        readonly private string gstrContactListUrl = "contact_lists?limit=1000";
        readonly private string gstrContactCustomFieldUrl = "contact_custom_fields?limit=100";
        readonly private string gstrEmailCampaignUrl = "emails?limit=500"; 
        private readonly object lckUpdateLock = new object();
        private bool isUpdating = false;


        public Dictionary<string, Contact> gdctContacts = new Dictionary<string, Contact>();
        public Dictionary<string, ContactList> gdctContactLists = new Dictionary<string, ContactList>();
        public Dictionary<string, CustomField> gdctCustomFields = new Dictionary<string, CustomField>();
        public Dictionary<string, EmailCampaign> gdctEmailCampaigns = new Dictionary<string, EmailCampaign>();
        public Dictionary<string, EmailCampaignActivity> gdctEmailCampaignActivities = new Dictionary<string, EmailCampaignActivity>();

        private ConstantContactOAuth gobjCCAuth = new ConstantContactOAuth();

        private string mstrTokenHeader => $"Bearer {this.gobjCCAuth.MstrAccessToken}";


        // add properties for dictionaries

        public Dictionary<string, Contact>.ValueCollection Contacts => this.gdctContacts.Values;
        public Dictionary<string, ContactList>.ValueCollection ContactLists => this.gdctContactLists.Values;
        public Dictionary<string, CustomField>.ValueCollection CustomFields => this.gdctCustomFields.Values;
        public Dictionary<string, EmailCampaign>.ValueCollection EmailCampaigns => this.gdctEmailCampaigns.Values;
        public Dictionary<string, EmailCampaignActivity>.ValueCollection EmailCampaignActivities => this.gdctEmailCampaignActivities.Values;

        public List<EmailCampaignActivityPreview> glstEmailCampaignActivityPreviews = new List<EmailCampaignActivityPreview>();

        public bool PopupAsked = false;

        /// <summary>
        /// Constuctor. Attempts to load the last cached data
        /// </summary>
        public ConstantContact()
        {

            try
            {
                this.LoadData();
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        public bool SignedIn
        {
            get
            {
                if (gobjCCAuth.MstrAccessToken != null && gobjCCAuth.MstrRefreshToken != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Updating
        {
            get
            {
                return this.isUpdating;
            }
        }


        /// <summary>
        /// One function to run all of the update function. Preforms thread sleeps to avoid going over the Constant Contact API Rate Limit
        ///
        /// </summary>
        public void RefreshData()
        {
            lock (this.lckUpdateLock)
            {
                this.isUpdating = true;
                this.gobjCCAuth.ValidateAuthentication();

                this.UpdateContacts();
                System.Threading.Thread.Sleep(300);

                this.UpdateContactLists();

                /*this.UpdateContactCustomFields();
                System.Threading.Thread.Sleep(300);

                this.UpdateEmailCampaigns();
                System.Threading.Thread.Sleep(200);

                this.UpdateEmailCampaignActivities();
                System.Threading.Thread.Sleep(200);

                this.UpdateEmailCampaignActivityPreviews();*/

                this.ContactListAssignment();
                //this.CustomFieldAssignment();

                this.CacheData();
                this.isUpdating = false;
            }
            


        }

        public void ValidateAuth()
        {
            this.gobjCCAuth.ValidateAuthentication();
        }

        /// <summary>
        /// Talks to this.ReadJsonFromUrl with the url for GETContacts and 
        ///     creates the coresponding objects that we are given
        /// </summary>
        private void UpdateContacts()
        {
            string strLink = this.gstrContactsUrl;

            Dictionary<string, Contact> dctTempContacts = new Dictionary<string, Contact>();

            while (true) {
                string strJson = this.ReadJsonFromUrl(strLink);

                JObject objJson = JObject.Parse(strJson);
                string strContactList = objJson.GetValue("contacts").ToString();

                List<Contact> lstDecodedJson = JsonConvert.DeserializeObject<List<Contact>>(strContactList);

                foreach (Contact objContact in lstDecodedJson)
                {
                    dctTempContacts.Add(objContact.contact_id, objContact);
                }
                
                try
                {
                    strLink = (string)objJson["_links"]["next"]["href"];
                    strLink = strLink.Substring(4, strLink.Length - 4);
                }
                catch (System.NullReferenceException)
                {
                    this.gdctContacts = dctTempContacts;
                    this.CacheData();
                    return;
                }
                
            }
            

        }
        /// <summary>
        /// Talks to this.ReadJsonFromUrl with the url for GETContactList and 
        ///     creates the coresponding objects that we are given
        /// </summary>
        private void UpdateContactLists()
        {
            //fix for while True
            string strJson = this.ReadJsonFromUrl(this.gstrContactListUrl);


            Dictionary<string, ContactList> dctTempContactLists = new Dictionary<string, ContactList>();

            //Console.WriteLine(strJson);

            Dictionary<string, List<ContactList>> dctDecodedJson = JsonConvert.DeserializeObject<Dictionary<string, List<ContactList>>>(strJson);

            foreach (ContactList lstContactList in dctDecodedJson["lists"])
            {
                dctTempContactLists.Add(lstContactList.list_id, lstContactList);
            }

            this.gdctContactLists = dctTempContactLists;
            this.CacheData();
            return;
        }

        /// <summary>
        /// Talks to this.ReadJsonFromUrl with the url for GETCustomFields and 
        ///     creates the coresponding objects that we are given
        /// </summary>
        private void UpdateContactCustomFields()
        {
            string strJson = this.ReadJsonFromUrl(this.gstrContactCustomFieldUrl);

            Dictionary<string, CustomField> dctTempCustomFields = new Dictionary<string, CustomField>();

            Dictionary<string, List<CustomField>> dctDecodedJson = JsonConvert.DeserializeObject<Dictionary<string, List<CustomField>>>(strJson);

            foreach (CustomField lstFieldList in dctDecodedJson["custom_fields"])
            {
                dctTempCustomFields.Add(lstFieldList.custom_field_id, lstFieldList);
            }

            this.gdctCustomFields = dctTempCustomFields;
            this.CacheData();
            return;
        }

        /// <summary>
        /// Talks to this.ReadJsonFromUrl with the url for EmailCampaigns and 
        ///     creates the coresponding objects that we are given
        /// </summary>
        private void UpdateEmailCampaigns()
        {
            string strLink = this.gstrEmailCampaignUrl;
            //this should make a temp dict and use that instead of just erasing the data first. 
            Dictionary<string, EmailCampaign> dctTempEmailCampaigns = new Dictionary<string, EmailCampaign>();
            while (true)
            {
                //json response for all campaigns
                string strJson = this.ReadJsonFromUrl(strLink);

                //seperate the campaigns key from the other json data
                JObject objJson = JObject.Parse(strJson);
                string strLists = objJson.GetValue("campaigns").ToString();

                List<EmailCampaign> dctDecodedJson = JsonConvert.DeserializeObject<List<EmailCampaign>>(strLists);

                foreach (EmailCampaign lstEmailCampaign in dctDecodedJson)
                {
                    if (lstEmailCampaign.current_status != "Removed")
                    {
                        dctTempEmailCampaigns.Add(lstEmailCampaign.campaign_id, lstEmailCampaign);
                    }
                }

                try
                {
                    strLink = (string)objJson["_links"]["next"]["href"];
                    strLink = strLink.Substring(4, strLink.Length - 4);
                }
                catch (System.NullReferenceException)
                {
                    this.gdctEmailCampaigns = dctTempEmailCampaigns;
                    this.CacheData();
                    return;
                }
            }
        }

        /// <summary>
        ///     update all of the activities for each campaign, with as much info as we can get. 
        /// </summary>
        private void UpdateEmailCampaignActivities()
        {
            //loop through all of the campaigns to get their id, then request all of that campaigns activities

            gdctEmailCampaignActivities = new Dictionary<string, EmailCampaignActivity>();
            foreach (EmailCampaign objCampaign in this.gdctEmailCampaigns.Values)
            {
                string strUrl = $"emails/{objCampaign.campaign_id}";
                objCampaign.Activities = new List<EmailCampaignActivity>();

                string strJson = this.ReadJsonFromUrl(strUrl);

                JObject objJson = JObject.Parse(strJson);
                string strActivities = objJson.GetValue("campaign_activities").ToString();
                List<ActivityList> lstDecodedJson = JsonConvert.DeserializeObject<List<ActivityList>>(strActivities);
                objCampaign.campaign_activities = lstDecodedJson;

                foreach (ActivityList objTempActivity in lstDecodedJson)
                {

                    string strActivityUrl = $"emails/activities/{objTempActivity.campaign_activity_id}";
                    string strActivityJson = this.ReadJsonFromUrl(strActivityUrl);
                    EmailCampaignActivity objActivity = JsonConvert.DeserializeObject<EmailCampaignActivity>(strActivityJson);
                    objActivity.gobjCampaign = objCampaign;
                    objCampaign.Activities.Add(objActivity);
                    gdctEmailCampaignActivities.Add(objActivity.campaign_activity_id, objActivity);

                    //addcontact lists to activity for reference
                    foreach (string strId in objActivity.contact_list_ids)
                    {
                        objActivity.glstContactLists.Add(this.gdctContactLists[strId]);
                    }


                }
                System.Threading.Thread.Sleep(250);

            }
            this.CacheData();
        }

        /// <summary>
        /// Updates the local Previews of the Campaigns and saves them
        /// </summary>
        private void UpdateEmailCampaignActivityPreviews()
        {
            this.glstEmailCampaignActivityPreviews = new List<EmailCampaignActivityPreview>();
            foreach (EmailCampaign objCampaign in this.gdctEmailCampaigns.Values)
            {
                foreach(EmailCampaignActivity objActivity in objCampaign.Activities)
                {
                    //get scheduals for the activity
                    //only promary email will have sched list
                    if (objActivity.role == "primary_email")
                    {
                        string strUrl = $"emails/activities/{objActivity.campaign_activity_id}/previews";
                        string strData = this.ReadJsonFromUrl(strUrl);

                        EmailCampaignActivityPreview objPreview = JsonConvert.DeserializeObject<EmailCampaignActivityPreview>(strData);

                        objPreview.activity = objActivity;

                        objActivity.mobjPreview = objPreview;
                        this.glstEmailCampaignActivityPreviews.Add(objPreview);

                            
                    }
                    
                }
            }
            this.CacheData();
        }

        /// <summary>
        /// Not currently used, Future Implementation
        /// </summary>
        private void UpdateContactTrackingReporting()
        {
            //current data set does not meet the finctionality of this function yet
            foreach (Contact objContact in this.Contacts)
            {
            //Contact objContact = this.FindContactByEmail("edwalk@svsu.edu");
                string strUrl = $"reports/contact_reports/{objContact.contact_id}/activity_details?tracking_activities_list=em_sends,em_opens,em_clicks,em_bounces,em_optouts,em_forwards";
                string strResponce = this.ReadJsonFromUrl(strUrl);

                JObject objJson = JObject.Parse(strResponce);
                string strTracking = objJson.GetValue("tracking_activities").ToString();

                List<Dictionary<string, string>> lstTrackingEvents = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(strTracking);

                foreach (Dictionary<string, string> dctEvent in lstTrackingEvents)
                {
                    objContact.gdctTracking[dctEvent["tracking_activity_type"]].Add(this.gdctEmailCampaignActivities[dctEvent["campaign_activity_id"]]);
                }
                //Console.WriteLine(lstTrackingEvents.ToString());
                System.Threading.Thread.Sleep(250);
            }

        }

        /// <summary>
        /// Not Currently used. Future Implemetation
        /// </summary>
        private void UpdateContactOpenRate()
        {
            //current data set does not meet the finctionality of this function yet
            foreach (Contact objContact in this.Contacts)
            {
                //Contact objContact = this.FindContactByEmail("edwalk@svsu.edu");
                string strStart = DateTime.Now.AddYears(-5).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.ffZ", CultureInfo.InvariantCulture);
                string strEnd = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffZ", CultureInfo.InvariantCulture);
                string strUrl = $"reports/contact_reports/{objContact.contact_id}/open_and_click_rates?start={strStart}&end={strEnd}"; 
                string strResponce = this.ReadJsonFromUrl(strUrl);


                Dictionary<string, string> dctRating = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResponce);

                objContact.open_rate = Convert.ToDouble(dctRating["average_open_rate"]);
                objContact.click_rate = Convert.ToDouble(dctRating["average_click_rate"]);
                objContact.included_activities_count = Convert.ToInt32(dctRating["included_activities_count"]);

                System.Threading.Thread.Sleep(250);

            }
        }

        /// <summary>
        /// Used to make a web request and retrieve the JSON from the given URL
        /// </summary>
        /// 
        /// [TODO] this function will be changed to use the HttpClient class in order to make it cleaner. 
        /// 
        /// <param name="strUrl"> The URL extention that the request is to be made to </param>
        /// <returns></returns>
        private string ReadJsonFromUrl(string strUrl)
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.mstrTokenHeader);

            HttpResponseMessage objResponse = client.GetAsync(this.gstrBaseURL + strUrl).Result;

            return objResponse.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Used to update an api object with the url and json to put. 
        /// </summary>
        /// <param name="strJson">json to use as the data</param>
        /// <param name="strUrl">url to PUT to</param>
        private void PUTJson(string strJson, string strUrl)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.mstrTokenHeader);
            
            var data = new StringContent(strJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(this.gstrBaseURL + strUrl, data).Result;
        }

        /// <summary>
        /// Function to add contacts to CC
        /// </summary>
        /// <param name="strJson">contact string json</param>
        /// <param name="strUrl">post url</param>
        private void PostJson(string strJson, string strUrl)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.mstrTokenHeader);

            var data = new StringContent(strJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(this.gstrBaseURL+strUrl, data).Result;
        }

        /// <summary>
        /// Used to delete a contact by its ID
        /// </summary>
        /// <param name="contact_id">string ID of contact to delete</param>
        private async void DELETEContact(string contact_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.mstrTokenHeader);

            var response = await client.DeleteAsync(this.gstrBaseURL + $"contacts/{contact_id}");
        }

        /// <summary>
        /// Loops though the contacts and contact lists to create in memory refrences for easy access
        /// </summary>
        private void ContactListAssignment()
        {
            //loop through all contacts
            foreach (KeyValuePair<string, Contact> objContact in this.gdctContacts)
            {
                //loop through all lists that the contact is apart of and add to that contacts glstContactLists
                foreach (string strListId in objContact.Value.list_memberships)
                {
                    
                    //also add contact to the list's members list
                    if (!this.gdctContactLists[strListId].glstMembers.Contains(objContact.Value))
                    {
                        this.gdctContactLists[strListId].glstMembers.Add(objContact.Value);
                        objContact.Value.glstContactLists.Add(this.gdctContactLists[strListId]);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Loops through the contacts and assigns them with the custom feilds that they have
        /// </summary>
        private void CustomFieldAssignment()
        {
            //loop through all contacts
            foreach (KeyValuePair<string, Contact> objContact in this.gdctContacts)
            {
                //loop through the custom fields that they have and add references
                foreach (GETContactCustomField objField in objContact.Value.custom_fields)
                {
                    if (!objContact.Value.glstCustomFields.Contains(this.gdctCustomFields[objField.custom_field_id])){
                        objContact.Value.glstCustomFields.Add(this.gdctCustomFields[objField.custom_field_id]);
                        this.gdctCustomFields[objField.custom_field_id].glstContacts.Add(objContact.Value);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Called after LoadCache to correctly assign activities to campaigns in memory
        /// </summary>
        private void LocalActivityAssignments()
        {
            foreach(EmailCampaign objCampaign in this.EmailCampaigns)
            {
                foreach(EmailCampaignActivity objActivity in this.EmailCampaignActivities)
                {
                    //where objTmp != null select
                    if (objCampaign.campaign_activities != null)
                    {
                        if ((from objTmp in objCampaign.campaign_activities select objTmp.campaign_activity_id).Contains(objActivity.campaign_activity_id))
                        {
                            objActivity.gobjCampaign = objCampaign;
                            objCampaign.Activities.Add(objActivity);

                            //addcontact lists to activity for reference
                            foreach (string strId in objActivity.contact_list_ids)
                            {
                                objActivity.glstContactLists.Add(this.gdctContactLists[strId]);
                            }
                        }
                    }
                    
                }
            }
        }
        
        
        /// <summary>
        /// Called after LoadCache to correctly assign previews to activities in memory
        /// </summary>
        private void LocalPreviewAssignment()
        {
           foreach(EmailCampaignActivityPreview objPreview in this.glstEmailCampaignActivityPreviews)
           {
                EmailCampaignActivity objActivity = this.gdctEmailCampaignActivities[objPreview.campaign_activity_id];
                objPreview.activity = objActivity;
                objActivity.mobjPreview = objPreview;

           }
        }
        
        /// <summary>
        /// Given an email, returns the contact object 
        /// </summary>
        /// <param name="strEmail">string email</param>
        /// <returns>contact object, null if no match</returns>
        public Contact FindContactByEmail(string strEmail)
        {
            foreach (Contact objContact in this.gdctContacts.Values)
            {
                if (objContact.email_address.address.Trim() == strEmail.Trim())
                {
                    return objContact;
                }
            }
            return null;
        }

        /// <summary>
        /// return ContactList object by its Campaign name
        /// </summary>
        /// <param name="strname">Name of the Campaign </param>
        /// <returns>contact List, null if not found</returns>
        public ContactList FindListByName(string strname)
        {
            foreach (ContactList objList in this.gdctContactLists.Values){
                if (objList.name == strname.Trim())
                {
                    return objList;
                }
            }
            return null;
        }

        /// <summary>
        /// return Campaign Object by its campaign id
        /// </summary>
        /// <param name="strName">Name of the campaign</param>
        /// <returns></returns>
        public EmailCampaign FindCampaignByName(string strName)
        {
            foreach (EmailCampaign objTemp in this.gdctEmailCampaigns.Values)
            {
                if (objTemp.name == strName.Trim())
                {
                    return objTemp;
                }
            }
            return null;
        }

        /// <summary>
        /// Return a campaign given its name
        /// </summary>
        /// <param name="strName">name of the campaign</param>
        /// <returns>CampaignActivity</returns>
        public EmailCampaignActivity FindCampaignActivityByName(string strName)
        {
            foreach (EmailCampaignActivityPreview objTemp in this.glstEmailCampaignActivityPreviews)
            {
                if (objTemp.strCampaignName == strName.Trim())
                {
                    return objTemp.activity;
                }
            }
            return null;
        }

        /// <summary>
        /// provided an existing contact in the list, update that contact in Constant Contact
        /// </summary>
        /// <param name="objContact">Contact to update</param>
        public void Update(Contact objContact)
        {
            PUTContact objTemp = objContact.Update();
            string strUrl = $"contacts/{objContact.contact_id}";

            string strJson = JsonConvert.SerializeObject(objTemp,new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore
            });
            this.PUTJson(strJson, strUrl);
            this.CacheData();
        }


        /// <summary>
        /// Given a Contact List, update it on constant contact
        /// </summary>
        /// <param name="objContactList">Contact to update</param>
        public void Update(ContactList objContactList)
        {
            PUTContactList objTemp = objContactList.Update();
            string strUrl = $"contact_lists/{objContactList.list_id}";

            string strJson = JsonConvert.SerializeObject(objTemp, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            this.PUTJson(strJson, strUrl);
            this.CacheData();
        }

        /// <summary>
        /// Creates a new Contact in memory and in Constant Contact
        /// </summary>
        /// <param name="objContact">Contact to send up to Constant Contact</param>
        public void Create(Contact objContact)
        {
            if (!this.SignedIn)
                return;
            POSTContact objTempContact = objContact.Create();
            string strJson = JsonConvert.SerializeObject(objTempContact, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                
            });
            this.PostJson(strJson, "contacts");
            this.UpdateContacts();
            this.CacheData();
        }

        /// <summary>
        /// Creates a list in memory and sends it up to Constant Contact-
        /// </summary>
        /// <param name="objContactList"></param>
        public void Create(ContactList objContactList)
        {
           
            POSTContactList objTempContactList = objContactList.Create();
            string strJson = JsonConvert.SerializeObject(objTempContactList, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

            });
            this.PostJson(strJson, "contact_lists");
            this.UpdateContactLists();
            this.CacheData();
        }

        /// <summary>
        /// Deletes a list in the constant Contact API
        /// </summary>
        /// <param name="objList"></param>
        public void RemoveContactList(ContactList objList)
        {
            this.gobjCCAuth.ValidateAuthentication();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.mstrTokenHeader);

            var response = client.DeleteAsync(this.gstrBaseURL + $"contact_lists/{objList.list_id}").Result;
            this.gdctContactLists.Remove(objList.list_id);
            this.CacheData();

        }

        /// <summary>
        /// Assigns a Contact to a list in miemory and the API
        /// </summary>
        /// <param name="objContactList">ContactList to update</param>
        /// <param name="objContact">Contact to assign</param>
        public void AddContactToContactList(ContactList objContactList, Contact objContact)
        {
            if (objContact.list_memberships.Count() >= 50)
            {
                Console.WriteLine("User is at max list count");
            }
            else
            {
                this.gobjCCAuth.ValidateAuthentication();

                JArray objContactIds = new JArray(objContact.contact_id);
                JProperty objContactProp = new JProperty("contact_ids", objContactIds);

                JObject objSource = new JObject(objContactProp);

                JProperty objSourceProp = new JProperty("source", objSource);



                JArray LstListIDs = new JArray(objContactList.list_id);
                JProperty objListProp = new JProperty("list_ids", LstListIDs);


                JObject objFinal = new JObject();
                objFinal.Add(objSourceProp);
                objFinal.Add(objListProp);



                string strFinalJson = JsonConvert.SerializeObject(objFinal);


                this.PostJson(strFinalJson, "activities/add_list_memberships");
                objContact.list_memberships.Add(objContactList.list_id);
                this.ContactListAssignment();
                this.CacheData();
            }
        }

        /// <summary>
        /// Revoves a contact from a list
        /// </summary>
        /// <param name="objContactList">List to remove from</param>
        /// <param name="objContact">Contact to remove</param>
        public void RemoveContactFromContactList(ContactList objContactList, Contact objContact)
        {
            this.gobjCCAuth.ValidateAuthentication();

            JArray objContactIds = new JArray(objContact.contact_id);
            JProperty objContactProp = new JProperty("contact_ids", objContactIds);

            JObject objSource = new JObject(objContactProp);

            JProperty objSourceProp = new JProperty("source", objSource);


            JArray LstListIDs = new JArray(objContactList.list_id);
            JProperty objListProp = new JProperty("list_ids", LstListIDs);

            JObject objFinal = new JObject();
            objFinal.Add(objSourceProp);
            objFinal.Add(objListProp);


            string strFinalJson = JsonConvert.SerializeObject(objFinal);

            this.PostJson(strFinalJson, "activities/remove_list_memberships");

            objContactList.glstMembers.RemoveAll(x => x.contact_id == objContact.contact_id);
            objContact.glstContactLists.RemoveAll(x => x.list_id == objContactList.list_id);

            this.CacheData();
        }

        /// <summary>
        /// Adds a list to an activity in memory and the api
        /// </summary>
        /// <param name="objList">List to add</param>
        /// <param name="objActivity">Activity to have it added to</param>
        public void AddListToActivity(ContactList objList, EmailCampaignActivity objActivity)
        {
            objActivity.contact_list_ids.Add(objList.list_id);
            PUTEmailCampaignActivity objTemp = objActivity.Update();

            string strJson = JsonConvert.SerializeObject(objTemp, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

            });

            this.PUTJson(strJson, $"/emails/activities/{objActivity.campaign_activity_id}");

        }

        /// <summary>
        /// Removes a list from an activity in memory and API
        /// </summary>
        /// <param name="objList">List to remove</param>
        /// <param name="objActivity">Activity to remove from</param>
        public void RemoveListFromActivity(ContactList objList, EmailCampaignActivity objActivity)
        {
            objActivity.contact_list_ids.Remove(objList.list_id);
            PUTEmailCampaignActivity objTemp = objActivity.Update();

            string strJson = JsonConvert.SerializeObject(objTemp, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

            });

            this.PUTJson(strJson, $"/emails/activities/{objActivity.campaign_activity_id}");

        }

        /// <summary>
        /// Sends out the selected activity
        /// </summary>
        /// <param name="objActivity">Activity to send</param>
        public void SendActivity(EmailCampaignActivity objActivity)
        {
            if (objActivity.contact_list_ids.Count()>0)
            {
                string strUrl = $"emails/activities/{objActivity.campaign_activity_id}/schedules";
                //string strdate = objTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);

                string strData = $"{{\"scheduled_date\": \"0\"}}";

                this.PostJson(strData, strUrl);
                Console.WriteLine("send activity");
            }
            else
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// delete constant contact log in token file
        /// </summary>
        public void LogOut()
        {
            this.gobjCCAuth.LogOut();
            this.RemoveLocalCache();
            gdctContacts.Clear();
            gdctContactLists.Clear();
    }

        private void RemoveLocalCache()
        {
            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCSaveData.JSON";
            if (File.Exists(strFname))
            {
                File.Delete(strFname);
            }
        }

        /// <summary>
        /// Saves all the data in memory to a bin file for faster load on program start up
        /// </summary>
        private void CacheData()
        {
            try
            {
                string strContacts = JsonConvert.SerializeObject(this.gdctContacts, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strContactLists = JsonConvert.SerializeObject(this.gdctContactLists, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strCustomFields = JsonConvert.SerializeObject(this.gdctCustomFields, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strCampaigns = JsonConvert.SerializeObject(this.gdctEmailCampaigns, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strActivities = JsonConvert.SerializeObject(this.gdctEmailCampaignActivities, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strActivityPreviews = JsonConvert.SerializeObject(this.glstEmailCampaignActivityPreviews, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCSaveData.JSON";

                Dictionary<string, string> dctData = new Dictionary<string, string>()
                    {
                        { "contacts",strContacts },
                        { "contactlists",strContactLists },
                        { "customfields",strCustomFields },
                        { "campaigns",strCampaigns },
                        { "activities",strActivities },
                        { "activitypreviews",strActivityPreviews }
                    };

                string strData = JsonConvert.SerializeObject(dctData);

                strData = this.Obfuscate(strData);

                File.WriteAllText(strFname, strData, Encoding.UTF8);
            }catch (Exception)
            {
                //if we cannot cache due to null reference just refurn and try again on next update
                return;
            }
            
        }

        /// <summary>
        /// Loads the cached data for faster load times and proform the local assignments
        /// </summary>
        private void LoadData()
        {
            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCSaveData.JSON";
            try
            {

                string strData = File.ReadAllText(strFname, Encoding.UTF8);

                strData = this.Deobfuscate(strData);

                Dictionary<string, string> dctData = JsonConvert.DeserializeObject<Dictionary<string, string>>(strData);

                this.gdctContacts = JsonConvert.DeserializeObject<Dictionary<string, Contact>>(dctData["contacts"]);
                this.gdctContactLists = JsonConvert.DeserializeObject<Dictionary<string, ContactList>>(dctData["contactlists"]);
                this.gdctCustomFields = JsonConvert.DeserializeObject<Dictionary<string, CustomField>>(dctData["customfields"]);
                this.gdctEmailCampaigns = JsonConvert.DeserializeObject<Dictionary<string, EmailCampaign>>(dctData["campaigns"]);
                this.gdctEmailCampaignActivities = JsonConvert.DeserializeObject<Dictionary<string, EmailCampaignActivity>>(dctData["activities"]);
                this.glstEmailCampaignActivityPreviews = JsonConvert.DeserializeObject<List<EmailCampaignActivityPreview>>(dctData["activitypreviews"]);

                this.LocalActivityAssignments();
                this.LocalPreviewAssignment();
                this.ContactListAssignment();
            }
            catch (Exception)
            {
                //if json is corupted or malformed, erase the file and load from Constant Contact API
                File.WriteAllText(strFname, "");
            }
            
        }

        /// <summary>
        /// Obfuscates a string to provide some level of privacy of data
        /// </summary>
        /// <param name="strIn">string to obfuscate</param>
        /// <returns></returns>
        private string Obfuscate(string strIn)
        {

            string strWorking = strIn;

            for (int i = 0; i < 7; i++)
            {
                char[] charArray = strWorking.ToCharArray();
                Array.Reverse(charArray);
                strWorking = new string(charArray);

                UTF8Encoding objUTF8 = new UTF8Encoding();
                byte[] bytValueArray = objUTF8.GetBytes(strWorking);
                strWorking = Convert.ToBase64String(bytValueArray);

            }
            
            return strWorking;
        }

        /// <summary>
        /// undoes the method this.Obfuscate() to have readable json for prosessing
        /// </summary>
        /// <param name="strIn">string to deobfuscate</param>
        /// <returns></returns>
        private string Deobfuscate(string strIn)
        {
            string strWorking = strIn;

            for (int i = 0; i < 7; i++)
            {

                byte[] lstBytes = Convert.FromBase64String(strWorking);
                strWorking =  Encoding.UTF8.GetString(lstBytes);

                char[] charArray = strWorking.ToCharArray();
                Array.Reverse(charArray);
                strWorking = new string(charArray);

            }

            return strWorking;

        }

    }
}
