using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorsPortalConstantContact
{
    /// <summary>
    /// the main reference point for the Custom feilds. This will have a few helper properties and meathods, but will 
    /// mainly be here for holding the data that has been retrieved from constant contact 
    /// </summary>
    public class CustomField : GETCustomField
    {
        public List<Contact> glstContacts = new List<Contact>();
    }
}
