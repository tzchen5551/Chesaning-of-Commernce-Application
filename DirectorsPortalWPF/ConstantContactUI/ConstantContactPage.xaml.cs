using DirectorsPortalConstantContact;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// File Purpose:
///     This file defines the logic for the Constant Contact screen. Functionality includes displaying
///     email group and email campaings as well as interfacing with the Constant Contact service.
///         
/// </summary>
namespace DirectorsPortalWPF.ConstantContactUI
{
    /// <summary>
    /// Interaction logic for ConstantContactPage.xaml
    /// </summary>
    public partial class ConstantContactPage : Page
    {
        // The object containing all data for the user of a Constant Contact account. 
        private ConstantContact gObjConstContact;
        private EmailCampaignActivity gObjCurrentlySelectedActivity;
        /// <summary>
        /// Initialization of the Constant Contact screen. Renders the Side menus for Contact Lists, 
        /// Email Campaigns and a preview of a selected Email Campaign Activity.
        /// </summary>
        public ConstantContactPage(ConstantContact ccHelper)
        {
            InitializeComponent();
            gObjConstContact = ccHelper;
            //Constant Contact Dev Account
            //Username: benjamin.j.dore@icloud.com
            //password: ayC&Aybab6sC422
            //
            // yes this is intentional, this is an accoutn we can all use for dev


            //ConstantContact CC = new ConstantContact();
            //CC.gobjCCAuth.ValidateAuthentication();

            //MessageBox.Show(CC.gdctEmailCampaigns.ElementAt(i).Value.Activities.First().permalink_url);

            //LoadEmailCampaigns(ccHelper);
            LoadContactLists(ccHelper);

            /*Update Contact
            Contact c = CC.FindContactByEmail("aamodt@example.com");
            c.company_name = "walmart";
            Console.WriteLine(c.contact_id);
            CC.Update(c);
            */

            /*update list 
            ContactList cl = CC.FindListByName("usable");
            cl.name = "Usable List";
            CC.Update(cl);
            */

            //add campaign
            //CC.AddCampaign();

            ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, this));

        }

        /// <summary>
        /// Pulls the list of email groups.This function allows for a dynamic load
        /// of the emails groups to the 'email groups' sidepane in the Constant Contact screen.
        /// </summary>
        public void LoadContactLists(ConstantContact ccHelper)
        {
            // Pull the email list element from the page
            object nodContactList = FindName("ContactList");
            // Ensure that the email list element is a stack panel
            if (nodContactList is StackPanel)
            {

                // Cast email list to stack panel so it can be used
                StackPanel vspContactList = nodContactList as StackPanel;
                vspContactList.Children.Clear();
                // TODO: This should be retrieved from an API from SDK team or database team
                //string[] rgstrGroups = { "Gold Members", "Silver Members", "Restaurants" };

                foreach (var ccEmailGroup in ccHelper.gdctContactLists)
                {
                    // For every email group found in the database, create a row
                    // in the email groups list with label and an edit button
                    StackPanel hspEmailGroupRow = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };

                    Label lblEmailGroupName;
                    if (ccEmailGroup.Value.name.Length > 12)
                    {
                        lblEmailGroupName = new Label()
                        {
                            Content = ccEmailGroup.Value.name.Substring(0, 12) + "..."
                        };
                    }
                    else
                    {
                        lblEmailGroupName = new Label()
                        {
                            Content = ccEmailGroup.Value.name
                        };
                    }

                    
                    Button btnEmailGroupEditButton = new Button()
                    {
                        Content = "Edit",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Template = (ControlTemplate)Application.Current.Resources["smallButton"],
                        Padding = new Thickness(0, 0, 35, 0),
                        Margin = new Thickness(5, 0, 0, 0),
                        Height = 15
                    };
                    btnEmailGroupEditButton.Click += (s, e) =>
                    {
                        /// <summary>
                        /// Navigates to the EditGroups screen and passes the corresponding group name
                        /// </summary>
                        /// <param name="sender">The 'Edit' Button</param>
                        /// <param name="e">The Click Event</param>
                        if (!gObjConstContact.SignedIn)
                            MessageBox.Show("Please make sure that you are logged in before making changes.", "Alert");
                        else if (gObjConstContact.Updating)
                            MessageBox.Show("Please wait for update to finish before making changes", "Alert");
                        else
                            ContactListFrame.Navigate(new EditContactListUI.EditContactListPage(ccHelper, ccEmailGroup.Value.name, ContactListFrame, this));
                    };
                    hspEmailGroupRow.Children.Add(btnEmailGroupEditButton);
                    hspEmailGroupRow.Children.Add(lblEmailGroupName);
                    vspContactList.Children.Add(hspEmailGroupRow);
                    
                }
            }
        }

        /// <summary>
        /// Creates a background worker on a separate thread (Upon clicking the 'Refresh' button) that does all the constant contact
        /// authentication. The Background worker is necessary to preventing the user interface from locking up
        /// while authentication is occuring. Once Constant Contact authentication is complete, the user interface is updated with data
        /// from the Constant Contact API.
        /// </summary>
        /// <param name="sender">The 'Refresh' button on the Constant Contact screen</param>
        /// <param name="e">The click event</param>
        private void RefreshConstantContact_Click(object sender, RoutedEventArgs e)
        {
            if (!gObjConstContact.Updating)
            {
                BackgroundWorker bWrk = new BackgroundWorker();
                
                btnRefreshConstantContact.Content = "Refreshing...";
                btnRefreshConstantContact.Width = 100;

                bWrk.DoWork += AuthenticateConstantContact;
                bWrk.RunWorkerCompleted += LoadConstantContactData;

                bWrk.RunWorkerAsync();

                ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, this));
            }
            else
            {
                MessageBox.Show("We are currently updating your Constant Contact data, please wait until we are finished to refresh again.", "Alert");
            }
            

        }

        /// <summary>
        /// To be used by the background worker when refreshing the Constant Contact page. Loads the data from the
        /// Constant Contact API into the UI
        /// </summary>
        /// <param name="sender">The background worker.</param>
        /// <param name="e">The arguments for when the worker completes work</param>
        private void LoadConstantContactData(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadContactLists(gObjConstContact);
            //LoadEmailCampaigns(gObjConstContact);

            btnRefreshConstantContact.Content = "Refresh";
            btnRefreshConstantContact.Width = 60;
            //txtEmailGroups.Text = "";

        }

        /// <summary>
        /// To be used by the background worker when refreshing the Constant Contact page. Authenticates the user to
        /// Constant Contact.
        /// </summary>
        /// <param name="sender">The background worker</param>
        /// <param name="e">The arguments for the 'DoWork' event</param>
        private void AuthenticateConstantContact(object sender, DoWorkEventArgs e)
        {
            gObjConstContact.RefreshData();
        }

        
        /// <summary>
        /// Logs out of the constant contact service, if logged in.
        /// </summary>
        /// <param name="sender">The "logout" button</param>
        /// <param name="e">The click event</param>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            gObjConstContact.LogOut();
            MessageBox.Show("You are now logged out", "Alert");
            LoadContactLists(gObjConstContact);
            ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, this));
        }

        /// <summary>
        /// Opens a pop-up window that displays the current frames help information. 
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">The Click event</param>
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpUI.HelpScreenWindow helpWindow = new HelpUI.HelpScreenWindow();
            helpWindow.Show();
            helpWindow.tabs.SelectedIndex = 3;
        }
    }
}
