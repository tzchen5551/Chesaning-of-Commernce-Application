using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// File Purpose:
///     This file defines the logic for the Data Import Conflict screen. This
///     screen is intended to resolve Excel import conflicts in which a row of data in Excel
///     would cause duplicated data in the Director's Portal Database. The user
///     has the option to select the rows from either the Database or Excel in which they want to keep and
///     save the changes to the database.
///         
/// </summary>
/// 
namespace DirectorsPortalWPF.SettingsUI
{
    /// <summary>
    /// Interaction logic for DataConflictPage.xaml
    /// </summary>
    public partial class DataConflictPage : Page
    {
        /// <summary>
        /// A dictionary global to the page that relates the list view model properties
        /// to their human readable names.
        /// </summary>
        readonly Dictionary<string, string> GDicHumanReadableTableFields
            = new Dictionary<string, string>();

        private List<Members> GObjDuplicateBusinesses;
        private List<Members> GObjSelectedForImport;

        /// <summary>
        /// Initialization of the Data Import Conflict Screen.
        /// </summary>
        public DataConflictPage(List<Members> duplicateBusiness)
        {
            InitializeComponent();
            PopulateHumanReadableTableFields();
            GObjDuplicateBusinesses = duplicateBusiness;
            GObjSelectedForImport = new List<Members>();
            GObjSelectedForImport.Add(new Members());
        }

        /// <summary>
        /// Returns the user back to the Settings screen where they attempted
        /// to import data using Excel
        /// </summary>
        /// <param name="sender">The 'Cancel Import' button</param>
        /// <param name="e">The Click event</param>
        private void BtnCancelLoad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Used to confirm the data the user want to save to the database and 
        /// resolve the data conflict between Director's Portal and Excel Import.
        /// </summary>
        /// <param name="sender">The 'Resolve and Save' Button</param>
        /// <param name="e">The Click Event</param>
        private void BtnResolveConflict_Click(object sender, RoutedEventArgs e)
        {
            foreach (Members currentMember in GObjSelectedForImport)
            {
                Business selectedBusiness = new Business();
                using (DatabaseContext context = new DatabaseContext())
                {
                    selectedBusiness = context.Businesses
                        .Include(x => x.MailingAddress)
                        .Include(x => x.PhysicalAddress)
                        .Include(x => x.BusinessReps)
                        .ThenInclude(x => x.ContactPerson)
                        .ThenInclude(x => x.Emails)
                        .Include(x => x.BusinessReps)
                        .ThenInclude(x => x.ContactPerson)
                        .ThenInclude(x => x.PhoneNumbers)
                        .FirstOrDefault(business => business.BusinessName.Equals(currentMember.gstrBusinessName));
                }

                if (selectedBusiness != null)
                {
                    DeleteMember(selectedBusiness);
                }
            }

            ImportToDatabase(GObjSelectedForImport);
            // Template code, need functionality to be implemented.
            NavigationService.GoBack();
        }

        /// <summary>
        /// Pending Implementation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkResolve_Click(object sender, RoutedEventArgs e)
        {
            // Should be used to mark an item to be saved to the Database.
            CheckBox btn = sender as CheckBox;
            BusinessTableViewModel selectedTableViewModel = btn.DataContext as BusinessTableViewModel;

            string selectedBusinessName = selectedTableViewModel.StrBuisnessName;
            Members selectedMember = GObjDuplicateBusinesses.Find(x => x.gstrBusinessName.Equals(selectedTableViewModel.StrBuisnessName));

/*            Business selectedBusiness = new Business();
            using (DatabaseContext context = new DatabaseContext())
            {
                selectedBusiness = context.Businesses
                    .Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers)
                    .FirstOrDefault(business => business.BusinessName.Equals(selectedTableViewModel.StrBuisnessName));
            }*/

            if (selectedMember != null)
                GObjSelectedForImport.Add(selectedMember);
        }

        /// <summary>
        /// Intended to populate the right table on the Database Conflict Screen. The right screen will
        /// be used to show items in the Excel import that conflict with 
        /// the Director's Portal Database.
        /// </summary>
        /// <param name="sender">The list view that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void LvExcelConflict_Loaded(object sender, RoutedEventArgs e)
        {
            /* Load all member data from the Buisness table into the list view. */
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                List<BusinessTableViewModel> lstTableViewModel = new List<BusinessTableViewModel>();

                foreach (Members busCurrentBusiness in GObjDuplicateBusinesses)
                {
                    BusinessTableViewModel objBusinessTableView = new BusinessTableViewModel
                    {
                        StrBuisnessName = busCurrentBusiness.gstrBusinessName
                    };

                    /* Get the associated addresses for this business. */
                    // Address objLocationAddress = dbContext.Addresses.Find(busCurrentBusiness.);
                    // Address objMalingAddress = dbContext.Addresses.Find(busCurrentBusiness.MailingAddress);




                    objBusinessTableView.StrLocationAddress = busCurrentBusiness?.gstrLocationAddress;
                    objBusinessTableView.StrMailingAddress = busCurrentBusiness?.gstrMailingAddress;

                    if (busCurrentBusiness.gstrCityStateZip.Split(',').Count() > 0)
                        objBusinessTableView.StrCity = busCurrentBusiness?.gstrCityStateZip.Split(',')[0];
                    if (busCurrentBusiness.gstrCityStateZip.Split(',').Count() > 1)
                        objBusinessTableView.StrState = busCurrentBusiness?.gstrCityStateZip.Split(',')[1].Trim().Split(' ')[0];
                    if (busCurrentBusiness.gstrCityStateZip.Split(',').Count() > 1)
                        objBusinessTableView.IntZipCode = busCurrentBusiness?.gstrCityStateZip.Split(',')[1].Trim().Split(' ')[1];

                    /* Get the business rep from the database. */
                    /* TODO: Need to figure out a way to display more than one buisiness rep. */
                    // BusinessRep objBusinessRep = dbContext.BusinessReps
                    //    .Where(r => r.BusinessId == busCurrentBusiness.Id).FirstOrDefault();

                    // if (objBusinessRep == null)
                    // {
                    //    /* If there is no business rep then the business will not have any contact person
                    //     * related inforamtion. */
                    //    objBusinessTableView.StrContactPerson = "";
                    //    objBusinessTableView.StrPhoneNumber = "";
                    //    objBusinessTableView.StrFaxNumber = "";
                    //    objBusinessTableView.StrEmailAddress = "";
                    //}
                    //else
                    {
                        // ContactPerson objContactPerson = dbContext.ContactPeople.Find(objBusinessRep.ContactPersonId);

                        objBusinessTableView.StrContactPerson = busCurrentBusiness?.gstrContactPerson;

                        /* Get the phone and fax number of the contact person. */
                        // List<PhoneNumber> objPhoneNumbers = dbContext.PhoneNumbers
                        //    .Where(pn => pn.ContactPersonId == objContactPerson.Id).ToList();

                        //foreach (PhoneNumber objCurrentPhoneNumber in objPhoneNumbers)
                        {
                        //    if (objCurrentPhoneNumber.GEnumPhoneType == PhoneType.Mobile ||
                        //        objCurrentPhoneNumber.GEnumPhoneType == PhoneType.Office)
                            {
                                objBusinessTableView.StrPhoneNumber = busCurrentBusiness?.gstrPhoneNumber;
                            }
                            //else
                            {
                                objBusinessTableView.StrFaxNumber = busCurrentBusiness?.gstrFaxNumber;
                            }
                        }

                        /* Get the contacts persons email address. */
                        // Email objEmail = dbContext.Emails
                        //    .Where(ea => ea.ContactPersonId == objContactPerson.Id).FirstOrDefault();
                        objBusinessTableView.StrEmailAddress = busCurrentBusiness?.gstrEmailAddress;
                    }

                    objBusinessTableView.StrWebsite = busCurrentBusiness?.gstrWebsiteAddress;
                    objBusinessTableView.StrLevel = busCurrentBusiness?.gstrLevel;

                    objBusinessTableView.IntEstablishedYear = busCurrentBusiness?.gstrEstablished;
                    lstTableViewModel.Add(objBusinessTableView);
                }

                /* Add a check box to allow user to check off rows of data they want to keep in the system. */
                var btnFactoryCheckResolve = new FrameworkElementFactory(typeof(CheckBox));
                btnFactoryCheckResolve.SetValue(ContentProperty, "");
                btnFactoryCheckResolve.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(ChkResolve_Click));

                DataTemplate dtResolve = new DataTemplate()
                {
                    VisualTree = btnFactoryCheckResolve
                };

                GridViewColumn gvcEdit = new GridViewColumn
                {
                    CellTemplate = dtResolve,
                    Header = "Overwrite"
                };

                gvExcelConflictRows.Columns.Add(gvcEdit);

                /* Create the columns for the table. */
                Type typeTableViewModel = typeof(BusinessTableViewModel);
                foreach (var property in typeTableViewModel.GetProperties())
                {
                    GridViewColumn gvcCol = new GridViewColumn
                    {
                        Header = GDicHumanReadableTableFields[property.Name],
                        DisplayMemberBinding = new Binding(property.Name)
                    };
                    gvExcelConflictRows.Columns.Add(gvcCol);
                }

                lvExcelConflict.ItemsSource = lstTableViewModel;
            }
        }

        /// <summary>
        /// A method for popualting the human readable names into the
        /// global dictionary. These fields will be used to relate the names of
        /// the properties to the their human readable column headers.
        /// </summary>
        private void PopulateHumanReadableTableFields()
        {
            /* TODO: This method will need to change as we develope a solution for dynamic
             * fields. */
            GDicHumanReadableTableFields.Clear();

            GDicHumanReadableTableFields.Add("StrBuisnessName", "Business Name");
            GDicHumanReadableTableFields.Add("StrMailingAddress", "Mailing Address");
            GDicHumanReadableTableFields.Add("StrLocationAddress", "Location Address");
            GDicHumanReadableTableFields.Add("StrCity", "City");
            GDicHumanReadableTableFields.Add("StrState", "State");
            GDicHumanReadableTableFields.Add("IntZipCode", "Zip Code");
            GDicHumanReadableTableFields.Add("StrContactPerson", "Contact");
            GDicHumanReadableTableFields.Add("StrPhoneNumber", "Phone Number");
            GDicHumanReadableTableFields.Add("StrFaxNumber", "Fax Number");
            GDicHumanReadableTableFields.Add("StrEmailAddress", "Email Address");
            GDicHumanReadableTableFields.Add("StrWebsite", "Website");
            GDicHumanReadableTableFields.Add("StrLevel", "Level");
            GDicHumanReadableTableFields.Add("IntEstablishedYear", "Established");
        }

        /// <summary>
        /// A method to check if an int in the database is null (or 0).
        /// 
        /// This ensures that null integers are left empty in the table instead of being
        /// displayed as 0.
        /// </summary>
        /// <param name="intCheck">Value to check for null (or 0)</param>
        /// <returns>
        /// If the int in the database is null (or 0) then null is returned.
        /// If the int in the databse is not null then the original value is returned.
        /// </returns>
        public int? CheckNullableInt(int? intCheck)
        {
            if (intCheck == 0)
            {
                return null;
            }
            else
            {
                return intCheck;
            }
        }


        /// <summary>
        /// A method for converting the membership level from the business entity to
        /// a human readable string.
        /// </summary>
        /// <param name="membershipLevel">The membership level from the business entity.</param>
        /// <returns>The human readable membership string.</returns>
        private string GetMebershipLevelString(MembershipLevel membershipLevel)
        {
            string strLevel = "";

            switch (membershipLevel)
            {
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
            }

            return strLevel;
        }

        /// <summary>
        /// A mehtod for removing a business and all it's info from the database.
        /// This method is called by the delete button on the edit member screen.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void DeleteMember(Business MSelectedBusiness)
        {

            /* Remove the member and return to the members page. */
            using (DatabaseContext context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        /* Remove the businesses addresses. */
                        context.Remove(MSelectedBusiness.MailingAddress);
                        context.Remove(MSelectedBusiness.PhysicalAddress);

                        /* Remove all the business reps for the business.
                            * This includes contact people, emails, and phone numbers.*/
                        foreach (BusinessRep rep in MSelectedBusiness.BusinessReps)
                        {
                            context.Remove(rep.ContactPerson);
                            context.Remove(rep);
                        }

                        /* Finally, remove the business. */
                        context.Remove(MSelectedBusiness);

                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        /// Take in a List of members to sort and import data into the database
        /// </summary>
        /// <param name="Data">List of opbject members.</param>
        public void ImportToDatabase(List<Members> Data)
        {
            List<string> lstStrFailedDataImports = new List<string>();

            using (var context = new DatabaseContext())
            {
                for (int intCounter = 1; intCounter <= Data.Count - 1; intCounter++)
                {
                    if (Data[intCounter].gstrBusinessName.Equals(""))
                    {
                        break;
                    }

                    String strCityStateZip = Data[intCounter].gstrCityStateZip;
                    String strLevelData = Data[intCounter].gstrLevel;
                    String strAdditionalNote = "";

                    String[] arrSplit = strCityStateZip.Split(',');
                    String strCityData = arrSplit[0];

                    String[] arrSplitStateZip = arrSplit[1].Split(' ');
                    String strStateData = arrSplitStateZip[1];
                    String strZipData = arrSplitStateZip[2];

                    try
                    {
                        Address objMailingAddress = new Address()
                        {
                            StreetAddress = Data[intCounter].gstrMailingAddress,
                            City = strCityData,
                            State = strStateData,
                        };

                        Address objLocationAddress = new Address()
                        {
                            StreetAddress = Data[intCounter].gstrLocationAddress,
                            City = strCityData,
                            State = strStateData,
                        };

                        if (strZipData.Length > 5)
                        {
                            String[] arrZipSplit = strZipData.Split('-');

                            objMailingAddress.ZipCode = Int32.Parse(arrZipSplit[0]);
                            objLocationAddress.ZipCode = Int32.Parse(arrZipSplit[0]);
                            objMailingAddress.ZipCodeExt = strZipData;
                            objLocationAddress.ZipCodeExt = strZipData;
                        }
                        else
                        {
                            objMailingAddress.ZipCode = Int32.Parse(strZipData);
                            objLocationAddress.ZipCode = Int32.Parse(strZipData);
                            objMailingAddress.ZipCodeExt = strZipData;
                            objLocationAddress.ZipCodeExt = strZipData;
                        };

                        String strLocationAddress = Data[intCounter].gstrLocationAddress;
                        String[] arrLocationSplit = strLocationAddress.Split(',');
                        String[] arrLocationSplitSlash = strLocationAddress.Split('/');
                        String[] arrLocationSplitAnd = strLocationAddress.Split('&');

                        if (arrLocationSplit.Length == 3)
                        {
                            String[] arrStateZip = arrLocationSplit[2].Split(' ');

                            strLocationAddress = arrLocationSplit[1] + arrLocationSplit[2];
                            objLocationAddress.StreetAddress = arrLocationSplit[0];
                            objLocationAddress.City = arrLocationSplit[1];
                            objLocationAddress.State = arrStateZip[1];
                            objLocationAddress.ZipCode = Int32.Parse(arrStateZip[2]);
                            objLocationAddress.ZipCodeExt = strZipData;

                        }
                        else if (arrLocationSplitSlash.Length > 1)
                        {
                            objLocationAddress.StreetAddress = arrLocationSplit[0];
                            strAdditionalNote = " - Additional Location Addresses : ";

                            for (int intCount = 1; intCount < arrLocationSplitSlash.Length; intCount++)
                            {
                                strAdditionalNote = strAdditionalNote + "  " + arrLocationSplitSlash[intCount] + " " + strCityStateZip;
                            }
                        }

                        arrLocationSplit = strLocationAddress.Split('&');

                        if (arrLocationSplit.Length > 1)
                        {
                            objLocationAddress.StreetAddress = arrLocationSplit[0];

                            strAdditionalNote = " - Additional Location Addresses : ";

                            for (int intCount = 1; intCount < arrLocationSplit.Length; intCount++)
                            {
                                arrLocationSplitSlash = arrLocationSplit[intCount].Split(',');

                                if (arrLocationSplitSlash.Length > 1)
                                {
                                    strAdditionalNote = strAdditionalNote + "  " + arrLocationSplit[intCount];

                                }
                                else
                                {
                                    strAdditionalNote = strAdditionalNote + "  " + arrLocationSplit[intCount] + " " + strCityStateZip;
                                }

                            }
                        }

                        context.Addresses.Add(objMailingAddress);
                        context.Addresses.Add(objLocationAddress);

                        Business objBusiness = new Business()
                        {
                            BusinessName = Data[intCounter].gstrBusinessName,
                            YearEstablished = Int32.Parse(Data[intCounter].gstrEstablished),
                            Website = Data[intCounter].gstrWebsiteAddress,
                            ExtraNotes = Data[intCounter].gstrNotes + " " + strAdditionalNote,
                            MailingAddress = objMailingAddress,
                            PhysicalAddress = objLocationAddress

                        };

                        if (strLevelData.Equals("Gold"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.GOLD;
                        }
                        else if (strLevelData.Equals("Silver"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.SILVER;
                        }
                        else if (strLevelData.Equals("Associate"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.ASSOCIATE;
                        }
                        else if (strLevelData.Equals("Individual"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.INDIVIDUAL;
                        }
                        else if (strLevelData.Equals("Courtesy"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.COURTESY;
                        }

                        context.Businesses.Add(objBusiness);



                        arrLocationSplit = Data[intCounter].gstrContactPerson.Split('&');
                        arrLocationSplitSlash = Data[intCounter].gstrContactPerson.Split('/');
                        arrLocationSplitAnd = Data[intCounter].gstrPhoneNumber.Split('/');
                        arrSplit = Data[intCounter].gstrEmailAddress.Split(',');

                        if (arrLocationSplit.Length > 1)
                        {
                            for (int intCount = 0; intCount < arrLocationSplit.Length; intCount++)
                            {
                                ContactPerson objContactPerson = new ContactPerson()
                                {
                                    Name = arrLocationSplit[intCount]
                                };

                                context.ContactPeople.Add(objContactPerson);

                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrPhoneNumber,
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);

                                if (Data[intCounter].gstrFaxNumber.Length > 1)
                                {
                                    PhoneNumber objFaxNumber = new PhoneNumber()
                                    {
                                        ContactPerson = objContactPerson,
                                        Number = Data[intCounter].gstrFaxNumber,
                                        GEnumPhoneType = PhoneType.Fax
                                    };

                                    context.PhoneNumbers.Add(objFaxNumber);
                                }

                                BusinessRep objBusinessRep = new BusinessRep()
                                {
                                    Business = objBusiness,
                                    ContactPerson = objContactPerson
                                };

                                context.BusinessReps.Add(objBusinessRep);

                                for (int intCountEmail = 0; intCountEmail < arrSplit.Length; intCountEmail++)
                                {

                                    Email objEmail = new Email()
                                    {
                                        ContactPerson = objContactPerson,
                                        EmailAddress = arrSplit[intCountEmail]
                                    };

                                    context.Emails.Add(objEmail);
                                }
                            }
                        }
                        else if (arrLocationSplitSlash.Length > 1 && arrLocationSplitAnd.Length > 1)
                        {
                            for (int intCount = 0; intCount < arrLocationSplitSlash.Length; intCount++)
                            {
                                ContactPerson objContactPerson = new ContactPerson()
                                {
                                    Name = arrLocationSplitSlash[intCount]
                                };

                                context.ContactPeople.Add(objContactPerson);

                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = arrLocationSplitAnd[intCount],
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);

                                if (Data[intCounter].gstrFaxNumber.Length > 1)
                                {
                                    PhoneNumber objFaxNumber = new PhoneNumber()
                                    {
                                        ContactPerson = objContactPerson,
                                        Number = Data[intCounter].gstrFaxNumber,
                                        GEnumPhoneType = PhoneType.Fax
                                    };
                                    context.PhoneNumbers.Add(objFaxNumber);
                                }

                                BusinessRep objBusinessRep = new BusinessRep()
                                {
                                    Business = objBusiness,
                                    ContactPerson = objContactPerson
                                };

                                context.BusinessReps.Add(objBusinessRep);

                                for (int intCountEmail = 0; intCountEmail < arrSplit.Length; intCountEmail++)
                                {

                                    Email objEmail = new Email()
                                    {
                                        ContactPerson = objContactPerson,
                                        EmailAddress = arrSplit[intCountEmail]
                                    };

                                    context.Emails.Add(objEmail);
                                }
                            }
                        }
                        else if (Data[intCounter].gstrContactPerson.Length > 0)
                        {

                            ContactPerson objContactPerson = new ContactPerson()
                            {
                                Name = Data[intCounter].gstrContactPerson
                            };

                            context.ContactPeople.Add(objContactPerson);


                            for (int intCount = 0; intCount < arrLocationSplitAnd.Length; intCount++)
                            {
                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrPhoneNumber,
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);
                            }
                            if (Data[intCounter].gstrFaxNumber.Length > 1)
                            {
                                PhoneNumber objFaxNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrFaxNumber,
                                    GEnumPhoneType = PhoneType.Fax
                                };

                                context.PhoneNumbers.Add(objFaxNumber);
                            }

                            BusinessRep objBusinessRep = new BusinessRep()
                            {
                                Business = objBusiness,
                                ContactPerson = objContactPerson
                            };

                            context.BusinessReps.Add(objBusinessRep);

                            for (int intCount = 0; intCount < arrSplit.Length; intCount++)
                            {

                                Email objEmail = new Email()
                                {
                                    ContactPerson = objContactPerson,
                                    EmailAddress = arrSplit[intCount]
                                };

                                context.Emails.Add(objEmail);
                            }
                        }

                        DateTime strCurrentYear = DateTime.Now;

                        YearlyData objYearlyData = new YearlyData
                        {
                            Year = Int32.Parse(strCurrentYear.Year.ToString()),
                            Business = objBusiness,
                            DuesPaid = Double.Parse(Data[intCounter].gstrDuesPaid),
                            TicketsReturned = Double.Parse(Data[intCounter].gstrRaffleTicketReturnedPaid),
                            Credit = Double.Parse(Data[intCounter].gstrCredit),
                            BallotNumber = Int32.Parse(Data[intCounter].gstrBallot),

                        };

                        if (Data[intCounter].gstrTerms.Equals("Annually"))
                        {
                            objYearlyData.TermLength = TermLength.Annually;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Semiannually"))
                        {
                            objYearlyData.TermLength = TermLength.Semiannually;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Quarterly"))
                        {
                            objYearlyData.TermLength = TermLength.Quarterly;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Monthly"))
                        {
                            objYearlyData.TermLength = TermLength.Monthly;
                        }

                        context.BusinessYearlyData.Add(objYearlyData);

                        context.SaveChanges();
                    }
                    catch
                    {
                        lstStrFailedDataImports.Add(Data[intCounter].gstrBusinessName);

                    }
                }
            }

            string strMessage = string.Join(Environment.NewLine, lstStrFailedDataImports);

            if (strMessage.Length > 0)
            {
                MessageBox.Show("ALL DATA ENTRY FAILED TO IMPORT TO DATABASE BELOW \n" + strMessage, "Excel Import Notice",
                       MessageBoxButton.OK,
                       MessageBoxImage.Information);

            }
        }
    }


    /// <summary>
    /// A view model that defines the members table.
    /// </summary>
    public class BusinessTableViewModel
    {
        public string StrBuisnessName { get; set; }
        public string StrMailingAddress { get; set; }
        public string StrLocationAddress { get; set; }
        public string StrCity { get; set; }
        public string StrState { get; set; }
        public string IntZipCode { get; set; }
        public string StrContactPerson { get; set; }
        public string StrPhoneNumber { get; set; }
        public string StrFaxNumber { get; set; }
        public string StrEmailAddress { get; set; }
        public string StrWebsite { get; set; }
        public string StrLevel { get; set; }
        public string IntEstablishedYear { get; set; }
    }
}
