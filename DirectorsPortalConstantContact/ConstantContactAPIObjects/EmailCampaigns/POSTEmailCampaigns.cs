using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    public class POSTEmailCampaign
    {
        public string name { get; set; }
        public List<POSTEmailCampaignActivity> email_campaign_activities { get; set; }
    }

    public class POSTEmailCampaignActivity
    {
        public int format_type { get; set; }
        public string from_name { get; set; }
        public string from_email { get; set; }
        public string reply_to_email { get; set; }
        public string subject { get; set; }
        public string preheader { get; set; }
        public string html_content { get; set; }
        public PhysicalAddressInFooter physical_address_in_footer { get; set; }
        public DocumentProperties document_properties { get; set; }
    }
}
