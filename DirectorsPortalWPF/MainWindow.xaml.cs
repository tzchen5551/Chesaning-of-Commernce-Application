using DirectorPortalDatabase;
using DirectorPortalDatabase.Utility;
using DirectorPortalDatabase.Models;
using DirectorsPortalConstantContact;
using DirectorsPortalWPF.ConstantContactUI;
using DirectorsPortalWPF.EmailMembersUI;
using DirectorsPortalWPF.GenerateReportsUI;
using DirectorsPortalWPF.HelpUI;
using DirectorsPortalWPF.MemberInfoUI;
using DirectorsPortalWPF.PaymentInfoUI;
using DirectorsPortalWPF.SettingsUI;
using DirectorsPortalWPF.TodoUI;
using DirectorsPortalWPF.ValidateWebsite;
using System;
using System.Linq;

using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

/// <summary>
/// File Purpose:
///     This file defines the Main Window object that will contain all the screens used across the Director's Portal Application. 
///     The CCOC heading and sidebar are defined here along with a WPF Frame to contain each screen (WPF Page).
///     
/// </summary>

namespace DirectorsPortalWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The object containing all data for the user of a Constant Contact account. 
        private ConstantContact gObjConstContact;
        private WebsitePreviewPage webPreviewPage;
        private ConstantContactPage constantContactPage;


        /// <summary>
        /// Launches the Window containing the application.
        /// </summary>
        private static Timer m_oTimer;

        public MainWindow()
        {
            InitializeComponent();
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));       // Appears selected
                                                                                                                    // Create the database if it doesn't exist
            DatabaseContext dbContextIntialStartup = new DatabaseContext();
            dbContextIntialStartup.Database.EnsureCreated();                     // Ensures the database is created upon application startup. If the database is not created, then the context will create the database.

            ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(true));
            gObjConstContact = new ConstantContact();

            // Populate the categories table, only if there are no categories already there.
            if (dbContextIntialStartup.Categories.ToList().Count() == 0)
                Categories.ImportFile();

            BackupUtility backupUtility = new BackupUtility();
            backupUtility.CheckBackupNotification();

            //Timer thread created to check for new todo's 
            m_oTimer = new System.Timers.Timer(.2 * 1000 * 60); // 10 seconds
            m_oTimer.Elapsed += NotificationTimer; // Timer callback
            m_oTimer.Enabled = true; // Start timer

            mainFrame.Navigate(new MembersPage(gObjConstContact));

        }
        private void NotificationTimer(Object source, ElapsedEventArgs e)
        {
            //Integer to hold value of how many incomplete todo requests
            int intNotifications = 0;

            //Ensures that this method is invoked
            this.Dispatcher.Invoke(() =>
            {

                //List to hold todo's
                List<Todo> lstTasks = new List<Todo>();

                //Gets all todo's from TodoListItems
                using (DatabaseContext Todo = new DatabaseContext())
                {
                    //Places data into List
                    lstTasks = Todo.TodoListItems.ToList();
                }

                //Loop to check for incomplete todo's
                foreach (Todo x in lstTasks)
                {
                    if (x.MarkedAsDone.Equals(false))
                    {
                        //Increments integer value for every incomplete todo
                        intNotifications++;
                    }
                }
                
                //If there are incomplete todo's then this if will fire
                if (intNotifications != 0)
                {
                    //Sets the shape and label visible. Also sets labels value.
                    lblNotificationTodo.Content = intNotifications;
                    lblNotificationTodo.Visibility = Visibility.Visible;
                    shapeNotificationLabel.Visibility = Visibility.Visible;
                }


            });

           
        }


        /// <summary>
        /// Navigates to the Payments screen. Sets the Payment button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Payment Info' Button</param>
        /// <param name="e">The Click Event</param>
        private void PaymentsPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new PaymentsPage());

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));      // Appears selected
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }

        /// <summary>
        /// Navigates to the Members screen. Sets the Member Info button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Member Info' Button</param>
        /// <param name="e">The Click Event</param>
        private void MembersPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new MembersPage(gObjConstContact));

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));       // Appears selected
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }

        /// <summary>
        /// Navigates to the Email Members screen. Sets the Email Members button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Email Members' Button</param>
        /// <param name="e">The Click Event</param>
        private void EmailPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new EmailPage());

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));        // Appears selected
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }


        /// <summary>
        /// Navigates to the Validate Website screen. Sets the Validate Website button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Validate Website' Button</param>
        /// <param name="e">The Click Event</param>
        private void WebsitePreviewPage_Navigate(object sender, RoutedEventArgs e)
        {
            if (webPreviewPage == null)
            {
                webPreviewPage = new WebsitePreviewPage();
            }
            mainFrame.Navigate(webPreviewPage);

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));       // Appears selected
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }


        /// <summary>
        /// Navigates to the Todos screen. Sets the Todos button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'TODOs' Button</param>
        /// <param name="e">The Click Event</param>
        private void TodoPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new TodoPage());

            //Hides notification when the TodoPage is navigated to.
            lblNotificationTodo.Visibility = Visibility.Hidden;
            shapeNotificationLabel.Visibility = Visibility.Hidden;

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));         // Appears selected
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }

        /// <summary>
        /// Navigates to the Settings screen. Sets the Settings button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Settings' Button</param>
        /// <param name="e">The Click Event</param>
        private void SettingsPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new SettingsPage());

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));     // Appears selected
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }

        /// <summary>
        /// Navigates to the Generate Reports screen. Sets the Generate Reports button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Generate Reports' Button</param>
        /// <param name="e">The Click Event</param>
        private void GenerateReportsPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new GenerateReportsPage());

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));    // Appears selected
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }

        /// <summary>
        /// Navigates to the Help screen. Sets the Help button to gray so as to appear selected. All other
        /// buttons appear deselected.
        /// </summary>
        /// <param name="sender">The 'Help' Button</param>
        /// <param name="e">The Click Event</param>
        private void HelpScreenPage_Navigate(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new HelpScreenPage());

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));         // Appears selected
        }

        private void ConstantContactPage_Navigate(object sender, RoutedEventArgs e)
        {
            if(constantContactPage == null)
            {
                constantContactPage = new ConstantContactPage(gObjConstContact);
            }
            mainFrame.Navigate(constantContactPage);

            btnSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnConstantContact.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD8D8D8"));  // Appears selected
            btnGenReport.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnMember.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnPayment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnTodo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnValWeb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F2F7"));
        }
    }
}
