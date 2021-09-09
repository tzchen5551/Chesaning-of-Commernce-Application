using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using DirectorsPortalWPF.EmailMembersSendEmailUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
/// This file defines the EmailPage object which contains the AddGroups, EditGroups, and SendEmail windows. 
/// Also contains the logic for displaying existing Email Groups. 
/// </summary>
namespace DirectorsPortalWPF.EmailMembersUI
{


    /// <summary>
    /// Interaction logic for EmailPage.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        /// <summary>
        /// Test list for setting up the UI. This variable should not be used in
        /// the final implmentation.
        /// </summary>
        List<EmailGroup> mrgGroupList = new List<EmailGroup>();
        DatabaseContext dbContext = new DatabaseContext();
        List<EmailGroup> rgSendingEmailGroups;
        EmailMembersSendEmailPage emailMembersSendEmailPage;

        /// <summary>
        /// Initialize the email page. Automatically gets run
        /// upon creating the page in WPF. Will call the
        /// <see cref="EmailPage.LoadEmailGroups()"/> function
        /// to load in a list of the email groups to the UI.
        /// </summary>

        public EmailPage()
        {
            InitializeComponent();
            LoadEmailGroups();
            rgSendingEmailGroups = new List<EmailGroup>();
            emailMembersSendEmailPage = new EmailMembersSendEmailPage(rgSendingEmailGroups, this);
            emailFrame.Navigate(emailMembersSendEmailPage);
        }

        /// <summary>
        /// Pulls the list of email groups. Depending on whether
        /// Outlook or ConstantContact is used, the list will come
        /// from a different place, but will be rendered on the UI
        /// the same way. This function allows for a dynamic load
        /// of the emails so that if a new group is added, it does
        /// not have to be hard coded.
        /// </summary>
        public void LoadEmailGroups()
        {
            // Pull the email list element from the page
            object nodGroupList = FindName("EmailGroupList");
            // Ensure that the email list element is a stack panel
            if (nodGroupList is StackPanel)
            {
                // Cast email list to stack panel so it can be used
                StackPanel vspGroupList = nodGroupList as StackPanel;
                vspGroupList.Children.Clear();

                // TODO: GroupList should be retrieved from an API from SDK team or database team. Values added for test purposes
                mrgGroupList = dbContext.EmailGroups.ToList();

                //mrgGroupList.Add(new Group("Silver", new string[] { "Tom", "John" }, "Test Note"));
                //mrgGroupList.Add(new Group("Gold", new string[] { "Jane", "Bill" }, "Test Note2"));

                foreach (EmailGroup group in mrgGroupList)
                {
                    // For every email group found in the database, create a row
                    // in the email groups list with label and an edit button
                    StackPanel hspEmailGroupRow = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    Label lblEmailGroupName;
                    if (group.GroupName.Length > 12)
                    {
                        lblEmailGroupName = new Label()
                        {
                            Content = group.GroupName.Substring(0, 12) + "..."
                        };
                    }
                    else
                    {
                        lblEmailGroupName = new Label()
                        {
                            Content = group.GroupName
                        };
                    }
                    Button btnEmailGroupEditButton = new Button()
                    {
                        Content = "Edit",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Template = (ControlTemplate)Application.Current.Resources["smallButton"],
                        Padding = new Thickness(0, 0, 35, 0),
                        Margin = new Thickness(5, 0, 0, 0),
                        Height = 15,
                    };
                    Button btnEmailGroupSendButton = new Button()
                    {
                        Content = "Select",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Template = (ControlTemplate)Application.Current.Resources["smallButton"],
                        Padding = new Thickness(0, 0, 42, 0),
                        Height = 15,
                    };


                    btnEmailGroupEditButton.Click += (s, e) =>
                    {
                        /// <summary>
                        /// Navigates to the EditGroups screen and passes the corresponding group name
                        /// </summary>
                        /// <param name="sender">The 'Edit' Button</param>
                        /// <param name="e">The Click Event</param>
                        emailFrame.Navigate(new EmailMembersEditGroupsUI.EmailMembersEditGroupsPage(group, this, emailMembersSendEmailPage));
                    };

                    btnEmailGroupSendButton.Click += (sender, e) => AddEmailGroupToMessage(sender, e, group);
                    hspEmailGroupRow.Children.Add(btnEmailGroupSendButton);
                    hspEmailGroupRow.Children.Add(btnEmailGroupEditButton);
                    hspEmailGroupRow.Children.Add(lblEmailGroupName);
                    vspGroupList.Children.Add(hspEmailGroupRow);
                }

            }
        }

        /// Navigates to the AddGroups screen.
        /// </summary>
        /// <param name="sender">The 'Add' Button</param>
        /// <param name="e">The Click Event</param>
        private void AddGroupsPage_Navigate(object sender, RoutedEventArgs e)
        {
            emailFrame.Navigate(new EmailMembersAddGroupsUI.EmailMembersAddGroupsPage(this, emailMembersSendEmailPage));
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
            helpWindow.tabs.SelectedIndex = 2;
        }

        /// <summary>
        /// Adds an email group to the 'To:' field, delimited by a semicolon (;)
        /// </summary>
        /// <param name="sender">The 'Select' button for the selected email group</param>
        /// <param name="e">The click event</param>
        /// <param name="emailGroup">The email group name being selected</param>
        private void AddEmailGroupToMessage(object sender, RoutedEventArgs e, EmailGroup emailGroup)
        {
            // This is just template code for now. In the future this method should be able to read a emailGroup object
            // and load the group name to the 'To' field.

            Button btnSelection = (Button)sender;

            // Only add the email group if it hasn't been selected, otherwise remove the group.
            if (emailMembersSendEmailPage.txtToField.Text.Contains(emailGroup.GroupName))
            {
                rgSendingEmailGroups.Remove(emailGroup);
                emailMembersSendEmailPage.txtToField.Text = emailMembersSendEmailPage.txtToField.Text.Replace($"{emailGroup.GroupName}; ", "");
                btnSelection.Content = "Select";
            }
            else
            {
                rgSendingEmailGroups.Add(emailGroup);
                emailMembersSendEmailPage.txtToField.Text += $"{emailGroup.GroupName}; ";
                btnSelection.Content = "Remove";
            }
        }
    }

}

/// <summary>
/// A placeholder test class that defines the properties of a Group.
/// </summary>
public class Group
{
    public string gstrName { get; set; }
    public string[] garrMembers { get; set; }
    public string gstrNote { get; set; }

    public Group(string name, string[] members, string note)
    {
        gstrName = name;
        garrMembers = members;
        gstrNote = note;
    }
}
