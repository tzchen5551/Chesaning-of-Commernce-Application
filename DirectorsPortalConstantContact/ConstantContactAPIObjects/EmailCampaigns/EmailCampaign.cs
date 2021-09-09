using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorsPortalConstantContact
{
    public class EmailCampaign : GETEmailCampaign
    {
        [Newtonsoft.Json.JsonIgnore]
        public List<EmailCampaignActivity> Activities = new List<EmailCampaignActivity>();


        public POSTEmailCampaign objNewCampaign()
        {
            POSTEmailCampaign objTempCampaign = new POSTEmailCampaign()
            {
                name = this.name
            };
            objTempCampaign.email_campaign_activities = new List<POSTEmailCampaignActivity>();

            foreach (EmailCampaignActivity objCampaignActivity in this.Activities)
            {
                objTempCampaign.email_campaign_activities.Add(objCampaignActivity.NewActivity());
            }

            return objTempCampaign;
        }


        public EmailCampaignActivity AddActivity(string strFromEmail, string strFromName, string strReplayToEmail, string strSubject, string strHTMLContent)
        {
            EmailCampaignActivity objNewActivity = new EmailCampaignActivity()
            {
                format_type = 5,
                from_email = strFromEmail,
                from_name = strFromName,
                reply_to_email = strReplayToEmail,
                subject = strSubject,
                html_content = strHTMLContent
            };

            this.Activities.Add(objNewActivity);

            return objNewActivity;
        }

        

    }
}
