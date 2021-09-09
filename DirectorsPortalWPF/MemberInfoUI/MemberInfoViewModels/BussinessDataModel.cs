using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalWPF.MemberInfoUI.MemberInfoViewModels
{
    /// <summary>
    /// A view model that defines the view for adding and editing a
    /// new business.
    /// </summary>
    class BusinessDataModel
    {
        public string StrBuisnessName { get; set; }
        public string StrWebsite { get; set; }
        public string StrLevel { get; set; }
        public int? IntEstablishedYear { get; set; }
        public string StrMailingAddress { get; set; }
        public string StrMailCity { get; set; }
        public string StrMailState { get; set; }
        public int? IntMailZipCode { get; set; }
        public string StrLocationAddress { get; set; }
        public string StrLocCity { get; set; }
        public string StrLocState { get; set; }
        public int? IntLocZipCode { get; set; }
        public string StrContactPerson { get; set; }
        public string StrPhoneNumber { get; set; }
        public string StrFaxNumber { get; set; }
        public string StrEmailAddress { get; set; }

        public static Dictionary<string, string> PopulateHumanReadableDataDic()
        {
            Dictionary<string, string> dicHumanReadableData = new Dictionary<string, string>();

            dicHumanReadableData.Add("StrBuisnessName", "Business Name");
            dicHumanReadableData.Add("StrWebsite", "Website");
            dicHumanReadableData.Add("StrLevel", "Level");
            dicHumanReadableData.Add("IntEstablishedYear", "Established");
            dicHumanReadableData.Add("StrMailingAddress", "Mailing Address");
            dicHumanReadableData.Add("StrMailCity", "City");
            dicHumanReadableData.Add("StrMailState", "State");
            dicHumanReadableData.Add("IntMailZipCode", "Zip Code");
            dicHumanReadableData.Add("StrLocationAddress", "Location Address");
            dicHumanReadableData.Add("StrLocCity", "Location City");
            dicHumanReadableData.Add("StrLocState", "Location State");
            dicHumanReadableData.Add("IntLocZipCode", "Location Zip Code");
            dicHumanReadableData.Add("StrContactPerson", "Contact Name");
            dicHumanReadableData.Add("StrPhoneNumber", "Phone Number");
            dicHumanReadableData.Add("StrFaxNumber", "Fax Number");
            dicHumanReadableData.Add("StrEmailAddress", "Email Address");

            return dicHumanReadableData;
        }
    }
}
