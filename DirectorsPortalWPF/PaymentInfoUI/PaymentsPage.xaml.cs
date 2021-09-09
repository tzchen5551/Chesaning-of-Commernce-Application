using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// 
/// File Name: PaymentsPage.xaml.cs
/// 
/// Part of Project: DirectorsPortal
/// 
/// Original Author: Drake D. Herman
/// 
/// Date Created: 1/27/2021
/// 
/// File Purpose:
///     This file defines the logic for the 'Payments' screen in the Directors Portal application.
///     
/// Command Line Parameter List:
///     (NONE)
/// 
/// Environmental Returns: 
///     (NONE)
/// 
/// Sample Invocation:
///     This code is executed when the user navigates to the "Payment Info" screen from the Directors
///     portal main menu. 
///     
/// Global Variable List:
///     (NONE)
///     
/// Modification History:
///     1/27/2021 - DH: Inital creation
///     4/7/2021 - BH: Integrated with PaymentsV2 database. Revised methods accordingly.
///     
/// </summary>
namespace DirectorsPortalWPF.PaymentInfoUI
{
    /// <summary>
    /// Interaction logic for PaymentsPage.xaml
    /// </summary>
    public partial class PaymentsPage : Page
    {
        public class PaymentUIDataFields
        {
            public TextBox txtPaymentName { get; }
            public DatePicker dpPaymentPostDate { get; }
            public ObservableCollection<PaymentItem> dataSource { get; }

            public PaymentUIDataFields(TextBox txtPaymentName, DatePicker dpPaymentPostDate,
                ObservableCollection<PaymentItem> dataSource)
            {
                this.txtPaymentName = txtPaymentName;
                this.dpPaymentPostDate = dpPaymentPostDate;
                this.dataSource = dataSource;
            }
        }

        public PaymentUIDataFields CurrentCreatePaymentUIElements = null;
        public PaymentUIDataFields CurrentEditPaymentUIElements = null;

        /// <summary>
        /// Intializes the Page and content within the Page
        /// </summary>
        public PaymentsPage()
        {
            InitializeComponent();
            var context = new DatabaseContext();
            List<Business> businesses = context.Businesses
                .OrderBy(b => b.BusinessName)
                .ToList();
            lbBusinessNames.ItemsSource = businesses;
        }

        /// <summary>
        /// This methods saves the newly entered payment and regenerates the payment list.
        /// 
        /// TODO: This method will need to add a payment to the database.
        /// 
        /// Original Author: Drake D. Herman
        /// Date Created: 1/27/2021
        /// 
        /// Modification History:
        ///     1/27/2020 - DH: Intial creation        
        /// </summary>
        /// <param name="sender">The UI object that called the function.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void SaveNewPayment(object sender, RoutedEventArgs e)
        {
            if(CurrentCreatePaymentUIElements != null)
            {
                var context = new DatabaseContext();
                Business selectedBusiness = lbBusinessNames.SelectedItem as Business;
                DateTime? paymentTimestamp = CurrentCreatePaymentUIElements.dpPaymentPostDate.SelectedDate;
                if(paymentTimestamp == null)
                {
                    paymentTimestamp = DateTime.Now;
                }
                // Use data from UI elements to create payment
                Payment payment = new Payment
                {
                    Business = context.Businesses.FirstOrDefault(b => b.Id == selectedBusiness.Id),
                    Subject = CurrentCreatePaymentUIElements.txtPaymentName.Text,
                    Timestamp = (DateTime)paymentTimestamp
                };
                context.Add(payment);

                decimal grossPay = 0;
                // Add items to the created payment by pulling rows from DataGrid
                foreach(PaymentItem createdItem in CurrentCreatePaymentUIElements.dataSource)
                {
                    createdItem.Payment = payment;
                    context.Add(createdItem);
                    grossPay += (createdItem.UnitPrice * createdItem.Quantity);
                }
                payment.GrossPay = grossPay;
                context.SaveChanges();
            }

            ResetPaymentInfoColumn();
        }

        /// <summary>
        /// Show the initial Payment listing view in the right UI column.
        /// </summary>
        private void ResetPaymentInfoColumn()
        {
            CurrentCreatePaymentUIElements = null;
            CurrentEditPaymentUIElements = null;
            btnAddPayment.Visibility = Visibility.Visible;
            // Toggle the index to trigger a refresh of the payments column
            int currentIndex = lbBusinessNames.SelectedIndex;
            lbBusinessNames.SelectedIndex = -1;
            lbBusinessNames.SelectedIndex = currentIndex;
        }

        /// <summary>
        /// This methods discards the new payment and regenerates the payment list.
        /// 
        /// Original Author: Drake D. Herman
        /// Date Created: 1/27/2021
        /// 
        /// Modification History:
        ///     1/27/2020 - DH: Intial creation
        /// </summary>
        /// <param name="sender">The UI object that called the function.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void CancelNewPayment(object sender, RoutedEventArgs e)
        {
            ResetPaymentInfoColumn();
        }

        /// <summary>
        /// This methods generates the UI for adding a new payment in place of the 
        /// payments list.
        /// 
        /// Original Author: Drake D. Herman
        /// Date Created: 1/27/2021
        /// 
        /// Modification History:
        ///     1/27/2020 - DH: Intial creation
        /// 
        /// </summary>
        /// <param name="sender">The UI object that called the function.</param>
        /// <param name="e">Event data asscociated with this event.</param>
        private void AddNewPayment(object sender, RoutedEventArgs e)
        {
            spCustomerPayments.Children.Clear();
            var context = new DatabaseContext();

            btnAddPayment.Visibility = Visibility.Hidden;

            Grid gTextFields = new Grid();
            gTextFields.Width = spCustomerPayments.Width;

            /* Define the grid columns. */
            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            colDef2.Width = new GridLength(2, GridUnitType.Star);

            gTextFields.ColumnDefinitions.Add(colDef1);
            gTextFields.ColumnDefinitions.Add(colDef2);

            /* Define the grid rows */
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();

            gTextFields.RowDefinitions.Add(rowDef1);
            gTextFields.RowDefinitions.Add(rowDef2);

            /* Create the payment title entry. */
            Label lblPaymentName = new Label();
            lblPaymentName.Content = "Payment Name:";
            lblPaymentName.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(lblPaymentName, 0);
            Grid.SetColumn(lblPaymentName, 0);

            TextBox txtPaymentName = new TextBox();
            txtPaymentName.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(txtPaymentName, 0);
            Grid.SetColumn(txtPaymentName, 1);

            /* Create the date entry. */
            Label lblPaymentDate = new Label();
            lblPaymentDate.Content = "Payment Date:";
            lblPaymentDate.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(lblPaymentDate, 1);
            Grid.SetColumn(lblPaymentDate, 0);

            DatePicker datePkrPaymentDate = new DatePicker();
            datePkrPaymentDate.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(datePkrPaymentDate, 1);
            Grid.SetColumn(datePkrPaymentDate, 1);

            gTextFields.Children.Add(lblPaymentName);
            gTextFields.Children.Add(txtPaymentName);
            gTextFields.Children.Add(lblPaymentDate);
            gTextFields.Children.Add(datePkrPaymentDate);

            /* Create the data grid for enterig new payments. */
            CurrentCreatePaymentUIElements = new PaymentUIDataFields(txtPaymentName,
                datePkrPaymentDate, new ObservableCollection<PaymentItem>());

            DataGrid dgNewPayment = new DataGrid();
            dgNewPayment.AutoGenerateColumns = false;
            dgNewPayment.Margin = new Thickness(2, 5, 2, 1);
            dgNewPayment.ItemsSource = CurrentCreatePaymentUIElements.dataSource;

            DataGridTextColumn colItem = new DataGridTextColumn();
            colItem.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            colItem.Header = "Item";
            colItem.Binding = new Binding("ItemName");
            dgNewPayment.Columns.Add(colItem);

            DataGridTextColumn colQuantity = new DataGridTextColumn();
            colQuantity.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            colQuantity.Header = "Quantity";
            colQuantity.Binding = new Binding("Quantity");
            dgNewPayment.Columns.Add(colQuantity);

            DataGridTextColumn colUnitCost = new DataGridTextColumn();
            colUnitCost.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            colUnitCost.Header = "Unit Price";
            colUnitCost.Binding = new Binding("UnitPrice");
            colUnitCost.Binding.StringFormat = "c";
            dgNewPayment.Columns.Add(colUnitCost);

            /* Create the save and cancel buttons. */
            StackPanel spSaveAndCancel = new StackPanel();
            spSaveAndCancel.Width = spCustomerPayments.Width;
            spSaveAndCancel.Orientation = Orientation.Horizontal;
            spSaveAndCancel.HorizontalAlignment = HorizontalAlignment.Right;
            spSaveAndCancel.Margin = new Thickness(2, 5, 2, 1);

            Button btnCancel = new Button();
            btnCancel.Tag =
            btnCancel.Margin = new Thickness(1, 0, 1, 0);
            btnCancel.Padding = new Thickness(2, 0, 2, 0);
            btnCancel.Width = 100;
            btnCancel.Content = "Cancel";
            btnCancel.Click += CancelNewPayment;
            btnCancel.Template = (ControlTemplate)Application.Current.Resources["smallButton"];

            Button btnSave = new Button();
            btnSave.Margin = new Thickness(1, 0, 1, 0);
            btnSave.Padding = new Thickness(2, 0, 2, 0);
            btnSave.Width = 100;
            btnSave.Content = "Add Payment";
            btnSave.Click += SaveNewPayment;
            btnSave.Template = (ControlTemplate)Application.Current.Resources["smallButton"]; 

            spSaveAndCancel.Children.Add(btnCancel);
            spSaveAndCancel.Children.Add(btnSave);

            spCustomerPayments.Children.Add(gTextFields);
            spCustomerPayments.Children.Add(dgNewPayment);
            spCustomerPayments.Children.Add(spSaveAndCancel);
        }

        /// <summary>
        /// Opens a pop-up window that displays the current frames help information. 
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">The Click event</param>
        private void HelpButtonHandler(object sender, EventArgs e)
        {
            HelpUI.HelpScreenWindow helpWindow = new HelpUI.HelpScreenWindow();
            helpWindow.Show();
            helpWindow.tabs.SelectedIndex = 1;

        }

        /// <summary>
        /// Event when the user selects a Business from the list in the left UI column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbBusinessNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spCustomerPayments.Children.Clear();
            btnAddPayment.Visibility = Visibility.Visible;
            if (lbBusinessNames.SelectedItem == null)
            {
                return;
            }
            var context = new DatabaseContext();
            Business selectedBusiness = lbBusinessNames.SelectedItem as Business;
            List<Payment> paymentsFromBusiness = context.Payments
                .Where(p => p.Business.Id == selectedBusiness.Id)
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            lblSelectedCustomer.Content = selectedBusiness.BusinessName;
            dpSelectedCustomer.Visibility = Visibility.Visible;

            foreach (Payment payment in paymentsFromBusiness)
            {
                context.Entry(payment).Collection(p => p.Items).Load();
                Expander expPayment = new Expander();
                expPayment.HorizontalAlignment = HorizontalAlignment.Stretch;
                expPayment.ExpandDirection = ExpandDirection.Down;
                expPayment.FontWeight = FontWeights.Normal;
                expPayment.Width = spCustomerPayments.Width;

                StackPanel spHeader = new StackPanel();
                StackPanel spHeaderTop = new StackPanel();
                StackPanel spHeaderBodyButtons = new StackPanel();
                Grid grdHeaderBody = new Grid();
                grdHeaderBody.HorizontalAlignment = HorizontalAlignment.Stretch;
                spHeaderTop.Orientation = Orientation.Horizontal;
                spHeaderBodyButtons.Orientation = Orientation.Horizontal;
                spHeader.HorizontalAlignment = HorizontalAlignment.Stretch;

                RowDefinition rowDef1 = new RowDefinition();
                RowDefinition rowDef2 = new RowDefinition();
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                ColumnDefinition colDef3 = new ColumnDefinition();
                colDef1.Width = new GridLength(1, GridUnitType.Star);
                colDef2.Width = new GridLength(1, GridUnitType.Auto);
                colDef3.Width = new GridLength(1, GridUnitType.Auto);
                grdHeaderBody.RowDefinitions.Add(rowDef1);
                grdHeaderBody.RowDefinitions.Add(rowDef2);
                grdHeaderBody.ColumnDefinitions.Add(colDef1);
                grdHeaderBody.ColumnDefinitions.Add(colDef2);
                grdHeaderBody.ColumnDefinitions.Add(colDef3);

                Label lblPaymentDate = new Label();
                lblPaymentDate.Padding = new Thickness(10, 0, 0, 0);
                lblPaymentDate.Content = payment.Timestamp.ToString("MM/dd/yyyy");

                TextBlock txtPaymentSubject = new TextBlock();
                txtPaymentSubject.Padding = new Thickness(10, 0, 0, 0);
                txtPaymentSubject.Text = payment.Subject;
                txtPaymentSubject.TextWrapping = TextWrapping.Wrap;
                txtPaymentSubject.HorizontalAlignment = HorizontalAlignment.Stretch;
                txtPaymentSubject.FontWeight = FontWeights.Bold;

                Label lblPaymentNetPay = new Label();
                lblPaymentNetPay.Padding = new Thickness(10, 0, 0, 0);
                lblPaymentNetPay.Content = payment.GrossPay.ToString("C", CultureInfo.CurrentCulture);

                Button btnEdit = new Button();
                btnEdit.Margin = new Thickness(7, 2, 5, 2);
                btnEdit.Padding = new Thickness(30, 0, 30, 0);
                btnEdit.Content = "Edit";
                btnEdit.DataContext = payment;
                btnEdit.Click += EditPayment;
                btnEdit.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
                btnEdit.VerticalAlignment = VerticalAlignment.Center;

                Button btnDelete = new Button();
                btnDelete.Margin = new Thickness(5, 2, 5, 2);
                btnDelete.Padding = new Thickness(30, 0, 30, 0);
                btnDelete.Content = "Delete";
                btnDelete.DataContext = payment;
                btnDelete.Click += DeletePayment;
                btnDelete.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
                btnDelete.VerticalAlignment = VerticalAlignment.Center;

                Grid.SetRow(txtPaymentSubject, 0);
                //Grid.SetRow(spHeaderBodyButtons, 0);
                Grid.SetRow(spHeaderTop, 1);
                //Grid.SetRow(lblPaymentNetPay, 1);
                //Grid.SetColumn(lblPaymentDate, 0);
                //Grid.SetColumn(spHeaderBodyButtons, 1);
                Grid.SetColumn(spHeaderTop, 0);
                Grid.SetColumnSpan(txtPaymentSubject, 2);

                spHeaderTop.Children.Add(lblPaymentDate);
                spHeaderTop.Children.Add(lblPaymentNetPay);
                // spHeaderTop.Children.Add(grdHeaderBody);

                grdHeaderBody.Children.Add(spHeaderTop);
                //grdHeaderBody.Children.Add(lblPaymentNetPay);
                grdHeaderBody.Children.Add(txtPaymentSubject);

                // grdHeaderBody.Children.Add(spHeaderBodyButtons);

                spHeaderBodyButtons.Children.Add(btnEdit);
                spHeaderBodyButtons.Children.Add(btnDelete);

                spHeader.Children.Add(grdHeaderBody);
                spHeader.Children.Add(spHeaderBodyButtons);

                expPayment.Header = spHeader;

                DataGrid dgPaymentItems = new DataGrid();
                dgPaymentItems.Width = spCustomerPayments.Width;
                dgPaymentItems.IsReadOnly = true;

                DataGridTextColumn colItem = new DataGridTextColumn();
                colItem.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                colItem.Header = "Item";
                colItem.Binding = new Binding("ItemName");
                dgPaymentItems.Columns.Add(colItem);

                DataGridTextColumn colQuantity = new DataGridTextColumn();
                colQuantity.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                colQuantity.Header = "Quantity";
                colQuantity.Binding = new Binding("Quantity");
                dgPaymentItems.Columns.Add(colQuantity);

                DataGridTextColumn colUnitCost = new DataGridTextColumn();
                colUnitCost.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                colUnitCost.Header = "Unit Price";
                colUnitCost.Binding = new Binding("UnitPrice");
                colUnitCost.Binding.StringFormat = "c";
                dgPaymentItems.Columns.Add(colUnitCost);

                List<PaymentItem> paymentItems = context.PaymentItems
                    .Where(p => p.Payment.Id == payment.Id)
                    .ToList();
                foreach (PaymentItem paymentItem in paymentItems)
                {
                    dgPaymentItems.Items.Add(paymentItem);
                }

                expPayment.Content = dgPaymentItems;

                spCustomerPayments.Children.Add(expPayment);
            }
        }

        /// <summary>
        /// Delete the payment the sending button was on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePayment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Payment payment = button.DataContext as Payment;
            var context = new DatabaseContext();
            context.Payments.Remove(payment);
            foreach(PaymentItem item in payment.Items)
            {
                context.PaymentItems.Remove(item);
            }
            context.SaveChanges();

            ResetPaymentInfoColumn();
        }

        /// <summary>
        /// Replace the right UI column with the Payment editing interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPayment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Payment payment = button.DataContext as Payment;

            spCustomerPayments.Children.Clear();
            var context = new DatabaseContext();

            btnAddPayment.Visibility = Visibility.Hidden;

            Grid gTextFields = new Grid();
            gTextFields.Width = spCustomerPayments.Width;

            /* Define the grid columns. */
            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            colDef2.Width = new GridLength(2, GridUnitType.Star);

            gTextFields.ColumnDefinitions.Add(colDef1);
            gTextFields.ColumnDefinitions.Add(colDef2);

            /* Define the grid rows */
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();

            gTextFields.RowDefinitions.Add(rowDef1);
            gTextFields.RowDefinitions.Add(rowDef2);

            /* Create the payment title entry. */
            Label lblPaymentName = new Label();
            lblPaymentName.Content = "Payment Name:";
            lblPaymentName.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(lblPaymentName, 0);
            Grid.SetColumn(lblPaymentName, 0);

            TextBox txtPaymentName = new TextBox();
            txtPaymentName.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(txtPaymentName, 0);
            Grid.SetColumn(txtPaymentName, 1);

            /* Create the date entry. */
            Label lblPaymentDate = new Label();
            lblPaymentDate.Content = "Payment Date:";
            lblPaymentDate.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(lblPaymentDate, 1);
            Grid.SetColumn(lblPaymentDate, 0);

            DatePicker datePkrPaymentDate = new DatePicker();
            datePkrPaymentDate.Margin = new Thickness(1, 1, 2, 1);
            Grid.SetRow(datePkrPaymentDate, 1);
            Grid.SetColumn(datePkrPaymentDate, 1);

            gTextFields.Children.Add(lblPaymentName);
            gTextFields.Children.Add(txtPaymentName);
            gTextFields.Children.Add(lblPaymentDate);
            gTextFields.Children.Add(datePkrPaymentDate);

            /* Create the data grid for entering new payments. */
            DataGrid dgPaymentItems = new DataGrid();
            dgPaymentItems.AutoGenerateColumns = false;
            dgPaymentItems.Margin = new Thickness(2, 5, 2, 1);

            /* Setup input UI in class scope for use in SaveEditPayment() */
            CurrentEditPaymentUIElements = new PaymentUIDataFields(
                txtPaymentName,
                datePkrPaymentDate,
                new ObservableCollection<PaymentItem>(payment.Items));

            /* Set source to pull from existing Payment */
            dgPaymentItems.ItemsSource = CurrentEditPaymentUIElements.dataSource;

            DataGridTextColumn colItem = new DataGridTextColumn();
            colItem.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            colItem.Header = "Item";
            colItem.Binding = new Binding("ItemName");
            dgPaymentItems.Columns.Add(colItem);

            DataGridTextColumn colQuantity = new DataGridTextColumn();
            colQuantity.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            colQuantity.Header = "Quantity";
            colQuantity.Binding = new Binding("Quantity");
            dgPaymentItems.Columns.Add(colQuantity);

            DataGridTextColumn colUnitCost = new DataGridTextColumn();
            colUnitCost.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            colUnitCost.Header = "Unit Price";
            colUnitCost.Binding = new Binding("UnitPrice");
            colUnitCost.Binding.StringFormat = "c";
            dgPaymentItems.Columns.Add(colUnitCost);

            /* Create the save and cancel buttons. */
            StackPanel spSaveAndCancel = new StackPanel();
            spSaveAndCancel.Width = spCustomerPayments.Width;
            spSaveAndCancel.Orientation = Orientation.Horizontal;
            spSaveAndCancel.HorizontalAlignment = HorizontalAlignment.Right;
            spSaveAndCancel.Margin = new Thickness(2, 5, 2, 1);

            Button btnCancel = new Button();
            btnCancel.Tag =
            btnCancel.Margin = new Thickness(1, 0, 1, 0);
            btnCancel.Padding = new Thickness(2, 0, 2, 0);
            btnCancel.Width = 100;
            btnCancel.Content = "Cancel";
            btnCancel.Click += CancelEditPayment;
            btnCancel.Template = (ControlTemplate)Application.Current.Resources["smallButton"];

            Button btnSave = new Button();
            btnSave.Margin = new Thickness(1, 0, 1, 0);
            btnSave.Padding = new Thickness(2, 0, 2, 0);
            btnSave.Width = 100;
            btnSave.Content = "Save Changes";
            btnSave.Click += SaveEditPayment;
            btnSave.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
            btnSave.DataContext = payment;

            spSaveAndCancel.Children.Add(btnCancel);
            spSaveAndCancel.Children.Add(btnSave);

            spCustomerPayments.Children.Add(gTextFields);
            spCustomerPayments.Children.Add(dgPaymentItems);
            spCustomerPayments.Children.Add(spSaveAndCancel);

            /* Populate the UI with the existing payment info */
            txtPaymentName.Text = payment.Subject;
            datePkrPaymentDate.SelectedDate = payment.Timestamp;
        }

        /// <summary>
        /// Commit the edited Payment fields to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEditPayment(object sender, RoutedEventArgs e)
        {
            if (CurrentEditPaymentUIElements != null)
            {
                Button button = sender as Button;
                Payment payment = button.DataContext as Payment;
                var context = new DatabaseContext();
                Business selectedBusiness = lbBusinessNames.SelectedItem as Business;
                // Use data from UI elements to reset payment members and save to db
                payment.Timestamp = (DateTime)CurrentEditPaymentUIElements.dpPaymentPostDate.SelectedDate;
                payment.Subject = CurrentEditPaymentUIElements.txtPaymentName.Text;

                decimal grossPay = 0;
                /* Rebuild items from DataGrid */
                foreach (PaymentItem createdItem in CurrentEditPaymentUIElements.dataSource)
                {
                    createdItem.Payment = payment;
                    context.Update(createdItem);
                    grossPay += (createdItem.UnitPrice * createdItem.Quantity);
                }
                payment.GrossPay = grossPay;
                context.Update(payment);
                context.SaveChanges();
            }

            ResetPaymentInfoColumn();
        }

        /// <summary>
        /// Cancel the edit Payment interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelEditPayment(object sender, RoutedEventArgs e)
        {
            ResetPaymentInfoColumn();
        }
    }
}