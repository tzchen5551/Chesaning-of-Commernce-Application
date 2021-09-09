using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorsPortalConstantContact
{
    public class PUTEmailCampaignActivity
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
}
