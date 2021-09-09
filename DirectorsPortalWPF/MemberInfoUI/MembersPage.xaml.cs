using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using DirectorPortalDatabase.Utility;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DirectorsPortalConstantContact;


namespace DirectorsPortalWPF.MemberInfoUI
{
    /// <summary>
    /// Interaction logic for MembersPage.xaml
    /// </summary>
    public partial class MembersPage : Page
    {
        /// <summary>
        /// A dictionary global to the page that relates the list view model properties
        /// to their human readable names.
        /// </summary>
        public static Dictionary<string, string> GDicHumanReadableTableFields 
            = new Dictionary<string, string>();

        private ConstantContact gObjConstContact;
        /// <summary>
        /// Intializes the Page and content within the Page and populates the
        /// dictionary of human readable property names.
        /// </summary>
        public MembersPage()
        {
            InitializeComponent();

            /* Repopulate the dictionary to accoutn for any field changes. */
            /* TODO: This is currently just a concept for when we have a better idea on how
             * dynamic tables will function. */
            GDicHumanReadableTableFields.Clear();
            GDicHumanReadableTableFields = BusinessTableViewModel.PopulateHumanReadableTableDic();
        }
        /// <summary>
        /// Intializes the Page and content within the Page and populates the
        /// dictionary of human readable property names.
        /// </summary>
        public MembersPage(ConstantContact gObjConstContact)
        {
            InitializeComponent();

            /* Repopulate the dictionary to accoutn for any field changes. */
            /* TODO: This is currently just a concept for when we have a better idea on how
             * dynamic tables will function. */
            GDicHumanReadableTableFields.Clear();
            GDicHumanReadableTableFields = BusinessTableViewModel.PopulateHumanReadableTableDic();
            this.gObjConstContact = gObjConstContact;
        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ModifyMembersPage(null, null, gObjConstContact));
        }

        /// <summary>
        /// Opens a pop-up window that displays the current frames help information. 
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">The Click event</param>
        public void HelpButtonHandler(object sender, EventArgs e)
        {
            HelpUI.HelpScreenWindow helpWindow = new HelpUI.HelpScreenWindow();
            helpWindow.Show();
            helpWindow.tabs.SelectedIndex = 0;

        }
        /// <summary>
        /// Pending Implementation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEditBusiness_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            BusinessTableViewModel selectedTableViewModel = btn.DataContext as BusinessTableViewModel;

            Business selectedBusiness = new Business();
            using (DatabaseContext context = new DatabaseContext()) 
            {
                selectedBusiness = context.Businesses
                    .Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.CategoryRefs)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers)
                    .FirstOrDefault(business => business.BusinessName.Equals(selectedTableViewModel.StrBuisnessName));
            }

            NavigationService.Navigate(new ModifyMembersPage(null, selectedBusiness, gObjConstContact));
        }

        /// <summary>
        /// A method to populate the lvMemberInfo with the business info once it has been
        /// loaded onto the page.
        /// </summary>
        /// <param name="sender">The list view that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void LvMemberInfo_Loaded(object sender, RoutedEventArgs e)
        {
            /* Load all member data from the Buisness table into the list view. */
            using (DatabaseContext context = new DatabaseContext()) 
            {
                List<BusinessTableViewModel> lstTableViewModel = new List<BusinessTableViewModel>();

                List<Business> lstBusiness = context.Businesses
                    .Include(bus => bus.MailingAddress)
                    .Include(bus => bus.PhysicalAddress)
                    .ToList();
                foreach (Business business in lstBusiness) 
                {
                    BusinessTableViewModel businessTableView = new BusinessTableViewModel();

                    businessTableView.StrBuisnessName = business.BusinessName;

                    /* Get the associated addresses for this business. */
                    //Address locationAddress = context.Addresses.Find(business.PhysicalAddressId);
                    //Address malingAddress = context.Addresses.Find(business.MailingAddressId);
                    Address locationAddress = business.PhysicalAddress;
                    Address malingAddress = business.MailingAddress;

                    businessTableView.StrLocationAddress = locationAddress?.StreetAddress;
                    businessTableView.StrMailingAddress = malingAddress?.StreetAddress;
                    businessTableView.StrCity = malingAddress?.City;
                    businessTableView.StrState = malingAddress?.State;
                    businessTableView.IntZipCode = CheckNullableInt(malingAddress?.ZipCode);

                    /* Get the business rep from the database. */
                    /* TODO: Need to figure out a way to display more than one buisiness rep. */
                    BusinessRep businessRep = context.BusinessReps
                        .Where(r => r.BusinessId == business.Id).FirstOrDefault();

                    if (businessRep == null)
                    {
                        /* If there is no business rep then the business will not have any contact person
                         * related inforamtion. */
                        businessTableView.StrContactPerson = "";
                        businessTableView.StrPhoneNumber = "";
                        businessTableView.StrFaxNumber = "";
                        businessTableView.StrEmailAddress = "";
                    }
                    else 
                    {
                        ContactPerson contactPerson = context.ContactPeople.Find(businessRep.ContactPersonId);

                        businessTableView.StrContactPerson = contactPerson?.Name;

                        /* Get the phone and fax number of the contact person. */
                        List<PhoneNumber> phoneNumbers = context.PhoneNumbers
                            .Where(pn => pn.ContactPersonId == contactPerson.Id).ToList();
                        foreach (PhoneNumber phoneNumber in phoneNumbers)
                        {
                            if (phoneNumber.GEnumPhoneType == PhoneType.Mobile ||
                                phoneNumber.GEnumPhoneType == PhoneType.Office)
                            {
                                businessTableView.StrPhoneNumber = phoneNumber?.Number;
                            }
                            else
                            {
                                businessTableView.StrFaxNumber = phoneNumber?.Number;
                            }
                        }

                        /* Get the contacts persons email address. */
                        Email email = context.Emails
                            .Where(ea => ea.ContactPersonId == contactPerson.Id).FirstOrDefault();
                        businessTableView.StrEmailAddress = email?.EmailAddress;
                    }

                    businessTableView.StrWebsite = business?.Website;
                    businessTableView.StrLevel = Business.GetMebershipLevelString(business.MembershipLevel);

                    businessTableView.IntEstablishedYear = CheckNullableInt(business?.YearEstablished);

                    lstTableViewModel.Add(businessTableView);
                }

                /* Create the columns for the table. */
                Type typeTableViewModel = typeof(BusinessTableViewModel);
                /*Create Data Columns*/
                foreach (var property in typeTableViewModel.GetProperties())
                {
                    GridViewColumn gvcCol = new GridViewColumn();
                    gvcCol.Header = GDicHumanReadableTableFields[property.Name];
                    gvcCol.DisplayMemberBinding = new Binding(property.Name);
                    gvMemberInfo.Columns.Add(gvcCol);
                }

                /* Add a button to edit the business to the end of each row. */
                var btnFactoryEditBusiness = new FrameworkElementFactory(typeof(Button));
                btnFactoryEditBusiness.SetValue(ContentProperty, "Edit");
                btnFactoryEditBusiness.SetValue(TemplateProperty, (ControlTemplate)Application.Current.Resources["smallButton"]);
                btnFactoryEditBusiness.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(BtnEditBusiness_Click));

                DataTemplate dtEdit = new DataTemplate() 
                { 
                    VisualTree = btnFactoryEditBusiness
                };

                GridViewColumn gvcEdit = new GridViewColumn
                {
                    CellTemplate = dtEdit,
                    Header = "Edit"
                };
                gvMemberInfo.Columns.Add(gvcEdit);
                /* Add a button to export the business to a PDF */
                var btnFactoryGenerateBusinessPDF = new FrameworkElementFactory(typeof(Button));
                btnFactoryGenerateBusinessPDF.SetValue(ContentProperty, "Create");
                btnFactoryGenerateBusinessPDF.SetValue(TemplateProperty, (ControlTemplate)Application.Current.Resources["smallButton"]);
                btnFactoryGenerateBusinessPDF.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(BtnCreMembPdf_Click));

                DataTemplate dtPDF = new DataTemplate()
                {
                    VisualTree = btnFactoryGenerateBusinessPDF
                };

                GridViewColumn gvcPDF = new GridViewColumn
                {
                    CellTemplate = dtPDF,
                    Header = "GeneratePDF"
                };
                gvMemberInfo.Columns.Add(gvcPDF);

                lvMemberInfo.ItemsSource = lstTableViewModel;

                /* Setup the list view filter. */
                CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(lvMemberInfo.ItemsSource);
                collectionView.Filter = LvMemberInfo_Filter;
            }
        }

        GridViewColumnHeader lastHeaderClicked = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;
        //Takes an input string, and direction
        //Sorts column with name matching the string in the given direction
        private void Sort(string strSortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvMemberInfo.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(strSortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        /// <summary>
        /// Calls the sort function when a column header is clicked passing it the column name and direction
        /// Maintains which column is currently being sorted and in which direction
        /// </summary>
        /// <param name="sender">The text box that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void GridViewColumnHeaderClickedHandler(object sender,
                                            RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var strSortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(strSortBy, direction);

                    lastHeaderClicked = headerClicked;
                    lastDirection = direction;
                }
            }
        }
        /// <summary>
        /// A method for detecting when the text in TxtFilter has changed.
        /// </summary>
        /// <param name="sender">The text box that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvMemberInfo.ItemsSource).Refresh();
        }

        /// <summary>
        /// A method for filtering across all the fields of lvMemberInfo based on the
        /// text entered in TxtFilter.
        /// </summary>
        /// <param name="item">The current item to check from the list view</param>
        /// <returns>A boolean indicated whether or not to display the item.</returns>
        private bool LvMemberInfo_Filter (object item)
        {
            bool boolColHasValue = false;

            if (String.IsNullOrEmpty(txtFilter.Text))
            {
                /* Show all the items in the list view since there is no filter. */
                return true;
            }
            else 
            {
                Type typeItem = typeof(BusinessTableViewModel);
                foreach (var property in typeItem.GetProperties()) 
                {
                    /* Skip over null fields so they don't interrupt the filtering. */
                    if (property.GetValue((item as BusinessTableViewModel), null) == null) 
                    {
                        continue;
                    }

                    /* Check each field in the list view and only display the rows that
                     * contain the entered text. */
                    if (property.GetValue((item as BusinessTableViewModel), null)
                        .ToString()
                        .IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0) 
                    {
                        boolColHasValue = true;
                    }
                }

                return boolColHasValue;
            }
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
        /// When the user clicks the 'Add New from PDF' button, they will have the ability to select a 
        /// New Membership Request PDF form that will then be parsed and added to the database automatically.
        /// This is intended to be a faster option that typing in new member details by hand.
        /// </summary>
        /// <param name="sender">The 'Add New from PDF' button</param>
        /// <param name="e">THe Click Event</param>
        private void BtnNewMembPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get fields from PDF then pass to Add Members Screen.
                Dictionary<string, string> dictFields = OpenFile();
                Business busToCheckExists = new Business();

                // Do some logic to find the business being modified.
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    busToCheckExists = dbContext.Businesses
                    .Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.CategoryRefs)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers)
                    .FirstOrDefault(business => business.BusinessName.Equals(dictFields["Business Name"]));
                }

                if (busToCheckExists == null)
                {
                    if (dictFields.ContainsKey("Business Name"))
                        NavigationService.Navigate(new ModifyMembersPage(dictFields, null, gObjConstContact));
                }
                else
                    MessageBox.Show($"{ dictFields["Business Name"] } already exists in the Database", "Business Already Exists");
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"This file is not compatible for import", "File Not Compatible");
            }
        }

        /// <summary>
        /// When the user click the 'Update from PDF' button, they will have the ability to select a 
        /// Update Membership Request PDF form that will be parsed for existing members and their data changes.
        /// Updated data is updated in the DB.
        /// </summary>
        /// <param name="sender">The 'Update from PDF' button</param>
        /// <param name="e">The Click Event</param>
        private void BtnModMembPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get fields from PDF
                Dictionary<string, string> dictFields = OpenFile();
                Business busModified = new Business();

                if (dictFields.ContainsKey("Business Name"))
                {
                    // Do some logic to find the business being modified.
                    using (DatabaseContext dbContext = new DatabaseContext())
                    {
                        busModified = dbContext.Businesses
                        .Include(x => x.MailingAddress)
                        .Include(x => x.PhysicalAddress)
                        .Include(x => x.CategoryRefs)
                        .ThenInclude(x => x.Category)
                        .Include(x => x.BusinessReps)
                        .ThenInclude(x => x.ContactPerson)
                        .ThenInclude(x => x.Emails)
                        .Include(x => x.BusinessReps)
                        .ThenInclude(x => x.ContactPerson)
                        .ThenInclude(x => x.PhoneNumbers)
                        .FirstOrDefault(business => business.BusinessName.Equals(dictFields["Business Name"]));
                    }

                    if (busModified != null)
                    {
                        if (dictFields.ContainsKey("Business Name"))
                            NavigationService.Navigate(new ModifyMembersPage(dictFields, busModified, gObjConstContact));
                    }
                    else
                        MessageBox.Show($"{ dictFields["Business Name"] } is not an existing Businss in the Database", "Business Not Found");
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"This file is not compatible for import", "File Not Compatible");
            }

        }
        /// <summary>
        /// When the user click the 'Create' button, the CreatePDF() function is called
        /// Creating a new pdf for the corresponding buisness
        /// </summary>
        /// <param name="sender">The 'Create' button</param>
        /// <param name="e">The Click Event</param>
        private void BtnCreMembPdf_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            BusinessTableViewModel selectedTableViewModel = btn.DataContext as BusinessTableViewModel;

            Business selectedBusiness = new Business();
            using (DatabaseContext context = new DatabaseContext())
            {
                selectedBusiness = context.Businesses
                    .Where(business => business.BusinessName.Equals(selectedTableViewModel.StrBuisnessName))
                    .FirstOrDefault();
            }
            CreatePDF(selectedBusiness);


        }

        /// <summary>
        /// Creates a new PDF from PDF Template
        /// 
        /// </summary>
        private void CreatePDF(Business bus)
        {

            //Path to MyDocuments folder
            var strExePath = AppDomain.CurrentDomain.BaseDirectory;


            string strTemplatePath = Directory.GetParent(strExePath) + "\\Resources\\general_membership_application_template.pdf";

            string strMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string strCleanName = bus.BusinessName.Replace(" ", "");
            char[] arrBad = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
            foreach(char cBad in arrBad)
            {
                strCleanName = strCleanName.Replace(Char.ToString(cBad), "_");
            }
            string strNewFile = String.Format(@"{0}\{1}.pdf", strMyDocumentsPath, strCleanName);
            //Path to Template file in myDocuments

            //Checks MyDocuments folder for template file
            if (!File.Exists(strTemplatePath))
            {
                MessageBox.Show("Template PDF Not Found.\n Put general_membership_application_template.pdf in your MyDocuments Folder");
            }
            else
            { 
                //create new reader object
                PdfReader pdfReader = new PdfReader(strTemplatePath);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(strNewFile, FileMode.Create));
                AcroFields pdfFormFields = pdfStamper.AcroFields;
                    using (DatabaseContext context = new DatabaseContext())
                    {
                        //variable for form fields in PDF 
                        //Membership Name
                        pdfFormFields.SetField("Business Name", bus.BusinessName);

                        //Logic for Contact Person name, Phone, and Fax
                        string strContact = "";
                        string strPhone = "";
                        string strFax = "";
                        string strEmail = "";

                        BusinessRep businessRep = context.BusinessReps
                            .Where(r => r.BusinessId == bus.Id).FirstOrDefault();
                        if (businessRep != null)
                        {
                            ContactPerson contactPerson = context.ContactPeople.Find(businessRep.ContactPersonId);
                            strContact = contactPerson?.Name;

                            //Logic to grab Phone number used in later section
                            List<PhoneNumber> phoneNumbers = context.PhoneNumbers
                                .Where(pn => pn.ContactPersonId == contactPerson.Id).ToList();
                            foreach (PhoneNumber phoneNumber in phoneNumbers)
                            {
                                if (phoneNumber.GEnumPhoneType == PhoneType.Mobile ||
                                    phoneNumber.GEnumPhoneType == PhoneType.Office)
                                {
                                    strPhone = phoneNumber?.Number;
                                }
                                else
                                {
                                    strFax = phoneNumber?.Number;
                                }
                            }
                            /* Get the contacts persons email address. */
                            Email email = context.Emails
                                .Where(ea => ea.ContactPersonId == contactPerson.Id).FirstOrDefault();
                            strEmail = email?.EmailAddress;
                        }

                        //Contact Person
                        pdfFormFields.SetField("Contact Name", strContact);
                        // Mailing Address
                        Address mailingAddress = context.Addresses.FirstOrDefault(x => x.Id == bus.MailingAddressId);
                        pdfFormFields.SetField("Mailing Address", mailingAddress?.StreetAddress);
                        // Location Address
                        Address locationAddress = context.Addresses.FirstOrDefault(x => x.Id == bus.PhysicalAddressId);
                        pdfFormFields.SetField("Location Address", locationAddress?.StreetAddress);
                        // City, State, Zip
                        string strCityStateZip = String.Format("{0} {1} {2}", mailingAddress?.City, mailingAddress?.State, CheckNullableInt(mailingAddress?.ZipCode));
                        pdfFormFields.SetField("CityStateZip", strCityStateZip);
                        // Phone Number
                        pdfFormFields.SetField("Phone Number", strPhone);
                        //Fax
                        pdfFormFields.SetField("Fax Number", strFax);
                        //Email Address
                        pdfFormFields.SetField("Email Address", strEmail);
                        // Website
                        pdfFormFields.SetField("Website", bus.Website);
                        //Year Established
                        pdfFormFields.SetField("Established", bus.YearEstablished.ToString());
                        /*Mebership Levels Radio Buttons
                        0 = Gold, 1 = Silver, 2 = Associate, 3 = Individual
                        */
                        string strLevel = "";
                        switch (bus.MembershipLevel.ToString())
                        {
                            case "GOLD":
                                strLevel = "Gold";
                                break;

                            case "SILVER":
                                strLevel = "Silver";
                                break;

                            case "ASSOCIATE":
                                strLevel = "Associate";
                                break;

                            default:
                                strLevel = "Individual";
                                break;
                        }
                        pdfFormFields.SetField("Level", strLevel);
                        /*Membership Payment Frequency Radio Buttons
                        0 = Bi Annual, 1 = Quarterly, 2 = Monthly
                            */
                        string strTermLength = "0";
                        string strIsAnnual = "Yes";
                        YearlyData yearlyData = context.BusinessYearlyData
                           .Where(r => r.BusinessId == bus.Id).FirstOrDefault();
                        string strTermLengthEnum = yearlyData?.TermLength.ToString();
                        switch (strTermLengthEnum)
                        {
                            case "Semiannually":
                                strTermLength = "0";
                                break;

                            case "Quarterly":
                                strTermLength = "1";
                                break;

                            case "Monthly":
                                strTermLength = "2";
                                break;

                            default:
                                strTermLength = "";
                                strIsAnnual = "";
                                break;
                        }
                        pdfFormFields.SetField("yearlyData", strTermLength);
                        // Non-Annual payment selected
                        pdfFormFields.SetField("NonAnnual", strIsAnnual);
                        pdfStamper.FormFlattening = false;
                        pdfStamper.Close();
                }
                string strArguments = $"/select, \"{strNewFile}\"";
                System.Diagnostics.Process.Start("explorer.exe", strArguments);
            }
        }
        /// <summary>
        /// Ability to open a file using the OpenFileDialog. 
        /// 
        /// . 
        /// </summary>
        private Dictionary<string, string> OpenFile()
        {
            //dictionary to store values to be sent to UI
            var dicToAdd = new Dictionary<string, string>();

            //opens file dialog to select pdf
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "PDF Files|*.pdf"
            };

            try
            {
                if (openFileDialog.ShowDialog() == true)
                    Console.WriteLine(File.ReadAllText(openFileDialog.FileName));

                //create new reader object
                PdfReader reader = new PdfReader(openFileDialog.FileName);
                //variable for form fields in PDF 
                AcroFields pdfFormFields = reader.AcroFields;
                //array to split city state zip 
                string strCityStateZip = pdfFormFields.GetField("CityStateZip");
                String[] strArrCityStateZip;
                //Logic to prevent errors if PDF is invalid
                if (strCityStateZip != null)
                {
                    strArrCityStateZip = strCityStateZip.ToString().Split(',');
                }
                else
                {
                    strArrCityStateZip = new string[0];
                }

                if (!pdfFormFields.Fields.ContainsKey("Business Name"))
                {
                    MessageBox.Show("This PDF is not compatible with the PDF Import Tool, please fill out the member details manually.", "Alert");
                }

                //dictionary add statements to add pdf data to ui
                dicToAdd.Add("Business Name", pdfFormFields.GetField("Business Name"));
                dicToAdd.Add("Website", pdfFormFields.GetField("Website"));
                dicToAdd.Add("Level", pdfFormFields.GetField("Level"));
                dicToAdd.Add("Established", pdfFormFields.GetField("Established"));

                // Mailing Address
                dicToAdd.Add("Mailing Address", pdfFormFields.GetField("Mailing Address"));

                if (strArrCityStateZip.Length > 0)
                    dicToAdd.Add("City", strArrCityStateZip[0]);
                else
                    dicToAdd.Add("City", "");

                if (strArrCityStateZip.Length > 1)
                    dicToAdd.Add("State", strArrCityStateZip[1].Replace(" ", ""));
                else
                    dicToAdd.Add("State", "");

                if (strArrCityStateZip.Length > 2)
                    dicToAdd.Add("Zip Code", strArrCityStateZip[2].Replace(" ", ""));
                else
                    dicToAdd.Add("Zip Code", "");

                // Location Address
                dicToAdd.Add("Location Address", pdfFormFields.GetField("Location Address"));

                if (strArrCityStateZip.Length > 0)
                    dicToAdd.Add("Location City", strArrCityStateZip[0]);
                else
                    dicToAdd.Add("Location City", "");

                if (strArrCityStateZip.Length > 1)
                    dicToAdd.Add("Location State", strArrCityStateZip[1].Replace(" ", ""));
                else
                    dicToAdd.Add("Location State", "");

                if (strArrCityStateZip.Length > 2)
                    dicToAdd.Add("Location Zip Code", strArrCityStateZip[2].Replace(" ", ""));
                else
                    dicToAdd.Add("Location Zip Code", "");

                dicToAdd.Add("Contact Name", pdfFormFields.GetField("Contact Name"));
                dicToAdd.Add("Phone Number", pdfFormFields.GetField("Phone Number"));
                dicToAdd.Add("Fax Number", pdfFormFields.GetField("Fax Number"));
                dicToAdd.Add("Email Address", pdfFormFields.GetField("Email Address"));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new Exception("File Not Compatible");
            }

            //dictionary return
            return dicToAdd;
        }
    }
}
