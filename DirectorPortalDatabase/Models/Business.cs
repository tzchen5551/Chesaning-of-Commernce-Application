using DirectorPortalDatabase.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DirectorPortalDatabase.Models
{
    public class Business : HasExtraFields
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the business
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// The year that the business was established
        /// </summary>
        public int? YearEstablished { get; set; }

        /// <summary>
        /// The membership level of the business
        /// </summary>
        public MembershipLevel MembershipLevel { get; set; }

        /// <summary>
        /// The id of mailing address of the business
        /// </summary>
        public int? MailingAddressId { get; set; }
        
        public virtual Address MailingAddress { get; set; }

        /// <summary>
        /// The id of physical address of the business
        /// </summary>
        public int? PhysicalAddressId { get; set; }

        public virtual Address PhysicalAddress { get; set; }

        /// <summary>
        /// The business's website address
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Any additional notes about a business
        /// </summary>
        public string ExtraNotes { get; set; }

        /// <summary>
        /// A list of the business reps for the business.
        /// </summary>
        public virtual List<BusinessRep> BusinessReps { get; set; }

        /// <summary>
        /// Represents an array of buisness type categories
        /// </summary>
/*        public virtual List<Categories> Categories 
        {
            get
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    return dbContext.CategoryRef.Where(x => x.BusinessId == Id).Select(b => b.Category).ToList();
                }
            }
        }*/

        public virtual List<CategoryRef> CategoryRefs { get; set; }
        /// <summary>
        /// Represents an array of the yearly data objects
        /// </summary>
        public virtual List<YearlyData> YearlyData { get; set; }

        /// <summary>
        /// A method for converting the membership level from the business entity to
        /// a human readable string.
        /// </summary>
        /// <param name="strEnumVal">The membership level from the data model</param>
        /// <returns>The membership enum value of the entered string.</returns>
        public static MembershipLevel GetMembershipLevelEnum(string strEnumVal)
        {
            MembershipLevel enumMembershipLevel;
            strEnumVal = strEnumVal.ToLower();

            switch (strEnumVal)
            {
                case "none":
                    enumMembershipLevel = MembershipLevel.NONE;
                    break;

                case "gold":
                    enumMembershipLevel = MembershipLevel.GOLD;
                    break;

                case "silver":
                    enumMembershipLevel = MembershipLevel.SILVER;
                    break;

                case "associate":
                    enumMembershipLevel = MembershipLevel.ASSOCIATE;
                    break;

                case "individual":
                    enumMembershipLevel = MembershipLevel.INDIVIDUAL;
                    break;

                case "courtesy":
                    enumMembershipLevel = MembershipLevel.COURTESY;
                    break;

                default:
                    enumMembershipLevel = MembershipLevel.NONE;
                    break;
            }

            return enumMembershipLevel;
        }

        /// <summary>
        /// A method for converting the membership level from the business entity to
        /// a human readable string.
        /// </summary>
        /// <param name="membershipLevel">The membership level from the business entity.</param>
        /// <returns>The human readable membership string.</returns>
        public static string GetMebershipLevelString(MembershipLevel membershipLevel)
        {
            string strLevel = "";

            switch (membershipLevel)
            {
                case MembershipLevel.NONE:
                    strLevel = "None";
                    break;

                case MembershipLevel.GOLD:
                    strLevel = "Gold";
                    break;

                case MembershipLevel.SILVER:
                    strLevel = "Silver";
                    break;

                case MembershipLevel.ASSOCIATE:
                    strLevel = "Associate";
                    break;

                case MembershipLevel.INDIVIDUAL:
                    strLevel = "Individual";
                    break;

                case MembershipLevel.COURTESY:
                    strLevel = "Courtesy";
                    break;

                default:
                    strLevel = "None";
                    break;
            }

            return strLevel;
        }
    }

    /// <summary>
    /// Represents the membership level of the business
    /// </summary>
    public enum MembershipLevel
    {
        NONE = 0,
        GOLD = 1,
        SILVER = 2,
        ASSOCIATE = 3,
        INDIVIDUAL = 4,
        COURTESY = 5
    }
}
