using DirectorsPortalWPF.MemberInfoUI;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DirectorsPortalWPF.Controls
{
    /// <summary>
    /// A custom control for adding new contact to the Modify Members page.
    /// </summary>
    public partial class ContactInput : UserControl
    {
        public string GStrTitle { get; set; } = string.Empty;
        public int GIntContactId { get; set; } = -1;
        public int GntEmailCount { get; set; } = 0;
        public int GIntNumberCount { get; set; } = 0;
        public List<int> GIntEmailsToRemove { get; set; } = new List<int>();
        public List<int> GIntNumbersToRemove { get; set; } = new List<int>();

        /// <summary>
        /// Initializes the control and sets the data context.
        /// </summary>
        public ContactInput()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        /// <summary>
        /// A method for adding a new email input to the control.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnAddEmail_Click(object sender, RoutedEventArgs e)
        {
            GntEmailCount++;

            EmailInput EiEmail = new EmailInput
            {
                GStrInputName = "Email " + GntEmailCount + ":",
            };

            SpContactEmails.Children.Add(EiEmail);
        }

        /// <summary>
        /// A method for adding a new contact number input to the control.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnAddNumber_Click(object sender, RoutedEventArgs e)
        {
            GIntNumberCount++;

            ContactNumberInput CniNumber = new ContactNumberInput
            {
                GStrInputName = "Number " + GIntNumberCount + ":",
            };

            SpContactNumbers.Children.Add(CniNumber);
        }

        /// <summary>
        /// A method for removing this contact control from the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnRemoveContact_Click(object sender, RoutedEventArgs e)
        {
            /* If this conatct input is not associated with an ID from the database, don't add it
             * to the list of numbers to be removed. */
            if (GIntContactId != -1) 
            {
                ModifyMembersPage page = NavigationService.GetNavigationService(this).Content as ModifyMembersPage;
                page.GIntContactsToRemove.Add(GIntContactId);
            }

            /* Setting the Content to null removes the control from the page. */
            Content = null;
        }
    }
}
