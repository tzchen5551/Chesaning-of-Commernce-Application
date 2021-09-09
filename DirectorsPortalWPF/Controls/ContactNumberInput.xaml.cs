using System.Windows;
using System.Windows.Controls;

namespace DirectorsPortalWPF.Controls
{
    /// <summary>
    /// A custom control for adding new phone number to a contact on the Modify Members
    /// page.
    /// </summary>
    public partial class ContactNumberInput : UserControl
    {
        public string GStrInputName { get; set; } = string.Empty;
        public int GIntNumberId { get; set; } = -1;
        public ContactInput GCiContactInputParent { get; set; }

        /// <summary>
        /// Initializes the control and sets the data context.
        /// </summary>
        public ContactNumberInput()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// A method for removing this contact number control from the form.
        /// </summary>
        /// <param name="sender">The object that called the method.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void BtnRemoveNumber_Click(object sender, RoutedEventArgs e)
        {
            /* If this phone number is not associated with an ID from the database, don't add it
             * to the list of numbers to be removed. */
            if (GIntNumberId != -1 && GCiContactInputParent != null) 
            {
                GCiContactInputParent.GIntNumbersToRemove.Add(GIntNumberId);
            }

            /* Setting the Content to null removes the control from the page. */
            Content = null;
        }
    }
}
