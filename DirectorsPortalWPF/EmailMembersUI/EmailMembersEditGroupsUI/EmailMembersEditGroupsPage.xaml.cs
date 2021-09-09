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
using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using DirectorsPortalWPF.EmailMembersSendEmailUI;
using DirectorsPortalWPF.EmailMembersUI;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// This file has all of the logic for editing existing Email Groups
/// This page allows the director to edit all of the
/// different email groups without leaving the portal to use a service
/// like ConstantContact. 
/// </summary>
namespace DirectorsPortalWPF.EmailMembersEditGroupsUI
{
    /// <summary>
    /// Interaction logic for EmailMembersEditGroupsUI.xaml
    /// </summary>
    public partial class EmailMembersEditGroupsPage : Page
    {
        DatabaseContext dbContext = new DatabaseContext();
        List<Business> rgRemoveFromGroup = new List<Business>();
        List<Business> rgAddToGroup = new List<Business>();
        List<Business> rgBusinessesOfGroup;
        EmailPage objEmailPage;
        EmailGroup selectedEmailGroup;
        EmailMembersSendEmailPage objSendPage;

        public EmailMembersEditGroupsPage(EmailGroup objGroup, EmailPage emailPage, EmailMembersSendEmailPage sendPage)
        {
            InitializeComponent();
            LoadGroupData(objGroup);
            selectedEmailGroup = objGroup;
            objSendPage = sendPage;
            this.objEmailPage = emailPage;
        }
        /// <summary>
        /// Pulls the group name, list of members, and  notes
        /// of the selected group. Then displays them in the txtNotes,
        /// txtGroupsNames, and txtGroupMembers text boxes.
        /// </summary>
        public void LoadGroupData(EmailGroup objGroup)
        {
            int intGroupNameId = objGroup.Id;
            List<EmailGroupMember> rgEmailGroupMembers;
            List<Email> rgEmails = new List<Email>();

            //pull group name
            txtGroupName.Text = objGroup.GroupName;
            //get all associated Email Id's
            rgEmailGroupMembers = dbContext.EmailGroupMembers.Where(x => intGroupNameId == x.GroupId).ToList();
            foreach(EmailGroupMember emailGroupMembers in rgEmailGroupMembers)
            {
                rgEmails.AddRange(dbContext.Emails.Where(x => x.Id == emailGroupMembers.EmailId)
                .Include(e => e.ContactPerson)
                .ThenInclude(cp => cp.BusinessReps)
                .ThenInclude(br => br.Business)
                .ToList());
            }

            //pull buisnesses
            List<Business> rgBusinesses = dbContext.Businesses.Include(x => x.BusinessReps).ThenInclude(x => x.ContactPerson).ToList();
            rgBusinessesOfGroup = new List<Business>();
            foreach (Email email in rgEmails)
            {
                // Hi travis, like the commenting?
                //rgBuisnessesOfGroup.AddRange(rgBusinesses.Where(x => x.BusinessReps[0].ContactPerson.Emails[0] == emails).ToList());
                // rgBuisnessesOfGroup.AddRange(
                //     rgBusinesses.Where(
                //         x => x.BusinessReps
                //         .Select(br => br.ContactPerson.Emails)
                //         .Any(cp => cp.Any(em => em == emails))));
                // rgBusinessesOfGroup.AddRange(
                //     dbContext.Businesses
                //     .Include(b => b.BusinessReps)
                //     .ThenInclude(br => br.ContactPerson)
                //     .ThenInclude(cp => cp.Emails)
                //     .Where(b => b.BusinessReps
                //                 .Select(br => br.ContactPerson)
                //                 .Select(cp => cp.Emails)
                //                 .Any(cp => cp.Any(em => em == emails))));
                rgBusinessesOfGroup.AddRange(email.ContactPerson.BusinessReps.Select(cp => cp.Business));
            }
            lstGroupMembers.ItemsSource = rgBusinessesOfGroup;

            //pull notes
            txtNotes.Text = objGroup.Notes;
            }

        /// <summary>
        /// Gets called on the click of the "Save" button on the edit page.
        /// Will save changes made to an existing Group.
        /// Then return user to the SendEmailPage
        /// </summary>
        /// <param name="sender">The Save button object that has called the function.</param>
        /// <param name="e">The button press event</param>
        private void Save_Group(object sender, RoutedEventArgs e)
        {
            //string strGroupName = txtGroupName.Text;
            //List<Business> businesses = new List<Business>();
            //string strNotes = txtNotes.Text;
            //using (var context = new DatabaseContext())
            //{
            //    foreach (Business groupMember in lstGroupMembers.Items)
            //    {
            //        //Business b = context.Businesses.FirstOrDefault(x => x.Id == groupMember.Id);
            //        //b.BusinessName = "New Business Name";
            //        //Business b = context.Businesses.Include(x => x.email);
            //        //context.SaveChanges();

            //    }
            //}

            //this.objEmailPage.LoadEmailGroups();
            //// TODO: Link with database once implemented
            //this.NavigationService.Navigate(new EmailMembersSendEmailUI.EmailMembersSendEmailPage());

            // TODO: Still needs to be implemented
            string strGroupName = txtGroupName.Text;
            //List<Business> businesses = new List<Business>();

            if (!txtGroupName.Text.Equals(""))
            {
                string strNotes = txtNotes.Text;
                int intEmailGroupId;
                EmailGroup emailGroup = dbContext.EmailGroups.Where(x => x.GroupName.Equals(selectedEmailGroup.GroupName)).FirstOrDefault();
                //EmailGroup emailGroup = new EmailGroup();
                //emailGroup.GroupName = strGroupName;
                //emailGroup.Notes = strNotes;
                //dbContext.Add(emailGroup);
                //dbContext.SaveChanges();
                emailGroup.GroupName = strGroupName;
                emailGroup.Notes = strNotes;
                // List<EmailGroup> eg = dbContext.EmailGroups.Where(x => strGroupName == x.GroupName).ToList();

                intEmailGroupId = selectedEmailGroup.Id;
                {
                    List<EmailGroupMember> objEmailGroupMems = dbContext.EmailGroupMembers.Where(x => x.GroupId == selectedEmailGroup.Id).ToList();
                    foreach (Business groupMember in rgRemoveFromGroup)
                    {
                        List<BusinessRep> br = dbContext.BusinessReps.Where(x => groupMember.Id == x.BusinessId).Include(x => x.ContactPerson).ThenInclude(x => x.Emails).ToList();
                        int intEmailId = br[0].ContactPerson.Emails[0].Id;

                        dbContext.Remove(objEmailGroupMems.Find(g => g.EmailId == intEmailId));
                        dbContext.SaveChanges();
                    }

                    foreach (Business groupMember in rgAddToGroup)
                    {
                        Business b = dbContext.Businesses.FirstOrDefault(x => x.Id == groupMember.Id);
                        List<BusinessRep> br = dbContext.BusinessReps.Where(x => b.Id == x.BusinessId).Include(x => x.ContactPerson).ThenInclude(x => x.Emails).ToList();
                        int intEmailId = br[0].ContactPerson.Emails[0].Id;

                        EmailGroupMember emailGroupMember = new EmailGroupMember();
                        emailGroupMember.GroupId = intEmailGroupId;
                        emailGroupMember.EmailId = intEmailId;

                        dbContext.Add(emailGroupMember);
                        dbContext.SaveChanges();
                    }
                }

                selectedEmailGroup.GroupName = txtGroupName.Text;
                selectedEmailGroup.Notes = txtNotes.Text;
                dbContext.SaveChanges();


                this.objEmailPage.LoadEmailGroups();
                // TODO: Link with database once implemented
                this.NavigationService.Navigate(objSendPage);
            }
            else
            {
                MessageBox.Show("Please enter a Email Group Name", "Alert");
            }
        }

        /// <summary>
        /// Gets called on the click of the "Remove Member" button on the edit page.
        /// Will remove member from listbox
        /// </summary>
        /// <param name="sender">The Save button object that has called the function.</param>
        /// <param name="e">The button press event</param>
        private void Remove_Member(object sender, RoutedEventArgs e)
        {
            rgRemoveFromGroup.Add((Business)lstGroupMembers.SelectedItem);
            // lstGroupMembers.Items.Remove(lstGroupMembers.SelectedItem);
            rgBusinessesOfGroup.Remove((Business)lstGroupMembers.SelectedItem);
            lstGroupMembers.Items.Refresh();
            
        }

        /// <summary>
        /// Gets called on the click of the "Cancel" button on the edit page.
        /// Will return the user to the SendEmailPage
        /// </summary>
        /// <param name="sender">The Cancel button object that has called the function.</param>
        /// <param name="e">The button press event</param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(objSendPage);
        }

        /// <summary>
        /// Gets called when user types in Add Member To Group textbox.
        /// Querys the database for data matching entered text
        /// </summary>
        /// <param name="sender">The Add Member to Group textbox object that has called the function.</param>
        /// <param name="e">The text changed event</param>
        private void Search_Database(object sender, TextChangedEventArgs e)
        {
            lstPopup.Items.Clear();
            string strSearchTerm = txtAddGroupMembers.Text;
            Boolean boolExistsInGroup = false;
            List<BusinessRep> rgBusinessRepsWithValidEmails = null;

            popSearch.IsOpen = true;
            using (var context = new DatabaseContext())
            {
                List<Business> queryBusinesses = context.Businesses.Include(x => x.BusinessReps)
                    .ThenInclude(y => y.ContactPerson)
                    .ThenInclude(z => z.Emails)
                    .Where(
                    b => b.BusinessName.ToLower().Contains(strSearchTerm.ToLower())
                ).ToList();
                foreach (Business business in queryBusinesses)
                {
                    boolExistsInGroup = false;
                    foreach (Business groupMember in lstGroupMembers.Items)
                        if (groupMember.Id == business.Id)
                        {
                            boolExistsInGroup = true;
                            break;
                        }
                    if (!boolExistsInGroup)     // If the business is not already selected...
                    {
                        try
                        {
                            rgBusinessRepsWithValidEmails = business.BusinessReps
                                .FindAll(x => x.ContactPerson?.Emails?.Count() > 0)                                             // Chech there are email addresses under a business
                                .FindAll(z => z.ContactPerson?.Emails?.FindAll(l => !l.EmailAddress.Equals("")).Count() > 0);   // Make sure the emails are not empty strings
                            if (rgBusinessRepsWithValidEmails != null & rgBusinessRepsWithValidEmails.Count() > 0)
                                lstPopup.Items.Add(business);
                        }
                        catch (NullReferenceException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets called when user clicks away from Add Member To Group textbox.
        /// Hides the popup used to display search results
        /// </summary>
        /// <param name="sender">The Add Member to Group textbox object that has called the function.</param>
        /// <param name="e">The lost focus event</param>
        private void Hide_Search(object sender, RoutedEventArgs e)
        {
            popSearch.IsOpen = false;
        }

        /// <summary>
        /// Gets called when user selects an item from the popup.
        /// Displays the item in the group members list box
        /// </summary>
        /// <param name="sender">The popup object that has called the function.</param>
        /// <param name="e">The selection changed event</param>
        private void Add_Member_To_Group(object sender, SelectionChangedEventArgs e)
        {
            if (lstPopup.SelectedIndex >= 0)
            {
                // lstGroupMembers.Items.Add(lstPopup.SelectedItem);
                rgBusinessesOfGroup.Add((Business)lstPopup.SelectedItem);
                rgAddToGroup.Add((Business)lstPopup.SelectedItem);
                txtAddGroupMembers.Clear();
                popSearch.IsOpen = false;
                lstGroupMembers.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dbContext.Remove(selectedEmailGroup);
            dbContext.SaveChanges();
            this.objEmailPage.LoadEmailGroups();
            this.NavigationService.Navigate(objSendPage);
        }
    }
}
