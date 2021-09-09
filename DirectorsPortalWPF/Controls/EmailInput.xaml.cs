using System.Windows;
using System.Windows.Controls;

namespace DirectorsPortalWPF.Controls
{
    /// <summary>
    /// A custom control for adding new email addresses to a contact on the Modify Members
    /// page.
    /// </summary>
    public partial class EmailInput : UserControl
    {
        public string GStrInputName { get; set; } = string.Empty;
        public int GIntEmailId { get; set; } = -1;
        public ContactInput GCiContactInputParent { get; set; }

        /// <summary>
        /// Initializes the control and sets the data context.
        /// </summary>
        public EmailInput()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// A method for removing this email control from the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnRemoveEmail_Click(object sender, RoutedEventArgs e)
        {
            /* If this email is not associated with an ID from the database, don't add it
             * to the list of emails to be removed. */
            if (GIntEmailId != -1 && GCiContactInputParent != null) 
            {
                GCiContactInputParent.GIntEmailsToRemove.Add(GIntEmailId);
            }

            /* Setting the Content to null removes the control from the page. */
            Content = null;
        }
    }
}
