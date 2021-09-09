using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    public class PUTEmailCampaign
    {
        public string campaign_activity_id { get; set; }
        public string campaign_id { get; set; }
        public string role { get; set; }
        public List<string> contact_list_ids { get; set; }
        public List<int> segment_ids { get; set; }
        public string current_status { get; set; }
        public int format_type { get; set; }
        public string from_email { get; set; }
        public string from_name { get; set; }
        public string reply_to_email { get; set; }
        public string subject { get; set; }
        public string html_content { get; set; }
        public string template_id { get; set; }
        public string permalink_url { get; set; }
        public string preheader { get; set; }
        public PhysicalAddressInFooter physical_address_in_footer { get; set; }
        public DocumentProperties document_properties { get; set; }
    }

    public class PhysicalAddressInFooter
    {
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_optional { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string organization_name { get; set; }
        public string postal_code { get; set; }
        public string state_code { get; set; }
        public string state_name { get; set; }
        public string state_non_us_name { get; set; }
    }

    public class DocumentProperties
    {
        public string style_content { get; set; }
        public string letter_format { get; set; }
        public string greeting_salutation { get; set; }
        public string greeting_name_type { get; set; }
        public string greeting_secondary { get; set; }
        public string subscribe_link_enabled { get; set; }
        public string subscribe_link_name { get; set; }
        public string text_content { get; set; }
        public string permission_reminder_enabled { get; set; }
        public string permission_reminder { get; set; }
        public string view_as_webpage_enabled { get; set; }
        public string view_as_webpage_text { get; set; }
        public string view_as_webpage_link_name { get; set; }
        public string forward_email_link_enabled { get; set; }
        public string forward_email_link_name { get; set; }
    }


}
