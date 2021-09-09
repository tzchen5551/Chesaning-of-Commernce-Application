using DirectorPortalDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Utility
{
    public class JoinHelper
    {
        public enum EnumTable
        {
            Address, Business, BusinessRep, Categories, CategoryRef, ContactPerson, Email, EmailGroup, EmailGroupMember,
            Payment, PaymentItem, PhoneNumber, YearlyData
        }

        public class JoinResult
        {
            private IEnumerable<JoinResultRecord> EnmRecords { get; set; }
            /// <summary>
            /// Table enums are removed from this list when the JoinResult joins with them.
            /// </summary>
            private List<EnumTable> RGTablesToJoinWith { get; set; }

            /// <summary>
            /// Table enums are added to this list when they are joined into the result.
            /// </summary>
            private List<EnumTable> RGTablesJoined { get; set; }

            /// <summary>
            /// Indicates whether to perform left joins instead of inner joins.
            /// </summary>
            private readonly bool BlnUseLeftJoins;

            public JoinResultRecord[] RGRecords => EnmRecords.ToArray();

            public JoinResult (DatabaseContext dbContext, List<EnumTable> rgTablesToJoinWith, bool blnUseLeftJoins = true)
            {
                BlnUseLeftJoins = blnUseLeftJoins;
                // The argument's list elements are copied into this object's list.
                this.RGTablesToJoinWith = new List<EnumTable>(rgTablesToJoinWith.Count);
                foreach (EnumTable enumTable in rgTablesToJoinWith)
                {
                    this.RGTablesToJoinWith.Add(enumTable);
                }
                this.RGTablesJoined = new List<EnumTable>();
                // The data retrieval always begins by selecting all records from the central (Businesses) table.
                this.EnmRecords = dbContext.Businesses.AsEnumerable().Select(business => new JoinResultRecord { UdtBusiness = business });
                this.RGTablesJoined.Add(EnumTable.Business);
                this.RGTablesToJoinWith.Remove(EnumTable.Business);

                // Performs all joins necessary to retrieve all requested data.
                JoinUntilDone(dbContext);
            }

            /// <summary>
            /// Performs a join on the specified table and replaces the current result set with the join result.
            /// </summary>
            /// <param name="dbContext"></param>
            /// <param name="enumTable"></param>
            private void JoinWith(DatabaseContext dbContext, EnumTable enumTable)
            {
                List<JoinResultRecord> rgDataCopy = null;

                if (BlnUseLeftJoins)
                    // Copies all the current data so that records excluded from the result of the inner join
                    // can be added back to the dataset afterwords.
                    rgDataCopy = EnmRecords.ToList();

                if (enumTable == EnumTable.Address)
                {
                    // The Addresses table is handled differently because it is connected to the Businesses
                    // table by two associations, rather than one. Hence, two joins are performed, and the
                    // union of their results is taken.
                    IQueryable<Address> qryAddresses = dbContext.Addresses;
                    // Joins on mailing address id.
                    IEnumerable<JoinResultRecord> enmJoinResult1 = EnmRecords.Join(qryAddresses,
                        item => item.UdtBusiness?.MailingAddressId,
                        address => address.Id,
                        (item, address) => item.CopyAndReplace(address));
                    // Joins on physical address id.
                    IEnumerable<JoinResultRecord> enmJoinResult2 = EnmRecords.Join(qryAddresses,
                        item => item.UdtBusiness?.PhysicalAddressId,
                        address => address.Id,
                        (item, address) => item.CopyAndReplace(address));
                    // Takes the union of these join results.
                    EnmRecords = enmJoinResult1.Union(enmJoinResult2);
                }
                else if (enumTable == EnumTable.BusinessRep)
                {
                    EnmRecords = EnmRecords.Join(dbContext.BusinessReps,
                        item => item.UdtBusiness?.Id,
                        businessRep => businessRep.BusinessId,
                        (item, businessRep) => item.CopyAndReplace(businessRep));
                }
                else if (enumTable == EnumTable.Categories)
                {
                    // Categories are accessed via CategoryRefs.
                    EnmRecords = EnmRecords.Join(dbContext.Categories,
                        item => item.UdtCategoryRef?.CategoryId,
                        categories => categories.Id,
                        (item, categories) => item.CopyAndReplace(categories));
                }
                else if (enumTable == EnumTable.CategoryRef)
                {
                    EnmRecords = EnmRecords.Join(dbContext.CategoryRef,
                        item => item.UdtBusiness?.Id,
                        categoryRef => categoryRef.BusinessId,
                        (item, categoryRef) => item.CopyAndReplace(categoryRef));
                }
                else if (enumTable == EnumTable.ContactPerson)
                {
                    // ContactPeople are accessed via BusinessReps.
                    EnmRecords = EnmRecords.Join(dbContext.ContactPeople,
                        item => item.UdtBusinessRep?.ContactPersonId,
                        contactPerson => contactPerson.Id,
                        (item, contactPerson) => item.CopyAndReplace(contactPerson));
                }
                else if (enumTable == EnumTable.Email)
                {
                    // Emails are accessed via ContactPeople.
                    EnmRecords = EnmRecords.Join(dbContext.Emails,
                        item => item.UdtContactPerson?.Id,
                        email => email.ContactPersonId,
                        (item, email) => item.CopyAndReplace(email));
                }
                else if (enumTable == EnumTable.EmailGroup)
                {
                    // EmailGroups are accessed via EmailGroupMembers.
                    EnmRecords = EnmRecords.Join(dbContext.EmailGroups,
                        item => item.UdtEmailGroupMember?.GroupId,
                        emailGroup => emailGroup.Id,
                        (item, emailGroup) => item.CopyAndReplace(emailGroup));
                }
                else if (enumTable == EnumTable.EmailGroupMember)
                {
                    // EmailGroupMembers are accessed via Emails.
                    EnmRecords = EnmRecords.Join(dbContext.EmailGroupMembers,
                        item => item.UdtEmail?.Id,
                        emailGroupMember => emailGroupMember.EmailId,
                        (item, emailGroupMember) => item.CopyAndReplace(emailGroupMember));
                }
                else if (enumTable == EnumTable.Payment)
                {
                    IQueryable<Payment> qryPayments = dbContext.Payments.Include(payment => payment.Business);
                    EnmRecords = EnmRecords.Join(qryPayments,
                        item => item.UdtBusiness?.Id,
                        payment => payment.Business.Id,
                        (item, payment) => item.CopyAndReplace(payment));
                }
                else if (enumTable == EnumTable.PaymentItem)
                {
                    // PaymentItems are accessed via Payments.
                    IQueryable<PaymentItem> qryPaymentItems = dbContext.PaymentItems.Include(paymentItem => paymentItem.Payment);
                    EnmRecords = EnmRecords.Join(qryPaymentItems,
                        item => item.UdtPayment?.Id,
                        paymentItem => paymentItem.Payment.Id,
                        (item, paymentItem) => item.CopyAndReplace(paymentItem));
                }
                else if (enumTable == EnumTable.PhoneNumber)
                {
                    // PhoneNumbers are accessed via ContactPeople.
                    EnmRecords = EnmRecords.Join(dbContext.PhoneNumbers,
                        item => item.UdtContactPerson?.Id,
                        phoneNumber => phoneNumber.ContactPersonId,
                        (item, phoneNumber) => item.CopyAndReplace(phoneNumber));
                }
                else if (enumTable == EnumTable.YearlyData)
                {
                    EnmRecords = EnmRecords.Join(dbContext.BusinessYearlyData,
                        item => item.UdtBusiness?.Id,
                        yearlyData => yearlyData.BusinessId,
                        (item, yearlyData) => item.CopyAndReplace(yearlyData));
                }
                else
                {
                    throw new ArgumentException($"Cannot join with this table: {enumTable}");
                }

                if (BlnUseLeftJoins)
                {
                    // Takes the difference between the original dataset and the current one.
                    // The records left behind are the ones that were excluded from the inner join result.
                    IEnumerable<JoinResultRecord> enmMissingRecords = rgDataCopy.Except(EnmRecords);

                    // Adds the missing records back into the dataset.
                    EnmRecords = EnmRecords.Union(enmMissingRecords);
                }

                RGTablesJoined.Add(enumTable);
                RGTablesToJoinWith.Remove(enumTable);
            }

            /// <summary>
            /// Performs joins until the list of tables to join with is empty.
            /// </summary>
            /// <param name="dbContext"></param>
            private void JoinUntilDone(DatabaseContext dbContext)
            {
                // Iterates until there are no more tables to join with.
                while (RGTablesToJoinWith.Count > 0)
                {
                    // Gets one of the tables that must be joined with.
                    EnumTable enumTarget = RGTablesToJoinWith[0];

                    // Traverses back one table toward the central table.
                    EnumTable enumBeforeTarget = GetPrevTable(enumTarget);

                    // Traverses backwards until the target is one step away from the 
                    // set of tables that have been joined with.
                    while (!RGTablesJoined.Contains(enumBeforeTarget))
                    {
                        enumTarget = enumBeforeTarget;
                        enumBeforeTarget = GetPrevTable(enumBeforeTarget);
                    }

                    // Joins with the target table.
                    JoinWith(dbContext, enumTarget);
                }
            }
        }

        /// <summary>
        /// Returns the next table along the path from one table to another. Use this method when traversing the database tables.
        /// </summary>
        /// <param name="enumFromTable"></param>
        /// <param name="enumToTable"></param>
        /// <returns></returns>
        public static EnumTable GetNextTable(EnumTable enumFromTable, EnumTable enumToTable)
        {
            if (enumFromTable == enumToTable)
            {
                throw new ArgumentException("Invalid Join: self-join");
            }
            else
            {
                switch (enumFromTable)
                {
                    case EnumTable.Business:
                        switch (enumToTable)
                        {
                            case EnumTable.Address:
                                return EnumTable.Address;

                            case EnumTable.YearlyData:
                                return EnumTable.YearlyData;

                            case EnumTable.CategoryRef:
                            case EnumTable.Categories:
                                return EnumTable.CategoryRef;

                            case EnumTable.Payment:
                            case EnumTable.PaymentItem:
                                return EnumTable.Payment;

                            case EnumTable.BusinessRep:
                            case EnumTable.ContactPerson:
                            case EnumTable.PhoneNumber:
                            case EnumTable.Email:
                            case EnumTable.EmailGroupMember:
                            case EnumTable.EmailGroup:
                                return EnumTable.BusinessRep;

                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.BusinessRep:
                        switch (enumFromTable)
                        {
                            case EnumTable.ContactPerson:
                            case EnumTable.Email:
                            case EnumTable.EmailGroup:
                            case EnumTable.EmailGroupMember:
                            case EnumTable.PhoneNumber:
                                return EnumTable.ContactPerson;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.CategoryRef:
                        switch (enumFromTable)
                        {
                            case EnumTable.Categories:
                                return EnumTable.Categories;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.ContactPerson:
                        switch (enumFromTable)
                        {
                            case EnumTable.PhoneNumber:
                                return EnumTable.PhoneNumber;
                            case EnumTable.Email:
                            case EnumTable.EmailGroupMember:
                            case EnumTable.EmailGroup:
                                return EnumTable.Email;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.Email:
                        switch (enumFromTable)
                        {
                            case EnumTable.EmailGroupMember:
                            case EnumTable.EmailGroup:
                                return EnumTable.EmailGroupMember;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.EmailGroupMember:
                        switch (enumFromTable)
                        {
                            case EnumTable.EmailGroup:
                                return EnumTable.EmailGroup;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    case EnumTable.Payment:
                        switch (enumFromTable)
                        {
                            case EnumTable.PaymentItem:
                                return EnumTable.PaymentItem;
                            default:
                                throw new ArgumentException("Attempted to traverse database in the wrong direction");
                        }
                    default:
                        throw new ArgumentException("Attempted to traverse database in the wrong direction");
                }
            }
        }

        /// <summary>
        /// Returns the next table along the path from the destination table to the Businesses table. Use this method when working
        /// backwards from the destination to the start.
        /// </summary>
        /// <param name="enumDestinationTable"></param>
        /// <returns></returns>
        public static EnumTable GetPrevTable(EnumTable enumDestinationTable)
        {
            switch (enumDestinationTable)
            {
                case EnumTable.Address:
                case EnumTable.BusinessRep:
                case EnumTable.CategoryRef:
                case EnumTable.Payment:
                case EnumTable.YearlyData:
                    return EnumTable.Business;

                case EnumTable.Categories:
                    return EnumTable.CategoryRef;

                case EnumTable.PaymentItem:
                    return EnumTable.Payment;

                case EnumTable.ContactPerson:
                    return EnumTable.BusinessRep;

                case EnumTable.Email:
                case EnumTable.PhoneNumber:
                    return EnumTable.ContactPerson;

                case EnumTable.EmailGroup:
                    return EnumTable.EmailGroupMember;

                case EnumTable.EmailGroupMember:
                    return EnumTable.Email;

                case EnumTable.Business:
                    throw new ArgumentException("Already at the start table.");

                default:
                    throw new ArgumentException("Invalid table enum value");
            }
        }
    }
}
