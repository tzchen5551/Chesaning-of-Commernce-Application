using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// File Purpose:
///     This file is responsible for displaying a preview of the Membership page that will be 
///     uploaded to the Chesaning Chamber of Commerce website. Responsibilities of the file include:
///         - Generating the HTML preview
///         - Refreshing the HTML preview
///         - Copying HTML content to the Clipboard
///         
/// </summary>
namespace DirectorsPortalWPF.ValidateWebsite
{
    /// <summary>
    /// Interaction logic for WebsitePreviewPage.xaml
    /// </summary>
    public partial class WebsitePreviewPage : Page
    {
        // Preview Generator for the HTML template, responsible for loading the 
        // HTML template with latest membership data
        private HtmlPreviewGenerator GHpgPreview;

        /// <summary>
        /// Initialization of the Validate Webpage screen. Preview of the HTML content is generated with latest
        /// data from the database and is displayed on screen.
        /// </summary>
        public WebsitePreviewPage()
        {
            InitializeComponent();
            btnViewInWeb.IsEnabled = false;
            btnCopyContent.IsEnabled = false;
        }

        /// <summary>
        /// Updates the Web Form on the Validate Website screen to the latest HTML content for the 
        /// Member details. Used by a background worker
        /// </summary>
        /// <param name="sender">The background worker</param>
        /// <param name="e">The arguments for when background worker completes work</param>
        private void UpdateWebForm(object sender, RunWorkerCompletedEventArgs e)
        {
            frmValidateWebpage.Source = new Uri(GHpgPreview.GetTemplateLocation());
            btnRefreshValWeb.Content = "Refresh";
            btnRefreshValWeb.Width = 60;

            btnViewInWeb.IsEnabled = true;
            btnCopyContent.IsEnabled = true;
            btnRefreshValWeb.IsEnabled = true;
        }

        /// <summary>
        /// Refreshes the HTML content to the latest details for Chamber Members. Used by the 
        /// background worker.
        /// </summary>
        /// <param name="sender">The background worker</param>
        /// <param name="e">'DoWork' arguments that are used while the background worker is actively doing work.</param>
        private void RefreshPreview(Object sender, DoWorkEventArgs e)
        {
            try
            {
                GHpgPreview = new HtmlPreviewGenerator();
                GHpgPreview.GeneratePreview();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Opens a pop-up window that displays the current frames help information. 
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">The Click event</param>
        public void HelpButtonHandler(object sender, EventArgs e)
        {
            HelpUI.HelpScreenWindow window = new HelpUI.HelpScreenWindow();
            window.Show();
            window.tabs.SelectedIndex = 5;
        }

        /// <summary>
        /// When the 'Copy to Clipboard' button is selected. The HTML template is read and all HTML 
        /// content is copied to the clipboard. From there the user can paste the HTML into Weebly using an
        /// 'Embed Code' object.
        /// </summary>
        /// <param name="sender">The 'Copy to Clipboard' button</param>
        /// <param name="e">The Click Event</param>
        private void BtnCopyContent_Click(object sender, RoutedEventArgs e)
        {
            // StreamReader for reading the HTML File
            using (StreamReader strReader = new StreamReader(GHpgPreview.GetTemplateLocation()))
            {
                string strHTML = strReader.ReadToEnd();

                Console.WriteLine(strReader.ReadToEnd());
                Clipboard.SetText(strHTML);             // Save HTML content to Clipboard

            }

            // Alert the user! Let them know the copy has been completed.
            MessageBox.Show("Updated Webpage Content Copied to Clipboard!",
                "Copied to Clipboard",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// When the 'Preview in Web Browser' button is selected. The HTML preview will open in the
        /// default browser set by the operating system. This is useful since the Frame built into the app cannot render all
        /// HTML styling and some JavasScript. The Web Browser preview is a true display of content that is viewed.
        /// </summary>
        /// <param name="sender">The 'Preview in Web Browser' button</param>
        /// <param name="e">The Click Event</param>
        private void BtnViewInWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(GHpgPreview.GetTemplateLocation());        // Open preview in default web browser...
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);                         // In case no web browser is installed...
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);                                 // Any other error...
            }           
        }

        /// <summary>
        /// When the 'Refresh' button is selected. The HTML preview is refreshed with the latest data
        /// from the database.
        /// </summary>
        /// <param name="sender">The 'Refresh' button</param>
        /// <param name="e">The Click Event</param>
        private void BtnRefreshValWeb_Click(object sender, RoutedEventArgs e)
        {
            btnViewInWeb.IsEnabled = true;
            btnCopyContent.IsEnabled = true;

            btnRefreshValWeb.Content = "Refreshing...";
            btnRefreshValWeb.Width = 100;

            btnViewInWeb.IsEnabled = false;
            btnCopyContent.IsEnabled = false;
            btnRefreshValWeb.IsEnabled = false;

            BackgroundWorker bWrk = new BackgroundWorker();

            bWrk.DoWork += RefreshPreview;
            bWrk.RunWorkerCompleted += UpdateWebForm;

            bWrk.RunWorkerAsync();

        }
    }
}
