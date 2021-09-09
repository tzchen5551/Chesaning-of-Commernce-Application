using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DirectorPortalDatabase.Models;

namespace DirectorPortalDatabase.Utility
{
    /// <summary>
    /// Utility class that allows the program to retrieve metadata about a table using only the table's model name.
    /// Also allows the program to grab a DbSet from the context when the type is not known at compile time.
    /// </summary>
    public class MetadataHelper
    {
        /// <summary>
        /// Stores metadata on a single table field and allows for retrieval of a record's field value.
        /// </summary>
        public class TableField : FieldHelper.IDataField
        {
            /// <summary>
            /// The PropertyInfo object used to instantiate the TableField.
            /// </summary>
            private PropertyInfo UnderlyingProperty { get; set; }
            public Type TypeFieldType { get; private set; }
            public string StrPropertyName { get; private set; }
            public string StrHumanReadableName { get; private set; }
            public TableField(PropertyInfo property)
            {
                UnderlyingProperty = property;
                TypeFieldType = property.PropertyType;
                StrPropertyName = property.Name;
                StrHumanReadableName = GetHumanReadableFieldName(property.Name);
            }
            /// <summary>
            /// Gets the value of the field from a specific record instance.
            /// </summary>
            /// <param name="recordInstance"></param>
            /// <remarks>
            /// You must pass in an instance of the class which actually has the underlying property.
            /// </remarks>
            public object GetValue(object objRecordInstance)
            {
                if (objRecordInstance == null)
                    return null;
                return UnderlyingProperty.GetValue(objRecordInstance, null);
            }
        }

        /// <summary>
        /// Holds a collection of TableField objects to describe a database table's metadata.
        /// </summary>
        public class DatabaseTable
        {
            /// <summary>
            /// The Type used to instantiate the DatabaseTable.
            /// </summary>
            private Type TypeUnderlyingType { get; set; }
            private FieldHelper.IDataField[] RGFields { get; set; }
            public int IntNumberOfFields => RGFields.Length;
            
            /// <summary>
            /// Gets an object that provides metadata on one of this table's columns.
            /// </summary>
            /// <param name="intIndex"></param>
            /// <returns></returns>
            public FieldHelper.IDataField GetField(int intIndex)
            {
                return RGFields[intIndex];
            }

            public DatabaseTable(Type typeUnderlyingType)
            {
                TypeUnderlyingType = typeUnderlyingType;

                List<FieldHelper.IDataField> rgFields = new List<FieldHelper.IDataField>();

                // Iterates over the properties of the underlying class type.
                PropertyInfo[] rgProperties = typeUnderlyingType.GetProperties();
                foreach (PropertyInfo recordProperty in rgProperties)
                {
                    // Checks if this is a property type that we care about.
                    if (recordProperty.PropertyType.IsPrimitive
                        || recordProperty.PropertyType == typeof(string)
                        || recordProperty.PropertyType.IsEnum
                        || recordProperty.PropertyType == typeof(DateTime)
                        || recordProperty.PropertyType == typeof(decimal))
                    {
                        // Checks if this property is listed as one not to display.
                        if (!(GDictPropertiesToAvoidByModelType.ContainsKey(TypeUnderlyingType) 
                            && GDictPropertiesToAvoidByModelType[TypeUnderlyingType].Contains(recordProperty.Name)))
                        {
                            rgFields.Add(new TableField(recordProperty));
                        }
                    }
                }

                string strModelName = TypeUnderlyingType.Name;
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    List<FieldHelper.IDataField> rgExtraFields = dbContext.AdditionalFields
                        // Queries for all additional fields pointing to the underlying model.
                        .Where(udtField => udtField.TableName == strModelName)
                        // Stores the field names in objects that implement the FieldHelper.IDataField interface.
                        .Select(udtField => (FieldHelper.IDataField)new FieldHelper.CustomField(udtField.FieldName)).ToList();

                    rgFields.AddRange(rgExtraFields);
                }

                RGFields = rgFields.ToArray();
            }
        }

        ///<summary>
        /// Used to get the human-readable version of a variable name.
        /// </summary>
        private static Dictionary<string, string> GDictHumanReadableNames = new Dictionary<string, string>
        {
            // Table class names
            ["Categories"] = "Category",
            ["BusinessRep"] = "Business Representative",
            ["Todo"] = "To-Do List Items",
            // Address class properties
            ["ZipCodeExt"] = "Extended Zip Code",
            // PhoneNumber class properties
            ["GEnumPhoneType"] = "Phone Type"
        };

        /// <summary>
        /// Used to get an array of the names of properties to avoid displaying to the user.
        /// The key is the model type, and the value is the array of names.
        /// </summary>
        private static Dictionary<Type, string[]> GDictPropertiesToAvoidByModelType = new Dictionary<Type, string[]>
        {
            [typeof(Address)] = new string[] { "Id" },
            [typeof(Business)] = new string[] { "Id", "MailingAddressId", "PhysicalAddressId", "ExtraFields" },
            [typeof(Categories)] = new string[] { "Id" },
            [typeof(ContactPerson)] = new string[] { "Id" },
            [typeof(Email)] = new string[] { "Id", "ContactPersonId" },
            [typeof(EmailGroup)] = new string[] { "Id" },
            [typeof(Payment)] = new string[] { "Id" },
            [typeof(PaymentItem)] = new string[] { "Id" },
            [typeof(PhoneNumber)] = new string[] { "Id", "ContactPersonId" },
            [typeof(YearlyData)] = new string[] { "Id", "BusinessId", "ExtraFields" }
        };

        /// <summary>
        /// Converts the name of a class or property into a human-readable form with spacing.
        /// </summary>
        /// <param name="strClassOrPropertyName"></param>
        /// <remarks>
        /// Cannot yet parse names of unknown variables. Instead, for those, it returns what was passed in.
        /// </remarks>
        public static string GetHumanReadableFieldName(string strClassOrPropertyName)
        {
            // If the variable name has been parsed before, then its name can be found in the global dictionary.
            if (GDictHumanReadableNames.ContainsKey(strClassOrPropertyName))
            {
                return GDictHumanReadableNames[strClassOrPropertyName];
            }
            else
            {
                // Adds a space before every capital letter and then removes the leading space if there is one.
                return System.Text.RegularExpressions.Regex.Replace(strClassOrPropertyName, "[A-Z]", " $0").TrimStart();
            }
        }

        public class ModelInfo
        {
            public Type TypeModelType { get; private set; }
            public DatabaseTable UdtTableMetaData { get; private set; }
            public string StrHumanReadableName { get; private set; }
            public ModelInfo(Type typeModelType)
            {
                TypeModelType = typeModelType;
                UdtTableMetaData = new DatabaseTable(typeModelType);
                StrHumanReadableName = GetHumanReadableFieldName(typeModelType.Name);
            }
            /// <summary>
            /// Gets the DbSet from the context for this ModelInfo instance's model type. The return type is technically IQueryable of object.
            /// </summary>
            public IQueryable<object> GetContextDbSet(DatabaseContext dbContext)
            {
                // Calls the generic method dbContext.Set, which returns the DbSet for a generic type.
                // The call is modified to account for the fact that TypeModelType is not generic.
                IQueryable qryAllRecords = (IQueryable)dbContext.GetType().GetMethod("Set").MakeGenericMethod(TypeModelType).Invoke(dbContext, null);
                // Converts the IQueryable to IQueryable<object>.
                return qryAllRecords.Cast<object>();
            }

            /// <summary>
            /// For ModelInfo instances, returns whether the model types are the same. For all other types, returns false.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object objOther)
            {
                Type typeOtherType = objOther.GetType();
                if (typeOtherType == typeof(ModelInfo))
                {
                    ModelInfo udtOther = (ModelInfo)objOther;
                    return this.TypeModelType == udtOther.TypeModelType;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Maps model types to ModelInfo instances that describe them.
        /// </summary>
        private static Dictionary<Type, ModelInfo> GDictModels = new Dictionary<Type, ModelInfo>
        {
            [typeof(Address)] = new ModelInfo(typeof(Address)),
            [typeof(Business)] = new ModelInfo(typeof(Business)),
            [typeof(BusinessRep)] = new ModelInfo(typeof(BusinessRep)),
            [typeof(Categories)] = new ModelInfo(typeof(Categories)),
            [typeof(ContactPerson)] = new ModelInfo(typeof(ContactPerson)),
            [typeof(Email)] = new ModelInfo(typeof(Email)),
            [typeof(EmailGroup)] = new ModelInfo(typeof(EmailGroup)),
            [typeof(Payment)] = new ModelInfo(typeof(Payment)),
            [typeof(PaymentItem)] = new ModelInfo(typeof(PaymentItem)),
            [typeof(PhoneNumber)] = new ModelInfo(typeof(PhoneNumber)),
            [typeof(YearlyData)] = new ModelInfo(typeof(YearlyData))
        };

        /// <summary>
        /// Updates the GDictModels dictionary entries for models with extra fields.
        /// This allows the report generator to respond to new fields without restarting the program.
        /// </summary>
        public static void RefreshModelInfo()
        {
            // Gets only the model types with extra fields.
            Type[] rgModelsWithExtraFields = GDictModels.Keys
                .Where(typeModelType => typeModelType.IsSubclassOf(typeof(HasExtraFields))).ToArray();

            foreach (Type typeModelType in rgModelsWithExtraFields)
            {
                if (typeModelType.IsSubclassOf(typeof(HasExtraFields)))
                {
                    GDictModels[typeModelType] = new ModelInfo(typeModelType);
                }
            }
        }

        private static Type[] GRGModels =
        {
            typeof(Address),
            typeof(Business),
            //typeof(BusinessRep),
            typeof(Categories),
            typeof(ContactPerson),
            typeof(Email),
            typeof(EmailGroup),
            typeof(Payment),
            typeof(PaymentItem),
            typeof(PhoneNumber),
            typeof(YearlyData)
        };

        /// <summary>
        /// Maps the names of models to their types.
        /// </summary>
        private static Dictionary<string, Type> GDictTypeByName = new Dictionary<string, Type>
        {
            [typeof(Address).Name] = typeof(Address),
            [typeof(Business).Name] = typeof(Business),
            [typeof(BusinessRep).Name] = typeof(BusinessRep),
            [typeof(Categories).Name] = typeof(Categories),
            [typeof(ContactPerson).Name] = typeof(ContactPerson),
            [typeof(Email).Name] = typeof(Email),
            [typeof(EmailGroup).Name] = typeof(EmailGroup),
            [typeof(Payment).Name] = typeof(Payment),
            [typeof(PaymentItem).Name] = typeof(PaymentItem),
            [typeof(PhoneNumber).Name] = typeof(PhoneNumber),
            [typeof(YearlyData).Name] = typeof(YearlyData)
        };

        /// <summary>
        /// Maps model types to their corresponding enums.
        /// </summary>
        private static Dictionary<Type, JoinHelper.EnumTable> GDictEnumTableByType = new Dictionary<Type, JoinHelper.EnumTable>
        {
            [typeof(Address)] = JoinHelper.EnumTable.Address,
            [typeof(Business)] = JoinHelper.EnumTable.Business,
            [typeof(BusinessRep)] = JoinHelper.EnumTable.BusinessRep,
            [typeof(Categories)] = JoinHelper.EnumTable.Categories,
            [typeof(ContactPerson)] = JoinHelper.EnumTable.ContactPerson,
            [typeof(Email)] = JoinHelper.EnumTable.Email,
            [typeof(EmailGroup)] = JoinHelper.EnumTable.EmailGroup,
            [typeof(Payment)] = JoinHelper.EnumTable.Payment,
            [typeof(PaymentItem)] = JoinHelper.EnumTable.PaymentItem,
            [typeof(PhoneNumber)] = JoinHelper.EnumTable.PhoneNumber,
            [typeof(YearlyData)] = JoinHelper.EnumTable.YearlyData
        };

        public static int IntNumberOfModels => GRGModels.Length;
        /// <summary>
        /// Used to access the array of model types.
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static Type GetModelTypeByIndex(int intIndex)
        {
            return GRGModels[intIndex];
        }

        /// <summary>
        /// Returns an object that provides metadata pertaining to the model type that is passed in.
        /// </summary>
        /// <param name="typeModelType"></param>
        /// <returns></returns>
        public static ModelInfo GetModelInfo(Type typeModelType)
        {
            if (GDictModels.ContainsKey(typeModelType))
            {
                return GDictModels[typeModelType];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a model type into an enum value that is used by the JoinHelper utility class.
        /// </summary>
        /// <param name="typeModelType"></param>
        /// <returns></returns>
        public static JoinHelper.EnumTable GetEnumTable(Type typeModelType)
        {
            if (GDictEnumTableByType.ContainsKey(typeModelType))
            {
                return GDictEnumTableByType[typeModelType];
            }
            else
            {
                throw new ArgumentException("This Type does not have an EnumTable.");
            }
        }

        /// <summary>
        /// Returns an object that provides metadata pertaining to the model type whose name matches the string passed in.
        /// </summary>
        /// <param name="strModelName"></param>
        /// <returns></returns>
        public static ModelInfo GetModelInfoByName(string strModelName)
        {
            if (GDictTypeByName.ContainsKey(strModelName))
            {
                Type typeModelType = GDictTypeByName[strModelName];
                return GetModelInfo(typeModelType);
            }
            else
            {
                return null;
            }
        }
    }
}
