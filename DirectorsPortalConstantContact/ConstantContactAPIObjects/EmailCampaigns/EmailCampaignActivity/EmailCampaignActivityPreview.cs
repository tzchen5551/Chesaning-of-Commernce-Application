using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorsPortalConstantContact
{
    public class EmailCampaignActivityPreview
    {

        public EmailCampaignActivity activity;

        public string strCampaignName => activity.strCampaignName;
        public string campaign_activity_id { get; set; }
        public string from_email { get; set; }
        public string from_name { get; set; }
        public string preheader { get; set; }
        public string preview_html_content { get; set; }
        public string preview_text_content { get; set; }
        public string reply_to_email { get; set; }
        public string subject { get; set; }
    }
}
