using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using ExcelDataReader;
using DirectorPortalDatabase.Utility;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DirectorPortalPayPal;


/// <summary>
/// File Purpose:
///     This file defines the logic for the 'Settings' screen in the Directors Portal application. The 
///     'Settings' screen consists of tabs including:
///         - Backup and Restore
///         - Edit Fields
///         - OTHERS TO BE DETERMINED
///         
/// </summary>

namespace DirectorsPortalWPF.SettingsUI
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private MetadataHelper.ModelInfo GUdtSelectedReportType { get; set; }

        //Reference to form needed to be displayed during import excel data
        private ImportLoadPage frmLoadPage = new ImportLoadPage();
        
        //List using class Members to store Member information.
        List<Members> Members = new List<Members>();

        private List<string> rgAvailableFields;
        private string strOldFieldName;
        /// <summary>
        /// Intializes the Page and content within the Page
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent(); //initialize the GUI

/*            Business business;
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                business = dbContext.Businesses.Where(x => x.Id == 1).FirstOrDefault();
                business.SetField("Another Field", "Another Test", dbContext);
            }*/

            //create selections for the combo box that allows the user to choose when they will recieve backup notifications
            cmbNotificationFrequency.ItemsSource = new List<string> { "None", "Daily", "Weekly", "Monthly" };
            cmbNotificationTime.ItemsSource = GenerateDropdownTimeList();
            
            //loads settings
            LoadSavedSettings();
            DisplayEditableFields();
        }

        /// <summary>
        /// Loads in settings that are stored in the c# settings file.
        /// </summary>
        private void LoadSavedSettings()
        {
            
            rbtnMonday.IsChecked = true; //set this to true for first time use

            RadioButton radioButton; //used to cast an object iterator to a radiobutton in a for each loop

            //loads simple settings.
            cmbNotificationFrequency.SelectedIndex = Properties.Settings.Default.SelectedIndexFreq;
            cmbNotificationTime.SelectedIndex = Properties.Settings.Default.SelectedIndexTime;
            txtBoxFileBackup.Text = Properties.Settings.Default.BackupLocation;

            //if weekly has been selected check the correct radio button.
            //settings persist the name of the button that was last selected and 
            //compares it against all the options until a match is found
            if (cmbNotificationFrequency.SelectedValue.Equals("Weekly"))
            {
                foreach (object dayOfWeek in sPanelRadioButtonForWeekly.Children)
                {
                    if (dayOfWeek is RadioButton)
                    {
                        radioButton = (RadioButton)dayOfWeek;
                        if (radioButton.Name == Properties.Settings.Default.DayOfWeek)
                        {
                            radioButton.IsChecked = true;
                        }
                    }
                }

            }

            btnSaveNotificationSettings.Visibility = Visibility.Hidden; //dont show save settings button

        }

        /// <summary>
        /// Creates a list of times to be used in the dropdown list in the Backup and Restore section 
        /// of Settings.
        /// </summary>
        /// <returns>A list of all times in a day</returns>
        private List<string> GenerateDropdownTimeList()
        {
            return new List<string>
            {
                "",
                "12:00am",
                "1:00am",
                "2:00am",
                "3:00am",
                "4:00am",
                "5:00am",
                "6:00am",
                "7:00am",
                "8:00am",
                "9:00am",
                "10:00am",
                "11:00am",
                "12:00pm",
                "1:00pm",
                "2:00pm",
                "3:00pm",
                "4:00pm",
                "5:00pm",
                "6:00pm",
                "7:00pm",
                "8:00pm",
                "9:00pm",
                "10:00pm",
                "11:00pm"
            };
        }

        /// <summary>
        /// Deletes a Field from the list (will need to be converted for DB use). Will also display a message confirming the delete operation.
        /// 
        /// TODO: Modify the DeleteTextField method in 'Settings' to work with the DB
        /// </summary>
        /// <param name="sender">The Delete Button</param>
        /// <param name="e">The Click Event</param>
        /// <param name="sPanelToDelete">The Stack Panel containing the Field to be deleted.</param>
        private void DeleteTextField(object sender, RoutedEventArgs e, StackPanel sPanelToDelete)
        {
            TextBox txtBoxFieldEdit = (TextBox)sPanelToDelete.Children[0];
            MessageBoxResult confirmDelete = MessageBox.Show(
                $"Are you sure you want to delete \'{txtBoxFieldEdit.Text}\'? All data in this field will become inaccessible, this operation cannot be reversed.",
                "Warning!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            switch (confirmDelete)
            {
                case MessageBoxResult.Yes:
                    {
                        using (DatabaseContext dbContext = new DatabaseContext())
                        {
                            Business business = new Business();
                            business.DeleteField(dbContext, txtBoxFieldEdit.Text);
                            DisplayEditableFields();
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    return;
            }
        }

        /// <summary>
        /// Sets the TextBox that contains the name of the Field from the database. Currently just
        /// changes the TextField text that is there. This method can be modified to work with a database.
        /// 
        /// TODO: Modify the SetTextField method in the 'Settings' to work with the DB
        /// </summary>
        /// <param name="sender">The button that is calling the Click event</param>
        /// <param name="e">The event itself</param>
        /// <param name="txtBox">The TextBox that needs to be edited</param>
        private void SetTextField(object sender, RoutedEventArgs e, TextBox txtBox, Button btnDelete)
        {
            Button btnEditText = (Button)sender;       // Sender should be converted to a button.

            if (txtBox.IsEnabled)
            {   // Disable the TextBox and Save, change the button lable back to "Edit"
                txtBox.IsEnabled = false;
               
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    Business business = new Business();
                    business.RenameField(dbContext, strOldFieldName, txtBox.Text);
                    DisplayEditableFields();
                }
/*
                btnEditText.Content = "Edit";
                btnDelete.Visibility = Visibility.Hidden;*/
            }
            else
            {   // Enable the TextBox and change the button label to "Save"
                strOldFieldName = txtBox.Text;
                txtBox.IsEnabled = true;
                btnEditText.Content = "Save";
                btnDelete.Visibility = Visibility.Visible;
            }

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
            helpWindow.tabs.SelectedIndex = 7;

        }

        /// <summary>
        /// Creates a new Text Box for the 'Edit Fields' tab on the Settings screen.
        /// </summary>
        /// <param name="blnIsEnabled">Set the Text Field to either be enabled or disabled</param>
        /// <returns>Returns a generated TextBox object with pre-set formatting</returns>
        private TextBox CreateTextBox(bool blnIsEnabled)
        {
            TextBox txtBoxNewTextBox = new TextBox
            {
                Height = 30,
                Width = 300,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsEnabled = blnIsEnabled
            };

            return txtBoxNewTextBox;
        }

        /// <summary>
        /// Created a new Stack Panel to be used by the 'Edit Fields' tab on the Settings screen.
        /// </summary>
        /// <param name="OriDesiredOrientation">Set the orientation of the Stack Panel, either Vertical or Horizontal
        /// (NOTE: use Orientation.[Vertial | Horizontal]</param>
        /// <returns>Returns a generated StackPanel object with pre-set formatting</returns>
        private StackPanel CreateStackPanel(Orientation OriDesiredOrientation)
        {
            StackPanel sPanelNewStackPanel = new StackPanel
            {
                Orientation = OriDesiredOrientation,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Center
            };

            return sPanelNewStackPanel;
        }

        /// <summary>
        /// Creates a new Button to be used by the 'Edit Fields' tab on the Settings screen.
        /// </summary>
        /// <returns>Returns a generated Button object with pre-set formatting</returns>
        private Button CreateButton(string strButtonText)
        {
            Button btnNewButtton = new Button
            {
                Content = strButtonText,
                Template = (ControlTemplate)Application.Current.Resources["xtraSmallButton"],
                Margin = new Thickness(5)
            };

            return btnNewButtton;
        }

        /// <summary>
        /// Creates an instance of BackupUtility to select a location to backup to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseBackupLocation_Click(object sender, RoutedEventArgs e)
        {
            BackupUtility backupUtility = new BackupUtility();
            txtBoxFileBackup.Text = backupUtility.ChooseBackupLocation(txtBoxFileBackup.Text); //defaults to the location last backed up to

        }

        /// <summary>
        /// Hides and unhides the UI elements for adding a new field to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddField_Click(object sender, RoutedEventArgs e)
        {
            if (gridAddField.Visibility == Visibility.Hidden)
            {
                gridAddField.Visibility = Visibility.Visible;
                btnAddField.Content = "Cancel";
            }
            else
            {
                gridAddField.Visibility = Visibility.Hidden;
                btnAddField.Content = "Add Field";
            }
        }

        /// <summary>
        /// Saves a new Field to the list when the SaveField button is clicked.
        /// 
        /// TODO: Modify the BtnSaveField_Click to add a new field to the DB.
        /// </summary>
        /// <param name="sender">The button for the Save Field event</param>
        /// <param name="e">The click event</param>
        private void BtnSaveField_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                Business business = new Business();
                business.AddField(dbContext, txtBoxFieldName.Text);
                DisplayEditableFields();
            }

            gridAddField.Visibility = Visibility.Hidden;
            btnAddField.Content = "Add Field";
        }

        /// <summary>
        /// Creates an instance of BackupUtility to restore database from a backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestoreFromBackup_Click(object sender, RoutedEventArgs e)
        {
            BackupUtility backupUtility = new BackupUtility();
            backupUtility.RestoreFromBackup(txtBoxFileBackup.Text); //defaults to the location last backed up to
        }

        /// <summary>
        /// Creates an instance of BackupUtility to create a new backup of the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateBackupNow_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.BackupLocation = txtBoxFileBackup.Text; //persists the chosen location for backups
            Properties.Settings.Default.Save();

            BackupUtility backupUtility = new BackupUtility();
            backupUtility.CreateBackup(txtBoxFileBackup.Text); //default to the persisted location
        }

        /// <summary>
        /// Persists all settings pertaining to backup notification frequency both visually for the user and behind the scenes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveNotificationSettings_Click(object sender, RoutedEventArgs e)
        {
            
            RadioButton radioButton; //used to cast an object iterator to a radiobutton in a for each loop

            // If the notification frequency is weekly iterate through all radio buttons in the group and persist the one that is checked.
            if (cmbNotificationFrequency.SelectedValue.Equals("Weekly"))
            {
                foreach (object dayOfWeek in sPanelRadioButtonForWeekly.Children)
                {
                    if (dayOfWeek is RadioButton)
                    {
                        radioButton = (RadioButton)dayOfWeek;

                        if(radioButton.IsChecked.HasValue && radioButton.IsChecked.Value)
                        {
                            Properties.Settings.Default.DayOfWeek = radioButton.Name;
                            Properties.Settings.Default.Save();
                        }
                    }
                } 
                    
            }
            //If the notification frequency is monthly create the next notification on the first of each month.
            else if (cmbNotificationFrequency.SelectedValue.Equals("Monthly"))
            {
                DateTime dtThisMonth = DateTime.Now; //gets the current date and uses this to find the first day of next month
                Properties.Settings.Default.DayOfMonth = new DateTime(dtThisMonth.Year, dtThisMonth.Month, 1).AddMonths(1); //Next Month on the 1st
            }

            //persist simple settings.
            Properties.Settings.Default.SelectedIndexFreq = cmbNotificationFrequency.SelectedIndex;
            Properties.Settings.Default.Save();

            Properties.Settings.Default.SelectedIndexTime = cmbNotificationTime.SelectedIndex;
            Properties.Settings.Default.Save();

            Properties.Settings.Default.TimeOfDay = DateTime.Parse(cmbNotificationTime.Text);
            Properties.Settings.Default.Save();

            btnSaveNotificationSettings.Visibility = Visibility.Hidden; //make save settings button invisible to the user
        }

        /// <summary>
        /// Every time the user changes the notifictaion frequency display the proper controls for that setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbNotificationFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide the notification time elements if backup frequency is selected to 'None'
            if (cmbNotificationFrequency.SelectedValue.Equals("None"))
            {
                lblNotificationTime.Visibility = Visibility.Collapsed;
                cmbNotificationTime.Visibility = Visibility.Collapsed;
            } 
            else
            {
                lblNotificationTime.Visibility = Visibility.Visible;
                cmbNotificationTime.Visibility = Visibility.Visible;
            }

            // Unhide the Radio Buttons when choosing Weekly for notification frequency
            if (cmbNotificationFrequency.SelectedValue.Equals("Weekly"))
                sPanelRadioButtonForWeekly.Visibility = Visibility.Visible;
            else
                sPanelRadioButtonForWeekly.Visibility = Visibility.Collapsed;

            if (cmbNotificationFrequency.SelectedValue.Equals("Monthly"))
                sPanelMonthly.Visibility = Visibility.Visible;
            else
                sPanelMonthly.Visibility = Visibility.Collapsed;

            btnSaveNotificationSettings.Visibility = Visibility.Visible; //show the save settings button if the user wants to change their settings
        }

        /// <summary>
        /// if the time of day has been changed, show the save settings button if the user wants to change their settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbNotificationTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSaveNotificationSettings.Visibility = Visibility.Visible;
        }
        /// Contains the button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            
            BackgroundWorker bWrk = new BackgroundWorker();

            //String that contains the excel file that the user selects.
            string FilePath = FindFile();

            //Populated members List.
            Members = ReadExcelFile(FilePath);

            List<Members> rgDuplicates = FindDuplicateExcelData(Members);

            if (rgDuplicates.Count > 0)
            {

                foreach (Members duplicate in rgDuplicates)
                {
                    Members currentMember = Members.Find(x => x.gstrBusinessName.Equals(duplicate.gstrBusinessName));
                    if (currentMember != null)
                    {
                        Members.Remove(currentMember);
                        Console.WriteLine(currentMember.gstrBusinessName);
                    }
                }

                NavigationService.Navigate(new DataConflictPage(rgDuplicates));
            }

            try
            {
                //Thread created to import data 
                bWrk.DoWork += LoadHousingForImport;
                bWrk.RunWorkerCompleted += LoadSettingsPage;
                bWrk.RunWorkerAsync();

                if (!(rgDuplicates.Count > 0))
                {
                    //Form to display message to not click through the application during import data.
                    frmLoadPage.TopMost = true;
                    frmLoadPage.MaximizeBox = false;
                    frmLoadPage.MinimizeBox = false;
                    frmLoadPage.ControlBox = false;
                    frmLoadPage.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    frmLoadPage.Show();
                }
            }
            catch
            {
            }


        }

        /// Contains the method needed to start importing data into the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadHousingForImport(Object sender, DoWorkEventArgs e)
        {
            //
            if (Members.Count > 0)
            {
                ImportToDatabase(Members);
            }
        }

        /// Method for when the thread stops to close the form that was previously displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSettingsPage(object sender, RunWorkerCompletedEventArgs e)
        {
            frmLoadPage.Close();
        }

        private void BtnImportPayPal_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FindPayPalFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                List<Transaction> reportItems = new List<Transaction>();

                try
                {
                    StreamReader fileReader = new StreamReader(filePath);

                    string headerLine = fileReader.ReadLine();
                    if(headerLine == null)
                    {
                        MessageBox.Show("The CSV file selected is empty.",
                            "Empty CSV File");
                        return;
                    }
                    string[] columnHeaders = headerLine.Replace("\"", "").Split(',');

                    string line = "";
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        string[] rowData = line.Replace("\"", "").Split(',');
                        Transaction transaction = new Transaction(columnHeaders, rowData);
                        if (!transaction.IsValid)
                        {
                            MessageBox.Show("The data in the CSV file is of an unknown format.",
                                "Invalid CSV File");
                            return;
                        }
                        reportItems.Add(transaction);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Make sure the CSV file is not open in any other programs.",
                            "Unable to open file");
                    return;
                }

                TransactionImportResult result = CsvParser.RunImport(reportItems);
                if(result.Successful())
                {
                    MessageBox.Show(result.ImportSuccesses.Count() +
                        " PayPal transactions successfully imported into Payments.\n"
                        + result.DuplicateCount() + " duplicates were skipped.",
                        "PayPal Import Successful");
                }
                else
                {
                    int duplicateCount = result.DuplicateCount();
                    MessageBox.Show(result.ImportSuccesses.Count()
                        + " PayPal transactions successfully imported into Payments.\n"
                        + (result.ImportFailures.Count() - duplicateCount) + " failed to import.\n"
                        + duplicateCount + " duplicates were not imported.\n\n"
                        + "The importing wizard will now walk through manually importing the failures.",
                        "PayPal Import Needs Attention!");
                    PayPalTransactionImportWizard wizard = new PayPalTransactionImportWizard(result.ImportFailures);
                    wizard.Show();
                }
            }
        }

        /// <summary>
        /// ReadExcelFile method that will read the prior selected excel file. 
        /// </summary>
        /// <param name="filePath">String that contains the name and path of the excel sheet that will be read.</param>
        /// <returns>List that contains Members information from excel sheet.</returns>
        private List<Members> ReadExcelFile(string filePath)
        {
            //Temporary List to build the List that is returned.
            List<Members> TempMembers = new List<Members>();

            if (filePath != "")
            {
                try
                {
                    //Stream that is useed to read the excel file.
                    using (var Stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {

                        //Reader that is used to hold data read from the excel file.
                        using (var Reader = ExcelReaderFactory.CreateReader(Stream))
                        {
                            var dataset = Reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                            int intExtraFieldCount = dataset.Tables[0].Columns.Count - 20;
                            List<string> strlstColumnNames = new List<string>();
                            for (int i = 0; i < intExtraFieldCount; i++)
                            {
                                strlstColumnNames.Add(dataset.Tables[0].Columns[i + 20].ColumnName);
                            }
                            do
                            {
                                while (Reader.Read())
                                {
                                    Dictionary<string, string> extraFieldData = new Dictionary<string, string>();
                                    for (int i = 0; i < intExtraFieldCount; i++)
                                    {
                                        extraFieldData.Add(strlstColumnNames[i], Convert.ToString(Reader[20 + i]));
                                    }
                                    //Add data that was read from the Excel sheet into a list.
                                    TempMembers.Add(new Members(Convert.ToString(Reader[0]), Convert.ToString(Reader[1]), Convert.ToString(Reader[2]),
                                                                Convert.ToString(Reader[3]), Convert.ToString(Reader[4]), Convert.ToString(Reader[5]),
                                                                Convert.ToString(Reader[6]), Convert.ToString(Reader[7]), Convert.ToString(Reader[8]),
                                                                Convert.ToString(Reader[9]), Convert.ToString(Reader[10]), Convert.ToString(Reader[11]),
                                                                Convert.ToString(Reader[12]), Convert.ToString(Reader[13]), Convert.ToString(Reader[14]),
                                                                Convert.ToString(Reader[15]), Convert.ToString(Reader[17]), Convert.ToString(Reader[19]),
                                                                extraFieldData));

                                }
                                //Moves to the next sheet.
                            } while (Reader.NextResult());
                        }
                    }

                    if (filePath != "" && TempMembers.Count < 1)
                    {
                        MessageBox.Show("No Excel Data to import to Database", "Excel Import Notice",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
                    }

                }
                catch (System.IO.IOException)
                {
                    // Alert the user! Let them know the Excel file is open and needs to be clsoed
                    MessageBox.Show("To Import your Excel file, please close the Excel file first", "Excel Import Notice",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch
                {
                    // Alert the user! Let them know the Excel file is open and needs to be clsoed
                    MessageBox.Show("Excel file format does not match. Please check the Excel file", "Excel Import Notice",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }          
            }

            //Returns the temporary list for further use.
            return TempMembers;
        }

 

        public void DisplayEditableFields()
        {
            sPanelFields.Children.Clear();

            using (DatabaseContext dbContext = new DatabaseContext())
            {
                Business business = new Business();
                List<string> rgExtraFields = business.AvailableFields(dbContext);

                foreach (string field in rgExtraFields)
                {
                    StackPanel sPanelTxtBoxAndBtn = CreateStackPanel(Orientation.Horizontal);

                    Button btnEdit = CreateButton("Edit");
                    Button btnDelete = CreateButton("Delete");
                    btnDelete.Visibility = Visibility.Hidden;

                    TextBox txtBoxFieldEdit = CreateTextBox(false);
                    txtBoxFieldEdit.Text = $"{field}";
                    btnEdit.Click += (next_sender, next_e) => SetTextField(next_sender, next_e, txtBoxFieldEdit, btnDelete);
                    btnDelete.Click += (next_sender, next_e) => DeleteTextField(next_sender, next_e, sPanelTxtBoxAndBtn);


                    sPanelTxtBoxAndBtn.Children.Add(txtBoxFieldEdit);
                    sPanelTxtBoxAndBtn.Children.Add(btnEdit);
                    sPanelTxtBoxAndBtn.Children.Add(btnDelete);

                    sPanelFields.Children.Add(sPanelTxtBoxAndBtn);

                }
            }
        }

        /// <summary>
        /// Save button appears when the week day has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnWeekDayChanged(object sender, RoutedEventArgs e)
        {
            btnSaveNotificationSettings.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// FindDuplicateDBData method that will find existing business name in the database from the Excel file
        /// </summary>
        /// <param name="business"> List of all the members read from the Excel file</param>
        /// <returns>List that contains duplicate business information from the database.</returns>
        private List<Business> FindDuplicateDBData(List<Members> business)
        {
            List<Business> DuplicateBusinesses = new List<Business>();

            using (var context = new DatabaseContext())
            {
                for (int intCounter = 0; intCounter < business.Count; intCounter++)
                {
                    List<Business> Business = context.Businesses.Where(strBusinessName => strBusinessName.BusinessName.Equals(business[intCounter].gstrBusinessName)).ToList();

                    if (Business.Count > 0)
                    {
                        DuplicateBusinesses.Add(Business[0]);
                    }
                }
            }
            return DuplicateBusinesses;
        }


        /// <summary>
        /// FindDuplicateExcelData method that will find existing business name in the database from the Excel file
        /// </summary>
        /// <param name="business"> List of all the members read from the Excel file</param>
        /// <returns>List that contains duplicate business information from the Excel.</returns>
        private List<Members> FindDuplicateExcelData(List<Members> business)
        {
            List<Members> DuplicateBusinesses = new List<Members>();

            using (var context = new DatabaseContext())
            {
                for (int intCounter = 0; intCounter < business.Count; intCounter++)
                {
                    List<Business> Business = context.Businesses.Where(strBusinessName => strBusinessName.BusinessName.Equals(business[intCounter].gstrBusinessName)).ToList();

                    if (Business.Count > 0)
                    {
                        DuplicateBusinesses.Add(business[intCounter]);
                    }
                }
            }
            return DuplicateBusinesses;
        }


        /// Take in a List of members to sort and import data into the database
        /// </summary>
        /// <param name="Data">List of opbject members.</param>
        public void ImportToDatabase(List<Members> Data)
        {
            List<string> lstStrFailedDataImports = new List<string>();

            using (var context = new DatabaseContext())
            {

                if (Data.Count > 0)
                {
                    foreach (KeyValuePair<string, string> entry in Data[0].gdctExtraFields)
                    {
                        Business tmpBusiness = new Business();
                        tmpBusiness.AddField(context, entry.Key);
                    }
                    context.SaveChanges();
                }

                for (int intCounter = 1; intCounter < Data.Count - 1; intCounter++)
                {
                    if (Data[intCounter].gstrBusinessName.Equals(""))
                    {
                        break;
                    }

                    try
                    {
                        String strCityStateZip = Data[intCounter].gstrCityStateZip;
                        String strLevelData = Data[intCounter].gstrLevel;
                        String strAdditionalNote = "";

                        String[] arrSplit = strCityStateZip.Split(',');
                        String strCityData = arrSplit[0];

                        String[] arrSplitStateZip = arrSplit[1].Split(' ');
                        String strStateData = arrSplitStateZip[1];
                        String strZipData = arrSplitStateZip[2];

                   
                        Address objMailingAddress = new Address()
                        {
                            StreetAddress = Data[intCounter].gstrMailingAddress,
                            City = strCityData,
                            State = strStateData,
                            ZipCodeExt = strZipData
                        };

                        Address objLocationAddress = new Address()
                        {
                            StreetAddress = Data[intCounter].gstrLocationAddress,
                            City = strCityData,
                            State = strStateData,
                            ZipCodeExt = strZipData
                        };

                        if (strZipData.Length > 5)
                        {
                            String[] arrZipSplit = strZipData.Split('-');

                            objMailingAddress.ZipCode = Int32.Parse(arrZipSplit[0]);
                            objLocationAddress.ZipCode = Int32.Parse(arrZipSplit[0]);
                            objMailingAddress.ZipCodeExt = strZipData;
                            objLocationAddress.ZipCodeExt = strZipData;
                        }
                        else
                        {
                            objMailingAddress.ZipCode = Int32.Parse(strZipData);
                            objLocationAddress.ZipCode = Int32.Parse(strZipData);
                            objMailingAddress.ZipCodeExt = strZipData;
                            objLocationAddress.ZipCodeExt = strZipData;
                        };

                        String strLocationAddress = Data[intCounter].gstrLocationAddress;
                        String[] arrLocationSplit = strLocationAddress.Split(',');
                        String[] arrLocationSplitSlash = strLocationAddress.Split('/');
                        String[] arrLocationSplitAnd = strLocationAddress.Split('&');

                        if (arrLocationSplit.Length == 3)
                        {
                            String[] arrStateZip = arrLocationSplit[2].Split(' ');

                            strLocationAddress = arrLocationSplit[1] + arrLocationSplit[2];
                            objLocationAddress.StreetAddress = arrLocationSplit[0];
                            objLocationAddress.City = arrLocationSplit[1];
                            objLocationAddress.State = arrStateZip[1];
                            objLocationAddress.ZipCode = Int32.Parse(arrStateZip[2]);
                            objLocationAddress.ZipCodeExt = strZipData;

                        }
                        else if (arrLocationSplitSlash.Length > 1)
                        {
                            objLocationAddress.StreetAddress = arrLocationSplit[0];
                            strAdditionalNote = " - Additional Location Addresses : ";

                            for (int intCount = 1; intCount < arrLocationSplitSlash.Length; intCount++)
                            {
                                strAdditionalNote = strAdditionalNote + "  " + arrLocationSplitSlash[intCount] + " " + strCityStateZip;
                            }
                        }

                        arrLocationSplit = strLocationAddress.Split('&');

                        if (arrLocationSplit.Length > 1)
                        {
                            objLocationAddress.StreetAddress = arrLocationSplit[0];

                            strAdditionalNote = " - Additional Location Addresses : ";

                            for (int intCount = 1; intCount < arrLocationSplit.Length; intCount++)
                            {
                                arrLocationSplitSlash = arrLocationSplit[intCount].Split(',');

                                if (arrLocationSplitSlash.Length > 1)
                                {
                                    strAdditionalNote = strAdditionalNote + "  " + arrLocationSplit[intCount];

                                }
                                else
                                {
                                    strAdditionalNote = strAdditionalNote + "  " + arrLocationSplit[intCount] + " " + strCityStateZip;
                                }

                            }
                        }

                        context.Addresses.Add(objMailingAddress);
                        context.Addresses.Add(objLocationAddress);

                        Business objBusiness = new Business()
                        {
                            BusinessName = Data[intCounter].gstrBusinessName,
                            Website = Data[intCounter].gstrWebsiteAddress,
                            ExtraNotes = Data[intCounter].gstrNotes + " " + strAdditionalNote,
                            MailingAddress = objMailingAddress,
                            PhysicalAddress = objLocationAddress

                        };
                        if (Data[intCounter].gstrEstablished.Equals(""))
                        {
                            objBusiness.YearEstablished = 0;
                        }
                        else
                        {
                            objBusiness.YearEstablished = Int32.Parse(Data[intCounter].gstrEstablished);
                        }

                        if (strLevelData.Equals("Gold"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.GOLD;
                        }
                        else if (strLevelData.Equals("Silver"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.SILVER;
                        }
                        else if (strLevelData.Equals("Associate"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.ASSOCIATE;
                        }
                        else if (strLevelData.Equals("Individual"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.INDIVIDUAL;
                        }
                        else if (strLevelData.Equals("Courtesy"))
                        {
                            objBusiness.MembershipLevel = MembershipLevel.COURTESY;
                        }
                        foreach (KeyValuePair<string, string> extraField in Data[intCounter].gdctExtraFields) {
                            objBusiness.SetField(extraField.Key, extraField.Value, context);
                        }

                        context.Businesses.Add(objBusiness);

                        arrLocationSplit = Data[intCounter].gstrContactPerson.Split('&');
                        arrLocationSplitSlash = Data[intCounter].gstrContactPerson.Split('/');
                        arrLocationSplitAnd = Data[intCounter].gstrPhoneNumber.Split('/');
                        arrSplit = Data[intCounter].gstrEmailAddress.Split(',');

                        if (arrLocationSplit.Length > 1)
                        {
                            for (int intCount = 0; intCount < arrLocationSplit.Length; intCount++)
                            {
                                ContactPerson objContactPerson = new ContactPerson()
                                {
                                    Name = arrLocationSplit[intCount].Trim()
                                };

                                context.ContactPeople.Add(objContactPerson);

                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrPhoneNumber,
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);

                                if (Data[intCounter].gstrFaxNumber.Length > 1)
                                {
                                    PhoneNumber objFaxNumber = new PhoneNumber()
                                    {
                                        ContactPerson = objContactPerson,
                                        Number = Data[intCounter].gstrFaxNumber,
                                        GEnumPhoneType = PhoneType.Fax
                                    };

                                    context.PhoneNumbers.Add(objFaxNumber);
                                }

                                BusinessRep objBusinessRep = new BusinessRep()
                                {
                                    Business = objBusiness,
                                    ContactPerson = objContactPerson
                                };

                                context.BusinessReps.Add(objBusinessRep);

                                for (int intCountEmail = 0; intCountEmail < arrSplit.Length; intCountEmail++)
                                {

                                    Email objEmail = new Email()
                                    {
                                        ContactPerson = objContactPerson,
                                        EmailAddress = arrSplit[intCountEmail]
                                    };

                                    context.Emails.Add(objEmail);
                                }
                            }
                        }
                        else if (arrLocationSplitSlash.Length > 1 && arrLocationSplitAnd.Length > 1)
                        {
                            for (int intCount = 0; intCount < arrLocationSplitSlash.Length; intCount++)
                            {
                                ContactPerson objContactPerson = new ContactPerson()
                                {
                                    Name = arrLocationSplitSlash[intCount].Trim()
                                };

                                context.ContactPeople.Add(objContactPerson);

                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = arrLocationSplitAnd[intCount],
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);

                                if (Data[intCounter].gstrFaxNumber.Length > 1)
                                {
                                    PhoneNumber objFaxNumber = new PhoneNumber()
                                    {
                                        ContactPerson = objContactPerson,
                                        Number = Data[intCounter].gstrFaxNumber,
                                        GEnumPhoneType = PhoneType.Fax
                                    };
                                    context.PhoneNumbers.Add(objFaxNumber);
                                }

                                BusinessRep objBusinessRep = new BusinessRep()
                                {
                                    Business = objBusiness,
                                    ContactPerson = objContactPerson
                                };

                                context.BusinessReps.Add(objBusinessRep);

                                for (int intCountEmail = 0; intCountEmail < arrSplit.Length; intCountEmail++)
                                {

                                    Email objEmail = new Email()
                                    {
                                        ContactPerson = objContactPerson,
                                        EmailAddress = arrSplit[intCountEmail]
                                    };

                                    context.Emails.Add(objEmail);
                                }
                            }
                        }
                        else if(Data[intCounter].gstrContactPerson.Length > 0)
                        {

                            ContactPerson objContactPerson = new ContactPerson()
                            {
                                Name = Data[intCounter].gstrContactPerson.Trim()
                            };

                            context.ContactPeople.Add(objContactPerson);


                            for (int intCount = 0; intCount < arrLocationSplitAnd.Length; intCount++)
                            {
                                PhoneNumber objPhoneNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrPhoneNumber,
                                    GEnumPhoneType = PhoneType.Office
                                };

                                context.PhoneNumbers.Add(objPhoneNumber);
                            }
                            if (Data[intCounter].gstrFaxNumber.Length > 1) 
                            {
                                PhoneNumber objFaxNumber = new PhoneNumber()
                                {
                                    ContactPerson = objContactPerson,
                                    Number = Data[intCounter].gstrFaxNumber,
                                    GEnumPhoneType = PhoneType.Fax
                                };

                                context.PhoneNumbers.Add(objFaxNumber);
                            }

                            BusinessRep objBusinessRep = new BusinessRep()
                            {
                                Business = objBusiness,
                                ContactPerson = objContactPerson
                            };

                            context.BusinessReps.Add(objBusinessRep);

                            for (int intCount = 0; intCount < arrSplit.Length; intCount++)
                            {

                                Email objEmail = new Email()
                                {
                                    ContactPerson = objContactPerson,
                                    EmailAddress = arrSplit[intCount]
                                };

                                context.Emails.Add(objEmail);
                            }
                        }

                        DateTime strCurrentYear = DateTime.Now;

                        YearlyData objYearlyData = new YearlyData
                        {
                            Year = Int32.Parse(strCurrentYear.Year.ToString()),
                            Business = objBusiness,
                        };

                        if (Data[intCounter].gstrCredit.Equals(""))
                        {
                            objYearlyData.Credit = 0;
                        }
                        else
                        {
                            objYearlyData.Credit = Double.Parse(Data[intCounter].gstrCredit);
                        }

                        if (Data[intCounter].gstrDuesPaid.Equals(""))
                        {
                            objYearlyData.DuesPaid = 0;
                        }
                        else
                        {
                            objYearlyData.DuesPaid = Double.Parse(Data[intCounter].gstrDuesPaid);
                        }
                        if (Data[intCounter].gstrRaffleTicketReturnedPaid.Equals(""))
                        {
                            objYearlyData.TicketsReturned = 0;
                        }
                        else
                        {
                            objYearlyData.TicketsReturned = Double.Parse(Data[intCounter].gstrRaffleTicketReturnedPaid);
                        }
                        if (Data[intCounter].gstrBallot.Equals(""))
                        {
                            objYearlyData.BallotNumber = 0;
                        }
                        else
                        {
                            objYearlyData.BallotNumber = Int32.Parse(Data[intCounter].gstrBallot);
                        }

                        if (Data[intCounter].gstrTerms.Equals("Annually"))
                        {
                            objYearlyData.TermLength = TermLength.Annually;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Semiannually"))
                        {
                            objYearlyData.TermLength = TermLength.Semiannually;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Quarterly"))
                        {
                            objYearlyData.TermLength = TermLength.Quarterly;
                        }
                        else if (Data[intCounter].gstrTerms.Equals("Monthly"))
                        {
                            objYearlyData.TermLength = TermLength.Monthly;
                        }

                        context.BusinessYearlyData.Add(objYearlyData);

                        context.SaveChanges();                    
                    }
                    catch
                    {
                        lstStrFailedDataImports.Add(Data[intCounter].gstrBusinessName);

                    }                   
                }
            }

            string strMessage = string.Join(Environment.NewLine, lstStrFailedDataImports);

            if (strMessage.Length > 0) {
                frmLoadPage.Close();
                MessageBox.Show("Excel Import Failed. \n" + " \n" + "The failed businesses were: \n" + " \n" + strMessage, "Excel Import Notice",
                       MessageBoxButton.OK,
                       MessageBoxImage.Information);

            }
        }

        /// <summary>
        /// FindFile method that allows the user to select the file they would like to be read into the application.
        /// </summary>
        /// <returns>String that contains the file path.</returns>
        private string FindFile()
        {
            string FileName = "";
            string FileExtension = "";

            OpenFileDialog FileDialog = new OpenFileDialog();

            FileDialog.Title = "Excel File Dialog";
            FileDialog.InitialDirectory = @"c:\";
            FileDialog.Filter = "Excel Files|*.xls;*.xlsx;";
            FileDialog.FilterIndex = 2;
            FileDialog.RestoreDirectory = true;
            FileDialog.ShowDialog();


            FileExtension = Path.GetExtension(FileDialog.FileName);

            if (FileExtension.CompareTo(".xls") == 0 || FileExtension.CompareTo(".xlsx") == 0)
            {
                FileName = FileDialog.FileName;

            }
            else if (FileExtension.Equals(""))
            {

            }
            else
            {
                //Messagebox that prompts user to select the correct file type.
                MessageBox.Show("Please Select an valid Excel file.", "Excel Import Notice",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            }

            return FileName;
        }

        /// <summary>
        /// FindPayPalFile method that allows the user to select the file they would like to be read into the application for PayPal transactions.
        /// </summary>
        /// <returns>String that contains the file path.</returns>
        private string FindPayPalFile()
        {
            string FileName = "";
            string FileExtension = "";

            OpenFileDialog FileDialog = new OpenFileDialog();

            FileDialog.Title = "Select PayPal Export File...";
            FileDialog.InitialDirectory = @"c:\";
            FileDialog.Filter = "CSV Files|*.csv;";
            FileDialog.RestoreDirectory = true;
            FileDialog.ShowDialog();


            FileExtension = Path.GetExtension(FileDialog.FileName);

            if (FileExtension.ToLower() == ".csv")
            {
                FileName = FileDialog.FileName;
            }
            else
            {
                //Messagebox that prompts user to select the correct file type.
                MessageBox.Show("Please Select an valid CSV file.", "Import Notice",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            }

            return FileName;
        }

        /// NOT FOR PRODUCTION:
        /// 
        /// Allows developers to trigger the Data Import Conflict screen while under
        /// development.
        /// 
        /// </summary>
        /// <param name="sender">The 'Simulate Import Conflict' button</param>
        /// <param name="e">The Click Event</param>
        private void btnSimulateConflict_Click(object sender, RoutedEventArgs e)
        {
            // NavigationService.Navigate(new DataConflictPage());
        }

    }
}


/// <summary>
/// Class Members that contains the data fields for members.
/// </summary>
public class Members
{
    public string gstrEstablished { get; }
    public string gstrLevel { get; }
    public string gstrBusinessName { get; }
    public string gstrMailingAddress { get; }
    public string gstrLocationAddress { get; }
    public string gstrCityStateZip { get; }
    public string gstrContactPerson { get; }
    public string gstrPhoneNumber { get; }
    public string gstrFaxNumber { get; }
    public string gstrEmailAddress { get; }
    public string gstrWebsiteAddress { get; }
    public string gstrDuesPaid { get; }
    public string gstrRaffleTicketReturnedPaid { get; }
    public string gstrCredit { get; }
    public string gstrTerms { get; }
    public string gstrNotes { get; }
    public string gstrFreeWebAd { get; }
    public string gstrBallot { get; }
    public Dictionary<string, string> gdctExtraFields { get; }

    public Members(string strEstablished, string strLevel, string strBusinessName, string strMailingAddress,
                   string strLocationAddress, string strCityStateZip, string strContactPerson, string strPhoneNumber,
                   string strFaxNumber, string strEmailAddress, string strWebsiteAddress, string strDuesPaid, string strRaffleTicketReturnedPaid,
                   string strCredit, string strTerms, string strNotes, string strFreeWebAd, string strBallot, Dictionary<string, string> extraFieldData)
    {
        this.gstrEstablished = strEstablished;
        this.gstrLevel = strLevel;
        this.gstrBusinessName = strBusinessName;
        this.gstrMailingAddress = strMailingAddress;
        this.gstrLocationAddress = strLocationAddress;
        this.gstrCityStateZip = strCityStateZip;
        this.gstrContactPerson = strContactPerson;
        this.gstrPhoneNumber = strPhoneNumber;
        this.gstrFaxNumber = strFaxNumber;
        this.gstrEmailAddress = strEmailAddress;
        this.gstrWebsiteAddress = strWebsiteAddress;
        this.gstrDuesPaid = strDuesPaid;
        this.gstrRaffleTicketReturnedPaid = strRaffleTicketReturnedPaid;
        this.gstrCredit = strCredit;
        this.gstrTerms = strTerms;
        this.gstrNotes = strNotes;
        this.gstrFreeWebAd = strFreeWebAd;
        this.gstrBallot = strBallot;
        this.gdctExtraFields = extraFieldData;
    }

    public Members()
    {
        this.gstrEstablished = "";
        this.gstrLevel = "";
        this.gstrBusinessName = "";
        this.gstrMailingAddress = "";
        this.gstrLocationAddress = "";
        this.gstrCityStateZip = "";
        this.gstrContactPerson = "";
        this.gstrPhoneNumber = "";
        this.gstrFaxNumber = "";
        this.gstrEmailAddress = "";
        this.gstrWebsiteAddress = "";
        this.gstrDuesPaid = "";
        this.gstrRaffleTicketReturnedPaid = "";
        this.gstrCredit = "";
        this.gstrTerms = "";
        this.gstrNotes = "";
        this.gstrFreeWebAd = "";
        this.gstrBallot = "";
        this.gdctExtraFields = new Dictionary<string, string>();
    }
}
