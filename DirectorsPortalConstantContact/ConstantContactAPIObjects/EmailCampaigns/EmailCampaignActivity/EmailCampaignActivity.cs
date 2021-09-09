using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorsPortalConstantContact
{
    public class EmailCampaignActivity : GETEmailCampaignActivity
    {
        [Newtonsoft.Json.JsonIgnore]
        public EmailCampaignActivityPreview mobjPreview;
        [Newtonsoft.Json.JsonIgnore]
        public EmailCampaign gobjCampaign;
        [Newtonsoft.Json.JsonIgnore]
        public string strCampaignName => gobjCampaign.name;

        [Newtonsoft.Json.JsonIgnore]
        public List<ContactList> glstContactLists = new List<ContactList>();



        /// <summary>
        /// translate to POST for use with the API
        /// </summary>
        /// <returns></returns>
        public POSTEmailCampaignActivity NewActivity()
        {
            POSTEmailCampaignActivity objTempActivity = new POSTEmailCampaignActivity()
            {
                format_type = this.format_type,
                from_name = this.from_name,
                from_email = this.from_email,
                reply_to_email = this.reply_to_email,
                subject = this.subject,
                preheader = this.preheader,
                html_content = this.html_content,
                physical_address_in_footer = this.physical_address_in_footer,
                document_properties = this.document_properties
            };

            return objTempActivity;
        }

        /// <summary>
        /// translate to PUT for use with the API
        /// </summary>
        /// <returns></returns>
        public PUTEmailCampaignActivity Update()
        {
            return new PUTEmailCampaignActivity()
            {
                campaign_activity_id = this.campaign_activity_id,
                campaign_id = this.campaign_id,
                role = this.role,
                contact_list_ids = this.contact_list_ids,
                segment_ids = this.segment_ids,
                current_status = this.current_status,
                format_type = this.format_type,
                from_email = this.from_email,
                from_name = this.from_name,
                reply_to_email = this.reply_to_email,
                subject = this.subject,
                html_content = this.html_content,
                template_id = this.template_id,
                permalink_url = this.permalink_url,
                preheader = this.preheader,
                physical_address_in_footer = this.physical_address_in_footer,
                document_properties = this.document_properties

            };
        }

    }
}
