using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using DirectorPortalDatabase;
using System.IO;
using System.Windows;
using DirectorPortalDatabase.Models;
using System.Threading;


/// <summary>
/// 
/// File Name: BackupUtility.cs
/// 
/// Part of Project: DirectorsPortal
/// 
/// Original Author: Kaden M. Thompson
/// 
/// File Purpose:
///     This file is used for managing backups of the database, allowing the user to dictate where to save a backup to
///     or where the user would like to pull a backup from. 
///     
///     Additionally, this class contains a timer that runs on a thread to check every X amount of time if a new backup notification
///     should be created based on the current time and the time that is persisted in the database.
///      
/// </summary>

namespace DirectorsPortalWPF.SettingsUI
{
    /// <summary>
    /// Provides a front end for picking backup locations
    /// </summary>
    class BackupUtility
    {



        static Timer timer; //reference to the timer that is created, global so the garbage collector doesn't wipe out the timer
        const int intNOTIFICATION_POLLING_TIME_IN_SECONDS = 15; //timer checks every 15 seconds to see if there a new notificaiton to be made
        DatabaseContext dbContext = new DatabaseContext(); //reference to the db context for updating the database with TODOs
        Todo tdoNotificationSetting; //used for creating new TODOs
        const string strFilename = "directors_portal.db";

        /// <summary>
        /// enum for readability of notification frequency selected by the user
        ///    None - 0
        ///   Daily - 1
        ///  Weekly - 2
        /// Monthly - 3
        /// </summary>
        enum NotificationFrequency
        {
            None,   //0
            Daily,  //1
            Weekly, //2
            Monthly //3
        }

        /// <summary>
        /// Allows the user to select a folder on their system to be the path for the backup database file.
        /// </summary>
        /// <returns>A string containing the path the user choose to save database to</returns>
        /// <param name="strTargetPath">holds a path that has been persisted from the settings page</param>
        public String ChooseBackupLocation(string strTargetPath)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog(); //opens the file browser
            commonOpenFileDialog.IsFolderPicker = true;

            //checks to see if there is a backup location persisted or not
            if (strTargetPath.Length > 0)
            {
                commonOpenFileDialog.InitialDirectory = strTargetPath;
            }
            else
            {
                commonOpenFileDialog.InitialDirectory = "C:\\";
            }

            //returns the path chosen by the user
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return commonOpenFileDialog.FileName;
            }
            else //if something fails return default path
            {
                return "C:\\";
            }
        }

        /// <summary>
        /// Allows the user to select a database file to restore from
        /// </summary>
        /// <param name="strTargetPath">holds a path that has been persisted from the settings page</param>
        public void RestoreFromBackup(string strTargetPath)
        {
            string strBackupFilePath; //holds the path the user selected in the file browser

            //creates the destination file path and file name that will replace the current database
            string strDestFile = Path.Combine(DatabaseContext.GetFolderPath(), strFilename);

            //create a file dialog that restricts users to only database files.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Database files (*.db)|*.db",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            //checks to see if there is a backup location persisted or not
            if (strTargetPath.Length > 0)
            {
                openFileDialog.InitialDirectory = strTargetPath;
            }
            else
            { 
                openFileDialog.InitialDirectory = "C:\\";
            }

            //when the user selects the file they want to backup with perform the file replacement and notify the user
            if (openFileDialog.ShowDialog() == true)
            {
                strBackupFilePath = openFileDialog.FileName;
                File.Copy(strBackupFilePath, strDestFile, true);
                MessageBox.Show("Database restored from backup");
            }
        }

        /// <summary>
        /// Creates a copy from the database that is stored in the AppData>roaming folder and places it where the user chooses
        /// Database backups include the data and time so the user can tell what backups are from when.
        /// finally checks to make sure the file was actually created
        /// </summary>
        /// <param name="strTargetPath">holds a path that has been persisted from the settings page</param>
        public void CreateBackup(string strTargetPath)
        {
            string strSourcePath = DatabaseContext.GetFolderPath(); //gets the path of the current database
            
            //strings to hold path names and file names
            string strBackupFileName;
            string strSourcePathAndFile;
            string strDestPathAndFile;

            //check to make sure there is a path chosen...
            if (strTargetPath.Length == 0)
            {
                MessageBox.Show("Please choose a path first");
            }
            else // if there is a path then...
            {
                //creates a backup file name containing the date
                strBackupFileName = "DB_Backup " + ToSafeFileName(DateTime.Now.ToString()) + ".db";

                //get the path of the source and the path of the destination
                strSourcePathAndFile = Path.Combine(strSourcePath, strFilename);
                strDestPathAndFile = Path.Combine(strTargetPath, strBackupFileName);

                //create the backup, dont overwrite old backups
                File.Copy(strSourcePathAndFile, strDestPathAndFile, false);

                //checks to make sure the file was created
                if (File.Exists(strDestPathAndFile))
                {
                    MessageBox.Show("File saved successfully");

                }
            }

        }

        /// <summary>
        /// Removes colons and forward slashes created by the backup file name when assigning the current data and time to it
        /// </summary>
        /// <param name="strFileNameToClean">
        /// File name with illegal characters</param>
        /// <returns>
        /// File name with illegal characters removed
        /// </returns>
        public string ToSafeFileName(string strFileNameToClean)
        {
            return strFileNameToClean
                .Replace(":", "_")
                .Replace("/", "_");
        }

        /// <summary>
        /// creates a timer that will check every X amount of time to see if there is a backup to be made
        /// </summary>
        public void CheckBackupNotification()
        {

            int intNotificationFrequency; //stores the value of what notification frequency was selected by the user

            //timer variables for length and period
            var StartTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(intNOTIFICATION_POLLING_TIME_IN_SECONDS); //period set by global constant

            //create timer on a thread
            timer = new Timer((e) =>
            {
                //pull data from the settings
                intNotificationFrequency = Properties.Settings.Default.SelectedIndexFreq;
                switch (intNotificationFrequency) //execute the proper notification based on what the user selected
                {
                    case (int)NotificationFrequency.None:
                        break;
                    case (int)NotificationFrequency.Daily:
                        CreateNotification("Daily"); //no other checks are needed just check the time of day
                        break;
                    case (int)NotificationFrequency.Weekly:
                        WeeklyNotification();
                        break;
                    case (int)NotificationFrequency.Monthly:
                        MonthlyNotifcation();
                        break;
                }

            }, null, StartTimeSpan, periodTimeSpan);
        }

        /// <summary>
        /// Checks the time of day and creates a notification if current time has elapsed the time set by the user
        /// </summary>
        /// <param name="strTodoFreqTitle">Title to name the bacup notification for the user</param>
        private void CreateNotification(String strTodoFreqTitle)
        {
            DateTime dtCurrentTime = DateTime.Now;
            DateTime dtTimeToCheck = Properties.Settings.Default.TimeOfDay;

            //if the time of day is greater than the time to check for the notification...
            if (DateTime.Compare(dtCurrentTime, dtTimeToCheck) >= 0)
            {
                Properties.Settings.Default.TimeOfDay = DateTime.Now.AddDays(1); //advance next notification to tomorrow
                Properties.Settings.Default.Save();

                //crate the notification based on what frequency was chosen by the user
                tdoNotificationSetting = new Todo
                {
                    Title = strTodoFreqTitle + " Backup",
                    Description = "You have not made your " + strTodoFreqTitle.ToLower() + " backup"
                };

                //add TODO to the database.
                dbContext.TodoListItems.Add(tdoNotificationSetting);
                dbContext.SaveChanges();

                if(strTodoFreqTitle.Equals("Monthly")) //if this was called from month, then advance the notification to next month
                {
                    Properties.Settings.Default.DayOfMonth = Properties.Settings.Default.DayOfMonth.AddMonths(1);

                }
            }
        }

        /// <summary>
        /// Checks to see if it is time to make a new weekly notification
        /// </summary>
        private void WeeklyNotification()
        {

            DayOfWeek dtDayOfWeek = new DayOfWeek();
            String strDayOfNotification = Properties.Settings.Default.DayOfWeek;

            //checks if today is the day to create a new weekly notification
            if (dtDayOfWeek.ToString().Equals(strDayOfNotification.Replace("rbtn", ""))) //remove the rbtn from the name stored in the settings
            {
                CreateNotification("Weekly"); //create notifiation with title "Weekly"
            }
        }

        /// <summary>
        /// Checks to see if it is time to make a new monthly notification
        /// </summary>
        private void MonthlyNotifcation()
        {
            DateTime dtCurrentDay = DateTime.Now; //DateTime of today
            DateTime dtDayOfNotification = Properties.Settings.Default.DayOfMonth;//DateTime of next notification

            //compares two DateTime variable
            if(dtCurrentDay >= dtDayOfNotification)
            {
                CreateNotification("Monthly"); //create notifiation with title "Monthly"
            }
        }

    }

}

