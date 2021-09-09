using DirectorPortalDatabase.Models;
using DirectorPortalPayPal;
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

namespace DirectorsPortalWPF.SettingsUI
{

    /// <summary>
    /// The page for mapping a PayPal transaction that failed to import to a Business in the database.
    /// </summary>
    public partial class PayPalTransactionImportWizardPage : Page
    {
        public delegate void StatusChangedEventHandler(object source, StatusChangedEventArgs args);

        /// <summary>
        /// Context of a StatusChanged Event.
        /// </summary>
        public class StatusChangedEventArgs : EventArgs
        {
            public Business SelectedBusinessMapping { get; }
            public PageStatus FromStatus { get; }
            public PageStatus ToStatus { get; }
            public StatusChangedEventArgs(PageStatus fromStatus, PageStatus toStatus, Business business)
            {
                FromStatus = fromStatus;
                ToStatus = toStatus;
                SelectedBusinessMapping = business;
            }
        }

        /// <summary>
        /// The possible states the page could be in regarding what the user wants to do about this import failure.
        /// </summary>
        public enum PageStatus
        {
            /// <summary>
            /// Page not yet addressed by user.
            /// </summary>
            InProgress,
            /// <summary>
            /// Transaction was chosen to not be imported.
            /// </summary>
            Skipped,
            /// <summary>
            /// Transaction was mapped to a Business and confirmed by user.
            /// </summary>
            Confirmed
        }

        public event StatusChangedEventHandler OnStatusChanged;
        public TransactionImportFailure ImportFailure { get; }
        public PageStatus Status { get; private set; }
        public Business SelectedBusinessMapping { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="importFailure">The context of the import failure.</param>
        public PayPalTransactionImportWizardPage(TransactionImportFailure importFailure)
        {
            InitializeComponent();
            ImportFailure = importFailure;
            Status = PageStatus.InProgress;
            SelectedBusinessMapping = null;

            txtCustomerName.Text = ImportFailure.Transaction.Name;
            txtDate.Text = ImportFailure.Transaction.Date;
            txtFromEmail.Text = ImportFailure.Transaction.FromEmail;
            txtGrossPay.Text = ImportFailure.Transaction.Gross;
            txtSubject.Text = ImportFailure.Transaction.Subject;
            var context = new DirectorPortalDatabase.DatabaseContext();
            dgBusinesses.ItemsSource = context.Businesses
                .OrderBy(b => b.BusinessName)
                .ToList();
        }

        /// <summary>
        /// Triggered when the skip button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            PageStatus oldStatus = Status;
            Status = PageStatus.Skipped;
            SelectedBusinessMapping = null;
            if (OnStatusChanged != null)
            {
                OnStatusChanged(this, new StatusChangedEventArgs(oldStatus, Status, null));
            }
        }

        /// <summary>
        /// Triggered when the confirm button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmPage_Click(object sender, RoutedEventArgs e)
        {
            Business business = dgBusinesses.SelectedItem as Business;
            if(business == null)
            {
                /* Don't move on, show error to user, they need to select a business to confirm */
                MessageBox.Show("You must select a business to map this PayPal transaction to, or skip importing this payment.",
                    "Incomplete");
            }
            else
            {
                PageStatus oldStatus = Status;
                Status = PageStatus.Confirmed;
                SelectedBusinessMapping = business;
                if (OnStatusChanged != null)
                {
                    OnStatusChanged(this, new StatusChangedEventArgs(oldStatus, Status, business));
                }
            }
        }
    }
}
