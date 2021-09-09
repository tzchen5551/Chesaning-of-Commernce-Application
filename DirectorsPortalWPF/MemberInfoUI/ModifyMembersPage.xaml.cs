using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using DirectorsPortalWPF.Controls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using DirectorsPortalConstantContact;

namespace DirectorsPortalWPF.MemberInfoUI
{
    /// <summary>
    /// Interaction logic for EditAddMembersPage.xaml
    /// </summary>
    public partial class ModifyMembersPage : Page
    {
        public List<int> GIntContactsToRemove { get; set; } = new List<int>();
        public List<string> CategoriesToRemove = new List<string>();
        private Business MSelectedBusiness = null;
        private int MIntContactCount = 0;
        private bool MBoolIgnoreWarnings = false;
        private ConstantContact gObjConstContact;
        private List<string> rgFailedCCEmailAdds;
        /// <summary>
        /// A method for initializing the modify members page. This method determines whether the page should be
        /// setup to add a new member or edit an existing one based on the passed in Business parameter.
        /// </summary>
        /// <param name="dictPdfImport">A dictionary used to populate the form with data pulled from a PDF.</param>
        /// <param name="selectedBusiness">
        /// The business selected from the list view that should be populated in the form. If this field is null
        /// the page will be setup to add a new member instead of editing an existing one.
        /// </param>
        public ModifyMembersPage([Optional] Dictionary<string, string> dictPdfImport, Business selectedBusiness, ConstantContact gObjConstContact)
        {
            InitializeComponent();

            MSelectedBusiness = selectedBusiness;
            rgFailedCCEmailAdds = new List<string>();
            CreateExtraFields();

            RefreshCategories();
            this.gObjConstContact = gObjConstContact;

            if (selectedBusiness != null)
            {
                /* A business was passed in so this page needs to update an already existing business and not
                 * add a new one.*/

                //Populate categories from database
                foreach (string currentItem in lbCategories.Items)
                {
                    foreach (CategoryRef cat in selectedBusiness.CategoryRefs)
                    {
                        if (currentItem.Equals(cat.Category.Category))
                        {
                            lbCategories.SelectedItems.Add(currentItem);
                            CategoriesToRemove.Add(currentItem);
                        }
                    }
                }
                lbCategories.UpdateLayout();

                lblHeader.Content = "Update Member";
                btnModifyMember.Content = "Update";
                btnModifyMember.Click += BtnUpdateMember_Click;

                using (DatabaseContext context = new DatabaseContext())
                {
                    try
                    {
                        /* Populate the business info from the selected business. */
                        txtBusinessName.Text = selectedBusiness.BusinessName;

                        if (dictPdfImport != null && !dictPdfImport["Established"].Equals(""))
                        {
                            txtYearEst.Text = dictPdfImport["Established"];
                            txtYearEst.Background = Brushes.Green;
                            txtYearEst.Foreground = Brushes.White;
                            txtYearEst.FontWeight = FontWeights.Bold;
                        }
                        else 
                            txtYearEst.Text = selectedBusiness.YearEstablished.ToString();

                        if (dictPdfImport != null && !dictPdfImport["Website"].Equals(""))
                        {
                            txtWebsite.Text = dictPdfImport["Website"];
                            txtWebsite.Background = Brushes.Green;
                            txtWebsite.Foreground = Brushes.White;
                            txtWebsite.FontWeight = FontWeights.Bold;
                        }
                        else
                            txtWebsite.Text = selectedBusiness.Website;

                        txtNotes.Text = selectedBusiness.ExtraNotes;

                        if (dictPdfImport != null && !dictPdfImport["Level"].Equals(""))
                        {
                            switch (dictPdfImport["Level"])
                            {
                                case "Gold":
                                    cboMemberLevel.SelectedIndex = 1;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;

                                case "Silver":
                                    cboMemberLevel.SelectedIndex = 2;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;

                                case "Associate":
                                    cboMemberLevel.SelectedIndex = 3;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;

                                case "Indiviual":
                                    cboMemberLevel.SelectedIndex = 4;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;

                                case "Courtesy":
                                    cboMemberLevel.SelectedIndex = 5;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;
                                default:
                                    cboMemberLevel.SelectedIndex = 0;
                                    cboMemberLevel.Foreground = Brushes.Green;
                                    cboMemberLevel.FontWeight = FontWeights.Bold;
                                    break;
                            }
                        }
                        else 
                            cboMemberLevel.SelectedIndex = (int)selectedBusiness.MembershipLevel;

                        /* Populate the addresses for the selected business. */
                        if (dictPdfImport != null && !dictPdfImport["Mailing Address"].Equals(""))
                        {
                            txtMailAddr.Text = dictPdfImport["Mailing Address"];
                            txtMailAddr.Background = Brushes.Green;
                            txtMailAddr.FontWeight = FontWeights.Bold;
                        }
                        else
                            txtMailAddr.Text = selectedBusiness.MailingAddress?.StreetAddress;

                        if (dictPdfImport != null && !dictPdfImport["City"].Equals(""))
                        {
                            txtMailCity.Text = dictPdfImport["City"];
                            txtMailCity.Background = Brushes.Green;
                            txtMailCity.FontWeight = FontWeights.Bold;
                        }
                        else
                            txtMailCity.Text = selectedBusiness.MailingAddress?.City;

                        if (dictPdfImport != null && !dictPdfImport["State"].Equals(""))
                        {
                            txtMailState.Text = dictPdfImport["State"];
                            txtMailState.Background = Brushes.Green;
                            txtMailState.FontWeight = FontWeights.Bold;
                        }
                        else
                            txtMailState.Text = selectedBusiness.MailingAddress?.State;

                        if (dictPdfImport != null && !dictPdfImport["Zip Code"].Equals(""))
                        {
                            txtMailZip.Text = dictPdfImport["Zip Code"];
                            txtMailZip.Background = Brushes.Green;
                            txtMailZip.FontWeight = FontWeights.Bold;
                        }
                        else
                            txtMailZip.Text = selectedBusiness.MailingAddress?.ZipCodeExt.ToString();

                        if (selectedBusiness.MailingAddressId == selectedBusiness.PhysicalAddressId && !(dictPdfImport != null && !dictPdfImport["Location Address"].Equals("")))
                        {
                            ChkLocationSameAsMailing.IsChecked = true;
                        }
                        else
                        {
                            if (dictPdfImport != null && !dictPdfImport["Location Address"].Equals(""))
                            {
                                txtLocationAddr.Text = dictPdfImport["Location Address"];
                                txtLocationAddr.Background = Brushes.Green;
                                txtLocationAddr.FontWeight = FontWeights.Bold;
                            }
                            else
                                txtLocationAddr.Text = selectedBusiness.PhysicalAddress?.StreetAddress;

                            if (dictPdfImport != null && !dictPdfImport["Location City"].Equals(""))
                            {
                                txtLocationCity.Text = dictPdfImport["Location City"];
                                txtLocationCity.Background = Brushes.Green;
                                txtLocationCity.FontWeight = FontWeights.Bold;
                            }
                            else
                                txtLocationCity.Text = selectedBusiness.PhysicalAddress?.City;

                            if (dictPdfImport != null && !dictPdfImport["Location State"].Equals(""))
                            {
                                txtLocationState.Text = dictPdfImport["Location State"];
                                txtLocationState.Background = Brushes.Green;
                                txtLocationState.FontWeight = FontWeights.Bold;
                            }
                            else
                                txtLocationState.Text = selectedBusiness.PhysicalAddress?.State;

                            if (dictPdfImport != null && !dictPdfImport["Location Zip Code"].Equals(""))
                            {
                                txtLocationZip.Text = dictPdfImport["Location Zip Code"];
                                txtLocationZip.Background = Brushes.Green;
                                txtLocationZip.FontWeight = FontWeights.Bold;
                            } 
                            else
                                txtLocationZip.Text = selectedBusiness.PhysicalAddress?.ZipCodeExt.ToString();
                        }

                        /* Populate the contacts for the selected business. */
                        if (selectedBusiness.BusinessReps != null
                            && selectedBusiness.BusinessReps.Count > 0)
                        {
                            /* Generate the UI elements for each contact. */
                            foreach (BusinessRep rep in selectedBusiness.BusinessReps)
                            {
                                MIntContactCount++;

                                ContactInput CiContact = new ContactInput
                                {
                                    GStrTitle = "Contact " + MIntContactCount + ":",
                                    GIntContactId = rep.ContactPersonId
                                };

                                if (dictPdfImport != null && !dictPdfImport["Contact Name"].Equals(""))
                                {
                                    CiContact.TxtName.Text = dictPdfImport["Contact Name"];
                                    CiContact.TxtName.Background = Brushes.Green;
                                    CiContact.TxtName.FontWeight = FontWeights.Bold;
                                }
                                else
                                    CiContact.TxtName.Text = rep.ContactPerson.Name;

                                /* Populate the emails for the contact. */
                                if (rep.ContactPerson.Emails != null
                                    && rep.ContactPerson.Emails.Count > 0)
                                {
                                    foreach (Email email in rep.ContactPerson.Emails)
                                    {
                                        CiContact.GntEmailCount++;

                                        EmailInput eiEmail = new EmailInput
                                        {
                                            GStrInputName = "Email " + CiContact.GntEmailCount + ":",
                                            GCiContactInputParent = CiContact,
                                            GIntEmailId = email.Id
                                        };

                                        if (dictPdfImport != null && !dictPdfImport["Email Address"].Equals("") && !email.EmailAddress.Equals(dictPdfImport["Email Address"]))
                                        {
                                            eiEmail.TxtEmail.Text = dictPdfImport["Email Address"];
                                            eiEmail.TxtEmail.Background = Brushes.Green;
                                            eiEmail.TxtEmail.FontWeight = FontWeights.Bold;
                                        }
                                        else
                                            eiEmail.TxtEmail.Text = email.EmailAddress;

                                        CiContact.SpContactEmails.Children.Add(eiEmail);
                                    }
                                }

                                /* populate the numbers for the contact. */
                                if (rep.ContactPerson.PhoneNumbers != null 
                                    && rep.ContactPerson.PhoneNumbers.Count > 0)
                                {
                                    foreach (PhoneNumber number in rep.ContactPerson.PhoneNumbers)
                                    {
                                        CiContact.GIntNumberCount++;

                                        ContactNumberInput cniNumber = new ContactNumberInput
                                        {
                                            GStrInputName = "Number " + CiContact.GIntNumberCount + ":",
                                            GCiContactInputParent = CiContact,
                                            GIntNumberId = number.Id
                                        };

                                        if (dictPdfImport != null && !dictPdfImport["Phone Number"].Equals("") && !number.Number.Equals(dictPdfImport["Phone Number"])
                                            && ((int)number.GEnumPhoneType) == 1)
                                        {
                                            cniNumber.TxtContactNumber.Text = dictPdfImport["Phone Number"];
                                            cniNumber.TxtContactNumber.Background = Brushes.Green;
                                            cniNumber.TxtContactNumber.FontWeight = FontWeights.Bold;
                                            cniNumber.CboNumberType.SelectedIndex = 1;
                                        }
                                        else
                                        if (dictPdfImport != null && !dictPdfImport["Fax Number"].Equals("") && !number.Number.Equals(dictPdfImport["Fax Number"]) 
                                            && ((int)number.GEnumPhoneType) == 2)
                                        {
                                            cniNumber.TxtContactNumber.Text = dictPdfImport["Fax Number"];
                                            cniNumber.TxtContactNumber.Background = Brushes.Green;
                                            cniNumber.TxtContactNumber.FontWeight = FontWeights.Bold;
                                            cniNumber.CboNumberType.SelectedIndex = 2;
                                        }
                                        else
                                        {
                                            cniNumber.TxtContactNumber.Text = number.Number;
                                            cniNumber.CboNumberType.SelectedIndex = (int)number.GEnumPhoneType;
                                            cniNumber.txtNumberNotes.Text = number.Notes;
                                        }

                                            CiContact.SpContactNumbers.Children.Add(cniNumber);
                                    }
                                }

                                SpContacts.Children.Add(CiContact);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            else 
            {
                /* No business was passed in so this page needs to be setup to add a new business. */
                lblHeader.Content = "Add Member";
                btnModifyMember.Content = "Add Member";
                btnModifyMember.Click += BtnAddMember_Click;

                if (dictPdfImport != null)
                {
                    txtBusinessName.Text = dictPdfImport["Business Name"];
                    txtYearEst.Text = dictPdfImport["Established"];
                    txtWebsite.Text = dictPdfImport["Website"];

                    switch (dictPdfImport["Level"])
                    {
                        case "Gold":
                            cboMemberLevel.SelectedIndex = 1;
                            break;

                        case "Silver":
                            cboMemberLevel.SelectedIndex = 2;
                            break;

                        case "Associate":
                            cboMemberLevel.SelectedIndex = 3;
                            break;

                        case "Indiviual":
                            cboMemberLevel.SelectedIndex = 4;
                            break;

                        case "Courtesy":
                            cboMemberLevel.SelectedIndex = 5;
                            break;
                        default:
                            cboMemberLevel.SelectedIndex = 0;
                            break;
                    }


                    txtMailAddr.Text = dictPdfImport["Mailing Address"];
                    txtMailCity.Text = dictPdfImport["City"];
                    txtMailState.Text = dictPdfImport["State"];
                    txtMailZip.Text = dictPdfImport["Zip Code"];

                    ChkLocationSameAsMailing.IsChecked = true;

                    MIntContactCount++;

                    ContactInput CiContact = new ContactInput
                    {
                        GStrTitle = "Contact " + MIntContactCount + ":"
                    };

                    if ((dictPdfImport["Contact Name"] != null && !dictPdfImport["Contact Name"].Equals("")) ||
                        (dictPdfImport["Email Address"] != null && !dictPdfImport["Email Address"].Equals("")) ||
                        (dictPdfImport["Phone Number"] != null && !dictPdfImport["Phone Number"].Equals("")) ||
                        (dictPdfImport["Fax Number"] != null && !dictPdfImport["Fax Number"].Equals("")))
                    {
                        SpContacts.Children.Add(CiContact);
                        CiContact.TxtName.Text = dictPdfImport["Contact Name"];
                    }

                    if (dictPdfImport["Email Address"] != null && !dictPdfImport["Email Address"].Equals(""))
                    {
                        CiContact.GntEmailCount++;

                        EmailInput eiEmail = new EmailInput
                        {
                            GStrInputName = "Email " + CiContact.GntEmailCount + ":",
                            GCiContactInputParent = CiContact,
                        };
                        eiEmail.TxtEmail.Text = dictPdfImport["Email Address"];

                        CiContact.SpContactEmails.Children.Add(eiEmail);
                    }

                    if (dictPdfImport["Phone Number"] != null && !dictPdfImport["Phone Number"].Equals(""))
                    {
                        CiContact.GIntNumberCount++;

                        ContactNumberInput cniNumber = new ContactNumberInput
                        {
                            GStrInputName = "Number " + CiContact.GIntNumberCount + ":",
                            GCiContactInputParent = CiContact,
                        };
                        cniNumber.TxtContactNumber.Text = dictPdfImport["Phone Number"];
                        cniNumber.CboNumberType.SelectedIndex = 1;

                        CiContact.SpContactNumbers.Children.Add(cniNumber);
                    }

                    if (dictPdfImport["Fax Number"] != null && !dictPdfImport["Fax Number"].Equals(""))
                    {
                        CiContact.GIntNumberCount++;

                        ContactNumberInput cniFax = new ContactNumberInput
                        {
                            GStrInputName = "Number " + CiContact.GIntNumberCount + ":",
                            GCiContactInputParent = CiContact,
                        };
                        cniFax.TxtContactNumber.Text = dictPdfImport["Fax Number"];
                        cniFax.CboNumberType.SelectedIndex = 2;

                        CiContact.SpContactNumbers.Children.Add(cniFax);
                    }
                }

                btnDelete.Visibility = Visibility.Hidden;
            }
        }

        private void RefreshCategories()
        {
            //Creating list to act as item source for lbCategories
            List<string> items = new List<string>();
            using (var context = new DatabaseContext())
            {

                List<Categories> allCategories = context.Categories.OrderBy(c=>c.Category).ToList();

                foreach (Categories currentCat in allCategories)
                {
                    items.Add(currentCat.Category);
                }

                lbCategories.ItemsSource = items;
            }

            if (MSelectedBusiness!= null)
            {
                /* A business was passed in so this page needs to update an already existing business and not
                 * add a new one.*/

                /*                List<string> items = new List<string>();
                                foreach (object i in selectedBusiness.Categories)
                                {
                                    items.Add((string)i);
                                }

                                lbCategories.ItemsSource = items;*/

                foreach (string currentItem in items)
                {
                    foreach (CategoryRef cat in MSelectedBusiness.CategoryRefs)
                    {
                        if (currentItem.Equals(cat.Category.Category))
                            lbCategories.SelectedItems.Add(currentItem);
                    }
                }
                lbCategories.UpdateLayout();
            }
        }

        /// <summary>
        /// A method for canceling the modify business action, taking the user back to the member info page.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MembersPage(gObjConstContact));
        }

        /// <summary>
        /// A method for adding a new business to the database from the information entered in the page.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            bool blnAddToCC = false;

            if (!ValidateDataInForm(MBoolIgnoreWarnings)) 
            {
                /* Return early since the form has invalid or missing data. */
                return;
            }

            using (DatabaseContext context = new DatabaseContext()) 
            {
                using (var transaction = context.Database.BeginTransaction()) 
                {
                    try
                    {
                        /* Get the business info from the form. */
                        Business newBusiness = new Business();
                        newBusiness.BusinessName = txtBusinessName.Text;

                        int.TryParse(txtYearEst.Text, out int intYearEst);
                        newBusiness.YearEstablished = intYearEst;

                        newBusiness.Website = txtWebsite.Text;
                        newBusiness.ExtraNotes = txtNotes.Text;
                        newBusiness.MembershipLevel = (MembershipLevel)cboMemberLevel.SelectedIndex;

                        /* Get all fo the extra fields from the form. */
                        foreach (UIElement uiExtraField in spExtraFields.Children) 
                        {
                            if (uiExtraField is ExtraField) 
                            {
                                ExtraField efField = (uiExtraField as ExtraField);
                                string strFieldName = efField.GStrFieldName.TrimEnd(':');
                                newBusiness.SetField(strFieldName, efField.TxtExtraField.Text, context);
                            }
                        }

                        /* Get the mailing address from the form. */
                        Address newMailingAddress = new Address();
                        newMailingAddress.StreetAddress = txtMailAddr.Text;
                        newMailingAddress.City = txtMailCity.Text;
                        newMailingAddress.State = txtMailState.Text;

                        int.TryParse(txtMailZip.Text.Split('-')[0], out int intMailZipCode);
                        newMailingAddress.ZipCode = intMailZipCode;
                        newMailingAddress.ZipCodeExt = txtMailZip.Text;

                        /* Get the location address from the form. */
                        Address newPhysicalAddress = new Address();
                        if (ChkLocationSameAsMailing.IsChecked == false)
                        {
                            newPhysicalAddress.StreetAddress = txtLocationAddr.Text;
                            newPhysicalAddress.City = txtLocationCity.Text;
                            newPhysicalAddress.State = txtLocationState.Text;

                            int.TryParse(txtLocationZip.Text.Split('-')[0], out int intLocationZipCode);
                            newPhysicalAddress.ZipCode = intLocationZipCode;
                            newPhysicalAddress.ZipCodeExt = txtLocationZip.Text;
                        }
                        else
                        {
                            newPhysicalAddress = newMailingAddress;
                        }

                        /* Check the addresses to make sure that they contain data. We don't want to add blank
                         * addresses to the database. */
                        if (!newMailingAddress.IsEmpty()) 
                        {
                            newBusiness.MailingAddress = newMailingAddress;
                        }
                        if (!newPhysicalAddress.IsEmpty()) 
                        {
                            newBusiness.PhysicalAddress = newPhysicalAddress;
                        }

                        /* Get the contacts from the form. */
                        newBusiness.BusinessReps = new List<BusinessRep>();
                        foreach (UIElement uiContact in SpContacts.Children)
                        {
                            if (uiContact is ContactInput)
                            {
                                ContactInput ciContact = (uiContact as ContactInput);
                                BusinessRep newBusinessRep = new BusinessRep();
                                ContactPerson newContact = new ContactPerson
                                {
                                    Name = ciContact.TxtName.Text,
                                    Emails = new List<Email>(),
                                    PhoneNumbers = new List<PhoneNumber>()
                                };

                                /* Get the contacts emails from the form. */
                                foreach (UIElement uiEmail in ciContact.SpContactEmails.Children)
                                {
                                    if (uiEmail is EmailInput)
                                    {
                                        EmailInput eiEmail = (uiEmail as EmailInput);
                                        Email newEmail = new Email();

                                        newEmail.EmailAddress = eiEmail.TxtEmail.Text;

                                        /* Make sure the email address is not blank. We don't want to add blank addresses
                                         * to the database.*/
                                        if (!newEmail.EmailAddress.Equals("")) 
                                        {
                                            newContact.Emails.Add(newEmail);
                                        }

                                        blnAddToCC = MessageBox.Show($"Would you like to add {ciContact.TxtName.Text} to your Constant Contact account?", "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                                        if (blnAddToCC)
                                        {
                                            gObjConstContact.Create(new Contact(eiEmail.TxtEmail.Text, ciContact.TxtName.Text, "")
                                                {
                                                    company_name = txtBusinessName.Text
                                                }
                                            );
                                            if (gObjConstContact.FindContactByEmail(eiEmail.TxtEmail.Text) == null)
                                                rgFailedCCEmailAdds.Add(eiEmail.TxtEmail.Text);
                                        }
                                    }
                                }

                                /* Get the contacts phone numbers from the form. */
                                foreach (UIElement uiPhoneNumber in ciContact.SpContactNumbers.Children)
                                {
                                    if (uiPhoneNumber is ContactNumberInput)
                                    {
                                        ContactNumberInput cniNumber = (uiPhoneNumber as ContactNumberInput);
                                        PhoneNumber newPhoneNumber = new PhoneNumber();

                                        newPhoneNumber.Number = cniNumber.TxtContactNumber.Text;
                                        newPhoneNumber.GEnumPhoneType = (PhoneType)cniNumber.CboNumberType.SelectedIndex;
                                        newPhoneNumber.Notes = cniNumber.txtNumberNotes.Text;

                                        /* Make sure the phone number is not blank. We don't want to add blank numbers
                                         * to the database.*/
                                        if (!newPhoneNumber.Number.Equals("")) 
                                        {
                                            newContact.PhoneNumbers.Add(newPhoneNumber);
                                        }
                                    }
                                }

                                newBusinessRep.ContactPerson = newContact;
                                newBusiness.BusinessReps.Add(newBusinessRep);

                            }
                        }

                        context.Businesses.Add(newBusiness);
                        context.SaveChanges();

                        foreach (string item in lbCategories.SelectedItems)
                        {
                            Categories cat = context.Categories.Where(x => x.Category.Equals(item)).FirstOrDefault();
                            Business forCategories = context.Businesses.Where(x => x.BusinessName.Equals(newBusiness.BusinessName)).FirstOrDefault();
                            CategoryRef catRef = new CategoryRef();
                            catRef.BusinessId = forCategories.Id;
                            catRef.CategoryId = cat.Id;
                            context.CategoryRef.Add(catRef);
                        }
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

            string strListOfFailures = "";
            foreach (string failedEmail in rgFailedCCEmailAdds)
            {
                strListOfFailures += failedEmail + ", ";
            }

            if (rgFailedCCEmailAdds.Count() > 0 && blnAddToCC)
                MessageBox.Show("The following Constant Contact Emails failed to add to Constant Contact: " 
                    + strListOfFailures.Substring(0, strListOfFailures.Length - 2).ToString() + 
                    ". Please try adding the emails manually to  Constant Contact", "Alert");

            NavigationService.Navigate(new MembersPage(gObjConstContact));
        }

        /// <summary>
        /// A method for updating an exisitng business (This business is held in MSelectedBusiness) from the
        /// information entered in the page.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnUpdateMember_Click(object sender, RoutedEventArgs e)
        {
            bool blnAddToCC = false;
            if (!ValidateDataInForm(MBoolIgnoreWarnings))
            {
                /* Return early since the form has invalid or missing data. */
                return;
            }

            using (DatabaseContext context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                        /* Update the business info from the form. */
                        MSelectedBusiness.BusinessName = txtBusinessName.Text;

                        int.TryParse(txtYearEst.Text, out int intYearEst);
                        MSelectedBusiness.YearEstablished = intYearEst;

                        MSelectedBusiness.Website = txtWebsite.Text;
                        MSelectedBusiness.ExtraNotes = txtNotes.Text;
                        MSelectedBusiness.MembershipLevel = (MembershipLevel)cboMemberLevel.SelectedIndex;

                        /* Update all of the extra fields from the form. */
                        foreach (UIElement uiExtraField in spExtraFields.Children)
                        {
                            if (uiExtraField is ExtraField)
                            {
                                ExtraField efField = (uiExtraField as ExtraField);
                                string strFieldName = efField.GStrFieldName.TrimEnd(':');
                                MSelectedBusiness.SetField(strFieldName, efField.TxtExtraField.Text, context);
                            }
                        }

                        /* Update the mailing address from the form. */
                        if (MSelectedBusiness.MailingAddress == null) 
                        {
                            MSelectedBusiness.MailingAddress = new Address();
                        }

                        MSelectedBusiness.MailingAddress.StreetAddress = txtMailAddr.Text;
                        MSelectedBusiness.MailingAddress.City = txtMailCity.Text;
                        MSelectedBusiness.MailingAddress.State = txtMailState.Text;

                        int.TryParse(txtMailZip.Text.Split('-')[0], out int intMailZipCode);
                        MSelectedBusiness.MailingAddress.ZipCode = intMailZipCode;
                        MSelectedBusiness.MailingAddress.ZipCodeExt = txtMailZip.Text;

                        if (MSelectedBusiness.MailingAddressId == MSelectedBusiness.PhysicalAddressId &&
                            ChkLocationSameAsMailing.IsChecked == false)
                        {
                            /* The mailing address was the same as the location address, but now we want to add
                             * a new location address. */
                            Address newLocationAddress = new Address();
                            newLocationAddress.StreetAddress = txtLocationAddr.Text;
                            newLocationAddress.City = txtLocationCity.Text;
                            newLocationAddress.State = txtLocationState.Text;

                            int.TryParse(txtLocationZip.Text.Split('-')[0], out int intLocationZipCode);
                            newLocationAddress.ZipCode = intLocationZipCode;
                            newLocationAddress.ZipCodeExt = txtLocationZip.Text;

                            if (!newLocationAddress.IsEmpty())
                            {
                                context.Addresses.Add(newLocationAddress);
                                MSelectedBusiness.PhysicalAddress = newLocationAddress;
                            }
                        }
                        else if (MSelectedBusiness.MailingAddressId != MSelectedBusiness.PhysicalAddressId &&
                            ChkLocationSameAsMailing.IsChecked == true)
                        {
                            /* The location address was removed, delete it and update the business location ID 
                             * to the mailing ID. */
                            context.Addresses.Remove(MSelectedBusiness.PhysicalAddress);

                            MSelectedBusiness.PhysicalAddress = MSelectedBusiness.MailingAddress;
                        }
                        else if (MSelectedBusiness.MailingAddressId != MSelectedBusiness.PhysicalAddressId &&
                            ChkLocationSameAsMailing.IsChecked == false)
                        {
                            /* Update the location address with the one from the form. */
                            MSelectedBusiness.PhysicalAddress.StreetAddress = txtLocationAddr.Text;
                            MSelectedBusiness.PhysicalAddress.City = txtLocationCity.Text;
                            MSelectedBusiness.PhysicalAddress.State = txtLocationState.Text;

                            int.TryParse(txtLocationZip.Text.Split('-')[0], out int intLocationZipCode);
                            MSelectedBusiness.PhysicalAddress.ZipCode = intLocationZipCode;
                            MSelectedBusiness.PhysicalAddress.ZipCodeExt = txtLocationZip.Text;
                        }

                        /* Update the contacts from the form. */
                        foreach (UIElement uiContact in SpContacts.Children)
                        {
                            if (uiContact is ContactInput)
                            {
                                ContactInput ciContact = uiContact as ContactInput;
                                BusinessRep currentRep = MSelectedBusiness.BusinessReps
                                    .FirstOrDefault(rep => rep.ContactPersonId == ciContact.GIntContactId);

                                /* Check if the contact was removed. */
                                if (GIntContactsToRemove.Contains(ciContact.GIntContactId))
                                {
                                    context.Remove(currentRep.ContactPerson);
                                    context.Remove(currentRep);
                                }
                                else
                                {
                                    /* Check if this is a new or updated contact. */
                                    if (ciContact.GIntContactId != -1)
                                    {
                                        currentRep.ContactPerson.Name = ciContact.TxtName.Text;

                                        /* Get the contact's emails from the form. */
                                        foreach (UIElement uiEmail in ciContact.SpContactEmails.Children)
                                        {
                                            if (uiEmail is EmailInput)
                                            {
                                                EmailInput eiEmail = uiEmail as EmailInput;
                                                Email currentEmail = currentRep.ContactPerson.Emails?
                                                    .FirstOrDefault(email => email.Id == eiEmail.GIntEmailId);

                                                if (ciContact.GIntEmailsToRemove.Contains(eiEmail.GIntEmailId))
                                                {
                                                    context.Remove(currentEmail);
                                                }
                                                else if (eiEmail.GIntEmailId == -1)
                                                {
                                                    Email newEmail = new Email();
                                                    newEmail.EmailAddress = eiEmail.TxtEmail.Text;

                                                    if (!newEmail.EmailAddress.Equals("")) 
                                                    {
                                                        context.Emails.Add(newEmail);

                                                        currentRep.ContactPerson.Emails.Add(newEmail);
                                                    }
                                                }
                                                else
                                                {
                                                    currentEmail.EmailAddress = eiEmail.TxtEmail.Text;
                                                }
                                            }
                                        }

                                        /* Get the contact's numbers from the form. */
                                        foreach (UIElement uiNumber in ciContact.SpContactNumbers.Children)
                                        {
                                            if (uiNumber is ContactNumberInput)
                                            {
                                                ContactNumberInput cniNumber = uiNumber as ContactNumberInput;
                                                PhoneNumber currentNumber = currentRep.ContactPerson.PhoneNumbers?
                                                    .FirstOrDefault(number => number.Id == cniNumber.GIntNumberId);

                                                if (ciContact.GIntNumbersToRemove.Contains(cniNumber.GIntNumberId))
                                                {
                                                    context.Remove(currentNumber);
                                                }
                                                else if (cniNumber.GIntNumberId == -1)
                                                {
                                                    PhoneNumber newPhoneNumber = new PhoneNumber();
                                                    newPhoneNumber.Number = cniNumber.TxtContactNumber.Text;
                                                    newPhoneNumber.Notes = cniNumber.txtNumberNotes.Text;
                                                    newPhoneNumber.GEnumPhoneType = (PhoneType)cniNumber.CboNumberType.SelectedIndex;

                                                    if (!newPhoneNumber.Number.Equals("")) 
                                                    {
                                                        context.PhoneNumbers.Add(newPhoneNumber);

                                                        currentRep.ContactPerson.PhoneNumbers.Add(newPhoneNumber);
                                                    }
                                                }
                                                else
                                                {
                                                    currentNumber.Number = cniNumber.TxtContactNumber.Text;
                                                    currentNumber.Notes = cniNumber.txtNumberNotes.Text;
                                                    currentNumber.GEnumPhoneType = (PhoneType)cniNumber.CboNumberType.SelectedIndex;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        /* This is an entirely new contact. Add it to the database and business. */
                                        BusinessRep newRep = new BusinessRep();
                                        ContactPerson newContact = new ContactPerson
                                        {
                                            Name = ciContact.TxtName.Text,
                                            Emails = new List<Email>(),
                                            PhoneNumbers = new List<PhoneNumber>()
                                        };

                                        newRep.ContactPerson = newContact;

                                        /* Get the contacts emails from the form. */
                                        foreach (UIElement uiEmail in ciContact.SpContactEmails.Children)
                                        {
                                            if (uiEmail is EmailInput)
                                            {
                                                EmailInput eiEmail = (uiEmail as EmailInput);
                                                Email newEmail = new Email();

                                                newEmail.EmailAddress = eiEmail.TxtEmail.Text;
                                                newRep.ContactPerson.Emails.Add(newEmail);

                                                blnAddToCC = MessageBox.Show($"Would you like to add {ciContact.TxtName.Text} to your Constant Contact account?", "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                                                if (blnAddToCC)
                                                {
                                                    gObjConstContact.Create(new Contact(eiEmail.TxtEmail.Text, ciContact.TxtName.Text, "")
                                                    {
                                                        company_name = txtBusinessName.Text
                                                    }
                                                );
                                                    if (gObjConstContact.FindContactByEmail(eiEmail.TxtEmail.Text) == null)
                                                        rgFailedCCEmailAdds.Add(eiEmail.TxtEmail.Text);
                                                }
                                            }
                                        }

                                        /* Get the contacts phone numbers from the form. */
                                        foreach (UIElement uiPhoneNumber in ciContact.SpContactNumbers.Children)
                                        {
                                            if (uiPhoneNumber is ContactNumberInput)
                                            {
                                                ContactNumberInput cniNumber = (uiPhoneNumber as ContactNumberInput);
                                                PhoneNumber newPhoneNumber = new PhoneNumber();

                                                newPhoneNumber.Number = cniNumber.TxtContactNumber.Text;
                                                newPhoneNumber.Notes = cniNumber.txtNumberNotes.Text;
                                                newPhoneNumber.GEnumPhoneType = (PhoneType)cniNumber.CboNumberType.SelectedIndex;
                                                newRep.ContactPerson.PhoneNumbers.Add(newPhoneNumber);
                                            }
                                        }
                                        context.BusinessReps.Add(newRep);
                                        MSelectedBusiness.BusinessReps.Add(newRep);
                                    }
                                }
                            }
                        }

                        context.Update(MSelectedBusiness);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            
            DeleteCategories();
            AddCategories();

            string strListOfFailures = "";
            foreach (string failedEmail in rgFailedCCEmailAdds)
            {
                strListOfFailures += failedEmail + ", ";
            }

            if (rgFailedCCEmailAdds.Count() > 0 && blnAddToCC)
                MessageBox.Show("The following Constant Contact Emails failed to add to Constant Contact: "
                    + strListOfFailures.Substring(0, strListOfFailures.Length - 2).ToString() +
                    ". Please try adding the emails manually to  Constant Contact", "Alert");

            NavigationService.Navigate(new MembersPage(gObjConstContact));
        }

        /// <summary>
        /// A mehtod for removing a business and all it's info from the database.
        /// This method is called by the delete button on the edit member screen.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string strMessage = "Are you sure you want to delete this memeber?\n"
                + "This action cannot be undone.\n\n"
                + "NOTE: A business with payment info cannot be deleted. Payment info must be removed from the Payment Info Screen.";

            MessageBoxResult messageBoxResult = MessageBox.Show(strMessage, "Delete Member", MessageBoxButton.YesNo);
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    /* Remove the member and return to the members page. */
                    using (DatabaseContext context = new DatabaseContext()) 
                    {
                        using (var transaction = context.Database.BeginTransaction()) 
                        {
                            try
                            {
                                /* Remove the businesses addresses. */
                                if (MSelectedBusiness.MailingAddress != null) 
                                {
                                    context.Remove(MSelectedBusiness.MailingAddress);
                                }
                                if (MSelectedBusiness.PhysicalAddress != null) 
                                {
                                    context.Remove(MSelectedBusiness.PhysicalAddress);
                                }

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

                    NavigationService.Navigate(new MembersPage(gObjConstContact));
                    break;

                case MessageBoxResult.No:
                    /* Do nothing */
                    break;
            }
        }

        /// <summary>
        /// A method for populating the form with the extra fields from the DB.
        /// </summary>
        private void CreateExtraFields()
        {
            using (DatabaseContext context = new DatabaseContext()) 
            {
                try
                {
                    Business business = new Business();
                    List<string> strExtraFields = business.AvailableFields(context);

                    foreach (string strField in strExtraFields)
                    {
                        ExtraField efField = new ExtraField
                        {
                            GStrFieldName = strField + ":"
                        };

                        if (MSelectedBusiness != null)
                        {
                            efField.TxtExtraField.Text = MSelectedBusiness?.GetField(strField);
                        }

                        spExtraFields.Children.Add(efField);
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void AddCategories()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                
                //updates database with selected categories for business
                foreach (string item in lbCategories.SelectedItems)
                {
                    if (!CategoriesToRemove.Contains(item))
                    {
                        Categories cat = context.Categories.Where(x => x.Category.Equals(item)).FirstOrDefault();
                        CategoryRef catRef = new CategoryRef();
                        catRef.BusinessId = MSelectedBusiness.Id;
                        catRef.CategoryId = cat.Id;
                        context.CategoryRef.Add(catRef);
                    }
                }
                
                context.SaveChanges();
                context.Dispose();
            }
        }
        
        private void DeleteCategories()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                foreach(string i in lbCategories.Items)
                {
                    Categories catRemove = context.Categories.Where(x => x.Category.Equals(i)).FirstOrDefault();
                    CategoryRef catRefRemove = context.CategoryRef.Where(x => x.BusinessId.Equals(MSelectedBusiness.Id)).Where(x => x.CategoryId.Equals(catRemove.Id)).FirstOrDefault();
                    if (CategoriesToRemove.Contains(i) && !lbCategories.SelectedItems.Contains(i))
                    {
                        context.CategoryRef.Remove(catRefRemove);
                    }
                }
                
                context.SaveChanges();
                context.Dispose();
            }
        }

        /// <summary>
        /// A method for validating the data in the form.
        /// This method will mark all required data that is missing or incorrectly formatted in the form with a
        /// RED border.
        /// This method will mark all missing, non-required data in the form witht a YELLOW border.
        /// </summary>
        /// <param name="boolIgnoreWarnings">
        /// A booling variable that is set when the user chooses to ignore warnings. When true,
        /// no warning message box will be shown to the user.
        /// </param>
        /// <returns>A boolean value. If true the form has error / is missing required data.</returns>
        private bool ValidateDataInForm(bool boolIgnoreWarnings) 
        {
            string strWarningMessage = "The following fields are missing data:";
            bool boolAllDataIsValid = true;
            bool boolShowWarning = false;

            /* Verify the business info. 
             * Just make sure then name isn't blank and if there is a est. year make sure that it is
             * only numbers. */
            if (txtBusinessName.Text.Equals(""))
            {
                boolAllDataIsValid = false;

                txtBusinessName.BorderBrush = Brushes.Red;

                ToolTip ttBusinessName = new ToolTip();
                ttBusinessName.Content = "Please enter a business name.";

                txtBusinessName.ToolTip = ttBusinessName;
            }
            else 
            {
                ResetFormError(txtBusinessName);
            }

            if (!txtYearEst.Text.Equals("")
                && !int.TryParse(txtYearEst.Text, out int intYearEst))
            {
                boolAllDataIsValid = false;

                txtYearEst.BorderBrush = Brushes.Red;

                ToolTip ttYearEst = new ToolTip();
                ttYearEst.Content = "Year established must be a number.";

                txtYearEst.ToolTip = ttYearEst;
            }
            else
            {
                ResetFormError(txtYearEst);
            }

            /* Verify the mailing address.
             * Make sure that none of the fields are blank and zip code is a number. */
            if (txtMailAddr.Text.Equals(""))
            {
                boolShowWarning = true;

                txtMailAddr.BorderBrush = Brushes.Yellow;

                strWarningMessage += "\nMailing Address";
            }
            else 
            {
                ResetFormError(txtMailAddr);
            }

            if (txtMailCity.Text.Equals(""))
            {
                boolShowWarning = true;

                txtMailCity.BorderBrush = Brushes.Yellow;

                strWarningMessage += "\nMailing City";
            }
            else
            {
                ResetFormError(txtMailCity);
            }

            if (txtMailState.Text.Equals(""))
            {
                boolShowWarning = true;

                txtMailState.BorderBrush = Brushes.Yellow;

                strWarningMessage += "\nMailing State";
            }
            else
            {
                ResetFormError(txtMailState);
            }

            if (!txtMailZip.Text.Equals("")
                && !int.TryParse(txtMailZip.Text.Split('-')[0], out int intMailZip))
            {
                boolAllDataIsValid = false;

                txtMailZip.BorderBrush = Brushes.Red;

                ToolTip ttMailZip = new ToolTip();
                ttMailZip.Content = "Zip code must be in the following format ##### or #####-####";

                txtMailZip.ToolTip = ttMailZip;
            }
            else if (txtMailZip.Text.Equals(""))
            {
                boolShowWarning = true;

                txtMailZip.BorderBrush = Brushes.Yellow;

                strWarningMessage += "\nMailing Zip";
            }
            else
            {
                ResetFormError(txtMailZip);
            }

            /* Verify the physical address.
             * Make sure that none of the fields are blank and zip code is a number. */
            if (ChkLocationSameAsMailing.IsChecked == false)
            {
                if (txtLocationAddr.Text.Equals(""))
                {
                    boolShowWarning = true;

                    txtLocationAddr.BorderBrush = Brushes.Yellow;

                    strWarningMessage += "\nLocation Address";
                }
                else
                {
                    ResetFormError(txtLocationAddr);
                }

                if (txtLocationCity.Text.Equals(""))
                {
                    boolShowWarning = true;

                    txtLocationCity.BorderBrush = Brushes.Yellow;

                    strWarningMessage += "\nLocation City";
                }
                else
                {
                    ResetFormError(txtLocationCity);
                }

                if (txtLocationState.Text.Equals(""))
                {
                    boolShowWarning = true;

                    txtLocationState.BorderBrush = Brushes.Yellow;

                    strWarningMessage += "\nLocation State";
                }
                else
                {
                    ResetFormError(txtLocationState);
                }

                if (!txtLocationZip.Text.Equals("")
                    && !int.TryParse(txtLocationZip.Text.Split('-')[0], out int intLocationZip))
                {
                    boolAllDataIsValid = false;

                    txtLocationZip.BorderBrush = Brushes.Red;

                    ToolTip ttLocationZip = new ToolTip();
                    ttLocationZip.Content = "Zip code must be in the following format ##### or #####-####";

                    txtLocationZip.ToolTip = ttLocationZip;
                }
                else if (txtLocationZip.Text.Equals(""))
                {
                    boolShowWarning = true;

                    txtLocationZip.BorderBrush = Brushes.Yellow;

                    strWarningMessage += "\nLocation Zip";
                }
                else
                {
                    ResetFormError(txtLocationZip);
                }
            }
            else 
            {
                ResetFormError(txtLocationAddr);
                ResetFormError(txtLocationCity);
                ResetFormError(txtLocationState);
                ResetFormError(txtLocationZip);
            }

            Regex regexEmail = new Regex(@"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b");
            Regex regexPhoneNumber = new Regex(@"^[0-9]\d{2}-\d{3}-\d{4}$");

            foreach (UIElement uiContact in SpContacts.Children)
            {
                if (uiContact is ContactInput)
                {
                    ContactInput contactInput = (uiContact as ContactInput);

                    if (contactInput.TxtName.Text.Equals(""))
                    {
                        boolAllDataIsValid = false;

                        contactInput.TxtName.BorderBrush = Brushes.Red;

                        ToolTip ttContactName = new ToolTip();
                        ttContactName.Content = "Contact name cannot be empty.";

                        contactInput.TxtName.ToolTip = ttContactName;
                    }
                    else 
                    {
                        ResetFormError(contactInput.TxtName);
                    }

                    /* Get the contacts emails from the form. */
                    foreach (UIElement uiEmail in contactInput.SpContactEmails.Children)
                    {
                        if (uiEmail is EmailInput)
                        {
                            EmailInput emailInput = (uiEmail as EmailInput);
                            string strEmail = emailInput.TxtEmail.Text.ToLower().Trim();

                            if (emailInput.TxtEmail.Text.Equals("")) 
                            {
                                boolShowWarning = true;

                                emailInput.TxtEmail.BorderBrush = Brushes.Yellow;

                                strWarningMessage += "\n" + contactInput.GStrTitle + " email";
                            }
                            else if (!regexEmail.IsMatch(strEmail))
                            {
                                boolAllDataIsValid = false;

                                emailInput.TxtEmail.BorderBrush = Brushes.Red;

                                ToolTip ttEmail = new ToolTip();
                                ttEmail.Content = "The entered address is not valid.";

                                emailInput.TxtEmail.ToolTip = ttEmail;
                            }
                            else
                            {
                                ResetFormError(emailInput.TxtEmail);
                            }
                        }
                    }

                    /* Get the contacts phone numbers from the form. */
                    foreach (UIElement uiPhoneNumber in contactInput.SpContactNumbers.Children)
                    {
                        if (uiPhoneNumber is ContactNumberInput)
                        {
                            ContactNumberInput contactNumberInput = (uiPhoneNumber as ContactNumberInput);
                            string strNumber = contactNumberInput.TxtContactNumber.Text.Trim();

                            if (contactNumberInput.TxtContactNumber.Text.Equals(""))
                            {
                                boolShowWarning = true;

                                contactNumberInput.TxtContactNumber.BorderBrush = Brushes.Yellow;

                                strWarningMessage += "\n" + contactInput.GStrTitle + " phone number";
                            }
                            else if (!regexPhoneNumber.IsMatch(strNumber))
                            {
                                boolAllDataIsValid = false;

                                contactNumberInput.TxtContactNumber.BorderBrush = Brushes.Red;

                                ToolTip ttNumber = new ToolTip();
                                ttNumber.Content = "The entered phone number was not of the form xxx-xxx-xxxx.";

                                contactNumberInput.TxtContactNumber.ToolTip = ttNumber;
                            }
                            else 
                            {
                                ResetFormError(contactNumberInput.TxtContactNumber);
                            }
                        }
                    }
                }
            }

            if (boolShowWarning && !boolIgnoreWarnings) 
            {
                /* Warn the user there are blank fields that will not cause errors. */
                boolAllDataIsValid = false;
                CreateWarningMessageBox(strWarningMessage);
            }

            return boolAllDataIsValid;
        }

        /// <summary>
        /// This method resets a giving text box that was marked for an error or warning
        /// back to its default state.
        /// </summary>
        /// <param name="txtError">The textbox in the form to reset.</param>
        private void ResetFormError(TextBox txtError) 
        {
            txtError.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            txtError.ToolTip = null;
        }

        /// <summary>
        /// This message creates the warning message box to notify the user when some fields have been left
        /// blank.
        /// 
        /// When YES is selected in the message box the form will be resubmitted and warnings will be ignored. The resubmit
        /// will still fail if required data is missing or incrorrectly formatted.
        /// 
        /// When NO is selected the user is returned to the form and the form is not resubmitted.
        /// </summary>
        /// <param name="strMessage">
        /// The message to display in the message box. This message should state the fields in the form
        /// that generated the warnings.
        /// </param>
        private void CreateWarningMessageBox(string strMessage) 
        {
            strMessage += "\n\nWould you like to add the data anyways?";

            MessageBoxResult messageBoxResult = MessageBox.Show(strMessage, "Missing Data", MessageBoxButton.YesNo);
            switch (messageBoxResult) 
            {
                case MessageBoxResult.Yes:
                    /* Add the entered business */
                    MBoolIgnoreWarnings = true;
                    if (MSelectedBusiness != null)
                    {
                        BtnUpdateMember_Click(messageBoxResult, null);
                    }
                    else 
                    {
                        BtnAddMember_Click(messageBoxResult, null);
                    }
                    break;

                case MessageBoxResult.No:
                    /* Do nothing */
                    break;
            }
        }

        /// <summary>
        /// A method for adding a new contact input control to the SpContacts stack panel on
        /// the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnAddContact_Click(object sender, RoutedEventArgs e)
        {
            MIntContactCount++;

            ContactInput CiContact = new ContactInput
            {
                GStrTitle = "Contact " + MIntContactCount + ":"
            };

            
            if (!gObjConstContact.SignedIn && !gObjConstContact.PopupAsked)
            {
                if (MessageBox.Show("You are not currently signed into Constant Contact. If you do not sign in, any new members will not be added to Constant Contact. Would you like to Sign in now?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    gObjConstContact.ValidateAuth();
                }
                gObjConstContact.PopupAsked = true;
            }
            SpContacts.Children.Add(CiContact);
        }

        /// <summary>
        /// A method for disabling the SpLocation address stack panel when the user checks the
        /// ChkLocationSameAsMailing checkbox on the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void ChkLocationSameAsMailing_Checked(object sender, RoutedEventArgs e)
        {
            SpLocationAddress.IsEnabled = false;
        }

        /// <summary>
        /// A method for enabling the SpLocation address stack panel when the user unchecks the
        /// ChkLocationSameAsMailing checkbox on the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void ChkLocationSameAsMailing_Unchecked(object sender, RoutedEventArgs e)
        {
            SpLocationAddress.IsEnabled = true;
        }

        /// <summary>
        /// A method for adding a category to the category options.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            // Don't add a category if the user didn't fill out the text bod
            if (string.IsNullOrEmpty(txtAddCategory.Text))
            {
                MessageBox.Show("The added category cannot be empty.");
                return;
            }

            using (var context = new DatabaseContext())
            {
                // Don't add this label if it already exists
                if (context.Categories.Any(category => category.Category == txtAddCategory.Text))
                {
                    MessageBox.Show($"The category {txtAddCategory.Text} already exists.");
                    return;
                }

                // Add the new category to the DB
                Categories objNewCategory = new Categories
                {
                    Category = txtAddCategory.Text
                };
                context.Categories.Add(objNewCategory);
                context.SaveChanges();

                // Update the UI
                RefreshCategories();
                txtAddCategory.Clear();
            }
        }

        /// <summary>
        /// Gets called when user clicks into from Add Category textbox.
        /// Shows the popup used to display explaination label
        /// </summary>
        /// <param name="sender">The Add Category textbox object that has called the function.</param>
        /// <param name="e">The lost focus event</param>
        private void ShowPopup(object sender, RoutedEventArgs e)
        {
            popNewCategory.IsOpen = true;
        }

        /// <summary>
        /// Gets called when user clicks away from Add Category textbox.
        /// Hides the popup used to display explaination label
        /// </summary>
        /// <param name="sender">The Add Category textbox object that has called the function.</param>
        /// <param name="e">The lost focus event</param>
        private void HidePopup(object sender, RoutedEventArgs e)
        {
            popNewCategory.IsOpen = false;
        }
    }
}
