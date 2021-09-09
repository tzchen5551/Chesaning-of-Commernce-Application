using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Utility
{
    public class FieldHelper
    {
        /// <summary>
        /// An interface that defines methods for a class representing a data field that is either a database
        /// column or a JSON object property.
        /// </summary>
        public interface IDataField
        {
            /// <summary>
            /// The type of data stored in the field.
            /// </summary>
            Type TypeFieldType { get; }
            /// <summary>
            /// The name of the field as it is called internally.
            /// </summary>
            string StrPropertyName { get; }
            /// <summary>
            /// The name of the field shown to the user.
            /// </summary>
            string StrHumanReadableName { get; }
            /// <summary>
            /// Used to extract a field's value from a particular record.
            /// </summary>
            /// <param name="objRecordInstance"></param>
            /// <returns></returns>
            object GetValue(object objRecordInstance);
        }

        /// <summary>
        /// Used to access data from a custom field whose value is stored within a JSON object.
        /// </summary>
        public class CustomField : IDataField
        {
            /// <summary>
            /// Custom field values are always stored as strings.
            /// </summary>
            public Type TypeFieldType => typeof(string);
            public string StrPropertyName { get; private set; }
            public string StrHumanReadableName => StrPropertyName;

            public object GetValue(object objRecordInstance)
            {
                // Tests if the object has extra fields.
                if (typeof(HasExtraFields).IsInstanceOfType(objRecordInstance))
                {
                    // Gets the value from the JSON object.
                    HasExtraFields udtRecordWithExtraFields = (HasExtraFields)objRecordInstance;
                    return udtRecordWithExtraFields.GetField(StrPropertyName);
                }
                else
                {
                    throw new ArgumentException("Object does not support extra fields.");
                }
            }

            public CustomField(string strPropertyName)
            {
                StrPropertyName = strPropertyName;
            }
        }
    }
}
