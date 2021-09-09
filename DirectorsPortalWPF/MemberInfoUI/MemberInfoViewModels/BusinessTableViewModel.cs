using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalWPF.MemberInfoUI
{
    /// <summary>
    /// A view model that defines the members table.
    /// </summary>
    class BusinessTableViewModel
    {
        /* TODO: This data model will need to change as we develope a solution for
         * dynamic fields. */
        public string StrBuisnessName { get; set; }
        public string StrMailingAddress { get; set; }
        public string StrLocationAddress { get; set; }
        public string StrCity { get; set; }
        public string StrState { get; set; }
        public int? IntZipCode { get; set; }
        public string StrContactPerson { get; set; }
        public string StrPhoneNumber { get; set; }
        public string StrFaxNumber { get; set; }
        public string StrEmailAddress { get; set; }
        public string StrWebsite { get; set; }
        public string StrLevel { get; set; }
        public int? IntEstablishedYear { get; set; }

        /// <summary>
        /// A method for popualting the human readable names into the
        /// global dictionary. These fields will be used to relate the names of
        /// the properties to the their human readable column headers.
        /// </summary>
        public static Dictionary<string, string> PopulateHumanReadableTableDic()
        {
            /* TODO: This method will need to change as we develope a solution for dynamic
             * fields. */
            Dictionary<string, string> dicHumanReadableTableFields = new Dictionary<string, string>();

            dicHumanReadableTableFields.Add("StrBuisnessName", "Business Name");
            dicHumanReadableTableFields.Add("StrMailingAddress", "Mailing Address");
            dicHumanReadableTableFields.Add("StrLocationAddress", "Location Address");
            dicHumanReadableTableFields.Add("StrCity", "City");
            dicHumanReadableTableFields.Add("StrState", "State");
            dicHumanReadableTableFields.Add("IntZipCode", "Zip Code");
            dicHumanReadableTableFields.Add("StrContactPerson", "Contact");
            dicHumanReadableTableFields.Add("StrPhoneNumber", "Phone Number");
            dicHumanReadableTableFields.Add("StrFaxNumber", "Fax Number");
            dicHumanReadableTableFields.Add("StrEmailAddress", "Email Address");
            dicHumanReadableTableFields.Add("StrWebsite", "Website");
            dicHumanReadableTableFields.Add("StrLevel", "Level");
            dicHumanReadableTableFields.Add("IntEstablishedYear", "Established");

            return dicHumanReadableTableFields;
        }
    }
}
