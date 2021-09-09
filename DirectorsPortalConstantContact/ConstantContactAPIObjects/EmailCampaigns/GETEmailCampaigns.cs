using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    public class ActivityList
    {
        public string campaign_activity_id { get; set; }
        public string role { get; set; }

    }
    public class GETEmailCampaign
    {
        public List<ActivityList> campaign_activities { get; set; }
        public string campaign_id { get; set; }
        public DateTime created_at { get; set; }
        public string current_status { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int type_code { get; set; }
        public DateTime updated_at { get; set; }
    }
}
