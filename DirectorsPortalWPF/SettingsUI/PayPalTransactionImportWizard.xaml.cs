using DirectorPortalDatabase;
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
using System.Windows.Shapes;

namespace DirectorsPortalWPF.SettingsUI
{
    /// <summary>
    /// Interaction logic for PayPalTransactionImportWizard.xaml
    /// </summary>
    public partial class PayPalTransactionImportWizard : Window
    {
        public List<PayPalTransactionImportWizardPage> Pages { get; }
        public int PageIndex { get; private set; }
        public int AddressedCount { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="failedTransactions">The transactions that failed to import.</param>
        public PayPalTransactionImportWizard(List<TransactionImportFailure> failedTransactions)
        {
            InitializeComponent();
            Pages = new List<PayPalTransactionImportWizardPage>();
            foreach (TransactionImportFailure importFailure in failedTransactions)
            {
                if(importFailure.FailureReason != TransactionImportFailure.FailureReasons.Duplicate)
                {
                    PayPalTransactionImportWizardPage page = new PayPalTransactionImportWizardPage(importFailure);
                    page.OnStatusChanged += Page_OnStatusChanged;
                    Pages.Add(page);
                }
            }
            PageIndex = 0;
            AddressedCount = 0;
            
            lblPageIndex.Content = "Page " + (PageIndex + 1) + " of " + Pages.Count;
            lblCorrectedStatus.Content = AddressedCount + " of " + Pages.Count + " Addressed";

            frmPage.Navigate(Pages[PageIndex]);
        }

        /// <summary>
        /// Triggered when a PayPalTransactionImportWizardPage's status changes due to the user performing 
        /// an action on the page.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void Page_OnStatusChanged(object source, 
            PayPalTransactionImportWizardPage.StatusChangedEventArgs args)
        {
            AddressedCount = 0;
            foreach(PayPalTransactionImportWizardPage page in Pages)
            {
                if(page.Status != PayPalTransactionImportWizardPage.PageStatus.InProgress)
                {
                    AddressedCount++;
                }
            }
            lblCorrectedStatus.Content = AddressedCount + " of " + Pages.Count + " Addressed";
            btnPageNext_Click(null, null);
        }

        /// <summary>
        /// Triggered when the back button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPageBack_Click(object sender, RoutedEventArgs e)
        {
            if(PageIndex > 0)
            {
                PageIndex--;
                frmPage.Navigate(Pages[PageIndex]);
                lblPageIndex.Content = "Page " + (PageIndex + 1) + " of " + Pages.Count;
            }
        }

        /// <summary>
        /// Triggered when the next button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPageNext_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex < Pages.Count() - 1)
            {
                PageIndex++;
                frmPage.Navigate(Pages[PageIndex]);
                lblPageIndex.Content = "Page " + (PageIndex + 1) + " of " + Pages.Count;
            }
        }

        /// <summary>
        /// Triggered when the finish button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            /* If any pages aren't complete, fail to finish */
            if(AddressedCount != Pages.Count)
            {
                return;
            }

            var context = new DatabaseContext();
            int skippedCount = 0;
            int postedToDbCount = 0;
            foreach(PayPalTransactionImportWizardPage page in Pages)
            {
                if(page.Status == PayPalTransactionImportWizardPage.PageStatus.Skipped)
                {
                    skippedCount++;
                    continue;
                }
                else if(page.Status == PayPalTransactionImportWizardPage.PageStatus.Confirmed)
                {
                    Payment payment = new Payment
                    {
                        Business = context.Businesses.FirstOrDefault(b => b.Id == page.SelectedBusinessMapping.Id),
                        GrossPay = Decimal.Parse(page.ImportFailure.Transaction.Gross),
                        InvoiceNumber = page.ImportFailure.Transaction.InvoiceNumber,
                        Subject = page.ImportFailure.Transaction.Subject,
                        PayPalReferenceTxnId = page.ImportFailure.Transaction.ReferenceTxnId,
                        PayPalTransactionId = page.ImportFailure.Transaction.TransactionId,
                        ProcessingFees = Decimal.Parse(page.ImportFailure.Transaction.Fee),
                        Timestamp = DateTime.Parse(page.ImportFailure.Transaction.Date)
                    };
                    context.Payments.Add(payment);
                    postedToDbCount++;
                }
            }
            context.SaveChanges();
            this.Close();
            MessageBox.Show(postedToDbCount
                        + " PayPal transactions successfully corrected and imported into Payments.\n"
                        + skippedCount + " were skipped and not imported.",
                        "PayPal Import Wizard Finished");
        }
    }
}
