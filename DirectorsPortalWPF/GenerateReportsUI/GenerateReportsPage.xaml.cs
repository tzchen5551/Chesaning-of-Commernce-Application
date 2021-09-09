using System;
using System.Windows.Controls;
using ExcelDataReader;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DirectorPortalDatabase.Utility;
using ClosedXML.Excel;
using System.Diagnostics;

namespace DirectorsPortalWPF.GenerateReportsUI
{
    /// <summary>
    /// Interaction logic for GenerateReportsPage.xaml
    /// </summary>
    public partial class GenerateReportsPage : Page
    {
        /// <summary>
        /// A holder of metadata on both a model and one of its properties.
        /// </summary>
        private class ModelAndField
        {
            public MetadataHelper.ModelInfo UdtModelInfo { get; set; }
            public FieldHelper.IDataField UdtDataField { get; set; }
            public ModelAndField(MetadataHelper.ModelInfo udtModelInfo, FieldHelper.IDataField udtTableField)
            {
                UdtModelInfo = udtModelInfo;
                UdtDataField = udtTableField;
            }

            /// <summary>
            /// For other ModelAndField instances, compares both the model info and the table field name.
            /// For all other types, returns false.
            /// </summary>
            /// <param name="objOther"></param>
            /// <returns></returns>
            public override bool Equals(object objOther)
            {
                Type typeOtherType = objOther.GetType();
                if (typeOtherType == typeof(ModelAndField))
                {
                    ModelAndField udtOther = (ModelAndField)objOther;
                    return this.UdtDataField.StrPropertyName == udtOther.UdtDataField.StrPropertyName
                        && this.UdtModelInfo.Equals(udtOther.UdtModelInfo);
                }
                else
                {
                    return false;
                }
            }
        }

        private MetadataHelper.ModelInfo GUdtSelectedReportType { get; set; }
        private ComboBoxItem[] GRGReportTypeItems { get; set; }
        private List<string[]> GRGCurrentReport { get; set; }
        private List<ReportTemplate> GRGReportTemplates { get; set; }

        private int intKeyForExport = 0;



        public void ReportTypeSelectedHandler(object sender, EventArgs e)
        {
            // Gets the selected report type name from the combo box.
            ComboBoxItem cbiSelectedReportTypeItem = (ComboBoxItem)cboReportType.SelectedItem;
            // Extracts the model information from the ComboBoxItem.
            GUdtSelectedReportType = (MetadataHelper.ModelInfo)cbiSelectedReportTypeItem.Tag;
            if (GUdtSelectedReportType != null)
            {
                int intNumberOfFields = GUdtSelectedReportType.UdtTableMetaData.IntNumberOfFields;
                ListBoxItem[] rgFieldItems = new ListBoxItem[intNumberOfFields];

                // Iterates over the built-in fields of the selected table.
                for (int i = 0; i < intNumberOfFields; i++)
                {
                    // Stores the i-th built-in field's information in a new ListBoxItem.
                    FieldHelper.IDataField udtField = GUdtSelectedReportType.UdtTableMetaData.GetField(i);
                    ListBoxItem lbiFieldItem = new ListBoxItem();
                    lbiFieldItem.Content = udtField.StrHumanReadableName;
                    lbiFieldItem.Tag = udtField;

                    // Stores the new ListBoxItem in the array.
                    rgFieldItems[i] = lbiFieldItem;
                }

                // Displays the table's fields in the listbox.
                lstReportFields.ItemsSource = rgFieldItems;
            }
        }

        /// <summary>
        /// Draws the report in the UI.
        /// </summary>
        private void RenderReport()
        {
            // Clears the existing grid content.
            grdReportContent.Children.Clear();
            grdReportContent.RowDefinitions.Clear();
            grdReportContent.ColumnDefinitions.Clear();

            var wbWorkbook = new XLWorkbook();
            wbWorkbook.AddWorksheet("sheetName");
            var wsSheet = wbWorkbook.Worksheet("sheetName");


            int intRowCount = GRGCurrentReport.Count;
            List<int> lstColumnSize = new List<int>();

            //Loop to insert data into excel file.
            for (int i = 0; i < GRGCurrentReport.Count; i++)
            {
                for (int j = 0; j < GRGCurrentReport[i].Length; j++)
                {
                    //If value is null then skip
                    if (GRGCurrentReport[i].GetValue(j) == null)
                    {
                        break;
                    }
                    else
                    {
                        //Adds first value into list
                        if (i == 0)
                        {
                            int intValueSize = GRGCurrentReport[i].GetValue(j).ToString().Length;
                            lstColumnSize.Add(intValueSize);
                        }

                        //If new value is larger than initial value then sets larger value to new value.
                        if (lstColumnSize[j] < GRGCurrentReport[i].GetValue(j).ToString().Length)
                        {
                            lstColumnSize[j] = GRGCurrentReport[i].GetValue(j).ToString().Length;

                        }
                    }

                }
            }

            if (intRowCount > 0)
            {
                int intColCount = GRGCurrentReport[0].Length;

/*                for (int i = 0; i < intColCount; i++)
                {
                    grdReportContent.ColumnDefinitions.Add(new ColumnDefinition());
                }
                for (int i = 0; i < intRowCount; i++)
                {
                    grdReportContent.RowDefinitions.Add(new RowDefinition());
                }*/
                for (int i = 0; i < intRowCount; i++)
                {
                    for (int j = 0; j < intColCount; j++)
                    {
                        // Stores the report cell data in a TextBox, which is inserted into the Grid.
                        /*  TextBox txtReportCell = new TextBox();
                          txtReportCell.IsReadOnly = true;
                          txtReportCell.Text = GRGCurrentReport[i][j];
                          txtReportCell.SetValue(Grid.RowProperty, i);
                          txtReportCell.SetValue(Grid.ColumnProperty, j);
                          grdReportContent.Children.Add(txtReportCell);*/
                        //For the first row column size is set for excel columns
                        if (i == 0)
                        {
                            //Sets column size
                            wsSheet.Cell(i + 1, j + 1).WorksheetColumn().Width = lstColumnSize[j];
                        }
                        wsSheet.Cell(i + 1, j + 1).Value = GRGCurrentReport[i][j];
                    }
                }
            }
            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\Director's Portal Report.xlsx";
            try
            {
                wbWorkbook.SaveAs(strFname);

                Process.Start(strFname);
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("The process cannot access the file"))
                {
                    MessageBox.Show("Please close the current Excel report before generating a new report", "Alert");
                    Console.WriteLine(ex);
                }
                else
                {
                    MessageBox.Show($"An error occured when attempting to generate the report: {ex}");
                    Console.WriteLine(ex);
                }
            }


        }

        /// <summary>
        /// Draws the list of report templates in the UI.
        /// </summary>
        private void RenderReportTemplateList()
        {
            // Clears any existing grid content on the second page.
            grdReportTemplateList.Children.Clear();
            grdReportTemplateList.RowDefinitions.Clear();
            //grdReportTemplateList.ColumnDefinitions.Clear();

            int intRowCount = GRGReportTemplates.Count;

            for (int i = 0; i < intRowCount; i++)
            {
                // Defines a new row for this report template.
                grdReportTemplateList.RowDefinitions.Add(new RowDefinition());

                // Creates a text block to display the name of the template.
                TextBlock txtTemplateName = new TextBlock();
                txtTemplateName.Text = GRGReportTemplates[i].ReportTemplateName;
                txtTemplateName.SetValue(Grid.RowProperty, i);
                txtTemplateName.SetValue(Grid.ColumnProperty, 0);
                txtTemplateName.Margin = new Thickness(0, 5, 5, 5);
                grdReportTemplateList.Children.Add(txtTemplateName);

                // Creates a button to view the report.
                Button btnViewReport = new Button();
                btnViewReport.Content = "View Report";
                btnViewReport.Tag = GRGReportTemplates[i];
                btnViewReport.Click += ViewReportButtonHandler;
                btnViewReport.SetValue(Grid.RowProperty, i);
                btnViewReport.SetValue(Grid.ColumnProperty, 1);
                btnViewReport.Margin = new Thickness(0, 5, 5, 5);
                btnViewReport.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
                grdReportTemplateList.Children.Add(btnViewReport);

                // Creates a button to export the report to Excel.
                Button btnExportToExcel = new Button();
                btnExportToExcel.Content = "Export to Excel";
                btnExportToExcel.Tag = GRGReportTemplates[i];
                btnExportToExcel.Click += ExportToExcelButtonHandler;
                btnExportToExcel.SetValue(Grid.RowProperty, i);
                btnExportToExcel.SetValue(Grid.ColumnProperty, 2);
                btnExportToExcel.Margin = new Thickness(0, 5, 5, 5);
                btnExportToExcel.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
                grdReportTemplateList.Children.Add(btnExportToExcel);

                // Creates a button to delete the report template.
                Button btnDeleteReport = new Button();
                btnDeleteReport.Content = "Delete Report";
                btnDeleteReport.Tag = GRGReportTemplates[i];
                btnDeleteReport.Click += DeleteReportButtonHandler;
                btnDeleteReport.SetValue(Grid.RowProperty, i);
                btnDeleteReport.SetValue(Grid.ColumnProperty, 3);
                btnDeleteReport.Margin = new Thickness(0, 5, 5, 5);
                btnDeleteReport.Template = (ControlTemplate)Application.Current.Resources["smallButton"];
                grdReportTemplateList.Children.Add(btnDeleteReport);
            }
        }

        /// <summary>
        /// Gets the list of report templates from the database.
        /// </summary>
        private void GetReportTemplateList()
        {
            using (DirectorPortalDatabase.DatabaseContext dbContext = new DirectorPortalDatabase.DatabaseContext())
            {
                GRGReportTemplates = dbContext.ReportTemplates.ToList();
            }
            RenderReportTemplateList();
        }

        /// <summary>
        /// Queries the database for all ReportField records belonging to the specified ReportTemplate.
        /// </summary>
        /// <param name="udtTemplate"></param>
        private List<ReportField> GetReportTemplateFields(ReportTemplate udtTemplate)
        {
            List<ReportField> rgReportFields;

            using (DirectorPortalDatabase.DatabaseContext dbContext = new DirectorPortalDatabase.DatabaseContext())
            {
                rgReportFields = dbContext.ReportFields.Where(x => x.TemplateId == udtTemplate.Id).ToList();
            }

            return rgReportFields;
        }

        /// <summary>
        /// Generates the report itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GenerateReportButtonHandler(object sender, EventArgs e)
        {
            if (lstIncludedReportFields.Items.Count > 0)
            {
                List<ModelAndField> rgReportColumns = new List<ModelAndField>();
                foreach (ListBoxItem lbiIncludedItem in lstIncludedReportFields.Items)
                {
                    ModelAndField udtModelAndField = (ModelAndField)lbiIncludedItem.Tag;
                    rgReportColumns.Add(udtModelAndField);
                }

                // TODO: Remove duplicate code.
                // ------------------------------------------------------------------------------

                // The report is a list of rows, each in the form of a string array.
                List<string[]> rgReport = new List<string[]>();

                // The first row of the report will contain the names of the fields (with their table names).
                string[] rgReportHead = new string[rgReportColumns.Count];
                for (int intLoop = 0; intLoop < rgReportColumns.Count; intLoop++)
                {
                    rgReportHead[intLoop] = rgReportColumns[intLoop].UdtModelInfo.StrHumanReadableName
                        + ": " + rgReportColumns[intLoop].UdtDataField.StrHumanReadableName;
                }

                rgReport.Add(rgReportHead);

                // Makes a list of tables to be included in the report.
                List<JoinHelper.EnumTable> rgTables = new List<JoinHelper.EnumTable>();
                foreach (ModelAndField udtModelAndField in rgReportColumns)
                {
                    // Gets the model type.
                    Type typeModelType = udtModelAndField.UdtModelInfo.TypeModelType;

                    // Stores the corresponding EnumTable in the list if it isn't already there.
                    JoinHelper.EnumTable enumTable = MetadataHelper.GetEnumTable(typeModelType);
                    if (!rgTables.Contains(enumTable))
                    {
                        rgTables.Add(enumTable);
                    }
                }

                JoinHelper.JoinResult udtJoinResult;
                JoinResultRecord[] rgQueryResults;

                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    // Performs all the querying to get the requested data.
                    udtJoinResult = new JoinHelper.JoinResult(dbContext, rgTables);
                    rgQueryResults = udtJoinResult.RGRecords;

                    // Iterates over the result records.
                    for (int intLoop = 0; intLoop < rgQueryResults.Length; intLoop++)
                    {
                        // Creates an array for this record.
                        string[] rgReportRow = new string[rgReportColumns.Count];

                        JoinResultRecord udtRecord = rgQueryResults[intLoop];

                        // This boolean is used to prevent adding completely empty rows to the report.
                        bool blnAllEmpty = true;

                        for (int intFieldIndex = 0; intFieldIndex < rgReportColumns.Count; intFieldIndex++)
                        {
                            ModelAndField udtModelAndField = rgReportColumns[intFieldIndex];

                            // Gets the correct model from the record.
                            object objModel = udtRecord.GetObjectByType(udtModelAndField.UdtModelInfo.TypeModelType);

                            // Gets the value of the desired field.
                            object objValue = udtModelAndField.UdtDataField.GetValue(objModel);

                            // Stores the value in the report row (as a string).
                            rgReportRow[intFieldIndex] = objValue?.ToString() ?? "";

                            if (rgReportRow[intFieldIndex] != "")
                            {
                                blnAllEmpty = false;
                            }
                        }

                        if (!blnAllEmpty)
                        {
                            // Adds the array to the report.
                            rgReport.Add(rgReportRow);
                        }
                    }
                }

                GRGCurrentReport = rgReport;

                //Checks if integer is set to allow the Report to be Rendered.
                if (intKeyForExport == 0)
                {
                    RenderReport();
                }



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
            helpWindow.tabs.SelectedIndex = 4;

        }

        /// <summary>
        /// Switches to the form in which the new template name is input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveReportTypeButtonHandler(object sender, EventArgs e)
        {
            if (lstIncludedReportFields.Items.Count > 0)
            {
                // Switches to the template name input form.
                tbcMainControl.Visibility = Visibility.Collapsed;
                spTemplateInput.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Adds all of a report templates field's to the list of included fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ViewReportButtonHandler(object sender, EventArgs e)
        {

            // Clears out all existing ListBoxItems in the ListBox of included report items.
            lstIncludedReportFields.Items.Clear();

            // Gets the report template instance from the button.
            Button btnSender = (Button)sender;
            ReportTemplate udtReportTemplate = (ReportTemplate)btnSender.Tag;

            // Gets all fields included in this report.
            List<ReportField> rgReportFields = GetReportTemplateFields(udtReportTemplate);

            foreach (ReportField udtReportField in rgReportFields)
            {
                // Gets the model information and class property metadata for this report item.
                MetadataHelper.ModelInfo udtModelInfo = MetadataHelper.GetModelInfoByName(udtReportField.ModelName);

                // Searches for a property with a matching name.
                for (int intLoop = 0; intLoop < udtModelInfo.UdtTableMetaData.IntNumberOfFields; intLoop++)
                {
                    FieldHelper.IDataField udtTableField = udtModelInfo.UdtTableMetaData.GetField(intLoop);
                    if (udtTableField.StrPropertyName == udtReportField.ModelPropertyName)
                    {
                        // Creates a ListBoxItem to store this report item.
                        ModelAndField udtModelAndField = new ModelAndField(udtModelInfo, udtTableField);
                        ListBoxItem lbiReportItem = new ListBoxItem();
                        lbiReportItem.Content = udtModelInfo.StrHumanReadableName + ": " + udtTableField.StrHumanReadableName;
                        lbiReportItem.Tag = udtModelAndField;

                        // Adds the ListBoxItem to the ListBox.
                        lstIncludedReportFields.Items.Add(lbiReportItem);

                        break;
                    }
                }
            }

            tbiGenerateReports.IsSelected = true;
        }

        /// <summary>
        /// Gathers the selected data from the database, then allows the user to select where and what to name the 
        /// excel file. 
        /// </summary>
        /// <param name="sender">The 'Export to Excel' Button</param>
        /// <param name="e">The Click Event</param>
        public void ExportToExcelButtonHandler(object sender, EventArgs e)
        {
            //Variables to create an excel workbook.
            var wbWorkbook = new XLWorkbook();
            wbWorkbook.AddWorksheet("sheetName");
            var wsSheet = wbWorkbook.Worksheet("sheetName");
            //String for save file path
            String strfilepath = " ";

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Clears out all existing ListBoxItems in the ListBox of included report items.
            lstIncludedReportFields.Items.Clear();

            // Gets the report template instance from the button.
            Button btnSender = (Button)sender;
            ReportTemplate udtReportTemplate = (ReportTemplate)btnSender.Tag;

            // Gets all fields included in this report.
            List<ReportField> rgReportFields = GetReportTemplateFields(udtReportTemplate);

            foreach (ReportField udtReportField in rgReportFields)
            {
                // Gets the model information and class property metadata for this report item.
                MetadataHelper.ModelInfo udtModelInfo = MetadataHelper.GetModelInfoByName(udtReportField.ModelName);

                // Searches for a property with a matching name.
                for (int intLoop = 0; intLoop < udtModelInfo.UdtTableMetaData.IntNumberOfFields; intLoop++)
                {
                    FieldHelper.IDataField udtTableField = udtModelInfo.UdtTableMetaData.GetField(intLoop);
                    if (udtTableField.StrPropertyName == udtReportField.ModelPropertyName)
                    {
                        // Creates a ListBoxItem to store this report item.
                        ModelAndField udtModelAndField = new ModelAndField(udtModelInfo, udtTableField);
                        ListBoxItem lbiReportItem = new ListBoxItem();
                        lbiReportItem.Content = udtModelInfo.StrHumanReadableName + ": " + udtTableField.StrHumanReadableName;
                        lbiReportItem.Tag = udtModelAndField;

                        // Adds the ListBoxItem to the ListBox.
                        lstIncludedReportFields.Items.Add(lbiReportItem);

                        break;
                    }
                }
            }

            if (lstIncludedReportFields.Items.Count > 0)
            {
                List<ModelAndField> rgReportColumns = new List<ModelAndField>();
                foreach (ListBoxItem lbiIncludedItem in lstIncludedReportFields.Items)
                {
                    ModelAndField udtModelAndField = (ModelAndField)lbiIncludedItem.Tag;
                    rgReportColumns.Add(udtModelAndField);
                }

                // TODO: Remove duplicate code.
                // ------------------------------------------------------------------------------

                // The report is a list of rows, each in the form of a string array.
                List<string[]> rgReport = new List<string[]>();

                // The first row of the report will contain the names of the fields (with their table names).
                string[] rgReportHead = new string[rgReportColumns.Count];
                for (int intLoop = 0; intLoop < rgReportColumns.Count; intLoop++)
                {
                    rgReportHead[intLoop] = rgReportColumns[intLoop].UdtModelInfo.StrHumanReadableName
                        + ": " + rgReportColumns[intLoop].UdtDataField.StrHumanReadableName;
                }

                rgReport.Add(rgReportHead);

                // Makes a list of tables to be included in the report.
                List<JoinHelper.EnumTable> rgTables = new List<JoinHelper.EnumTable>();
                foreach (ModelAndField udtModelAndField in rgReportColumns)
                {
                    // Gets the model type.
                    Type typeModelType = udtModelAndField.UdtModelInfo.TypeModelType;

                    // Stores the corresponding EnumTable in the list if it isn't already there.
                    JoinHelper.EnumTable enumTable = MetadataHelper.GetEnumTable(typeModelType);
                    if (!rgTables.Contains(enumTable))
                    {
                        rgTables.Add(enumTable);
                    }
                }

                JoinHelper.JoinResult udtJoinResult;
                JoinResultRecord[] rgQueryResults;

                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    // Performs all the querying to get the requested data.
                    udtJoinResult = new JoinHelper.JoinResult(dbContext, rgTables);
                    rgQueryResults = udtJoinResult.RGRecords;

                    // This boolean is used to prevent adding completely empty rows to the report.
                    bool blnAllEmpty = true;

                    // Iterates over the result records.
                    for (int intLoop = 0; intLoop < rgQueryResults.Length; intLoop++)
                    {
                        // Creates an array for this record.
                        string[] rgReportRow = new string[rgReportColumns.Count]; //[rgQueryResults.Length];

                        JoinResultRecord udtRecord = rgQueryResults[intLoop];

                        for (int intFieldIndex = 0; intFieldIndex < rgReportColumns.Count; intFieldIndex++)
                        {
                            ModelAndField udtModelAndField = rgReportColumns[intFieldIndex];

                            // Gets the correct model from the record.
                            object objModel = udtRecord.GetObjectByType(udtModelAndField.UdtModelInfo.TypeModelType);

                            // Gets the value of the desired field.
                            object objValue = udtModelAndField.UdtDataField.GetValue(objModel);

                            // Stores the value in the report row (as a string).
                            rgReportRow[intFieldIndex] = objValue?.ToString() ?? "";

                            if (rgReportRow[intFieldIndex] != "")
                            {
                                blnAllEmpty = false;
                            }
                        }

                        if (!blnAllEmpty)
                        {
                            // Adds the array to the report.
                            rgReport.Add(rgReportRow);
                        }
                    }
                }

                GRGCurrentReport = rgReport;




            }
            List<int> lstColumnSize = new List<int>();

            //Loop to insert data into excel file.
            for (int i = 0; i < GRGCurrentReport.Count; i++)
            {
                for (int j = 0; j < GRGCurrentReport[i].Length; j++)
                {
                    //If value is null then skip
                    if (GRGCurrentReport[i].GetValue(j) == null)
                    {
                        break;
                    }
                    else
                    {
                        //Adds first value into list
                        if (i == 0)
                        {
                            int intValueSize = GRGCurrentReport[i].GetValue(j).ToString().Length;
                            lstColumnSize.Add(intValueSize);
                        }

                        //If new value is larger than initial value then sets larger value to new value.
                        if (lstColumnSize[j] < GRGCurrentReport[i].GetValue(j).ToString().Length)
                        {
                            lstColumnSize[j] = GRGCurrentReport[i].GetValue(j).ToString().Length;

                        }
                    }

                }
            }



            //Loop to insert data into excel file.
            for (int i = 0; i < GRGCurrentReport.Count; i++)
            {
                for (int j = 0; j < GRGCurrentReport[i].Length; j++)
                {
                    //If the report value is null then skip
                    if (GRGCurrentReport[i].GetValue(j) == null)
                    {
                        break;
                    }
                    else
                    {
                        //For the first row column size is set for excel columns
                        if (i == 0)
                        {
                            //Sets column size
                            wsSheet.Cell(i + 1, j + 1).WorksheetColumn().Width = lstColumnSize[j];
                        }

                        //Write report value to excel cell
                        wsSheet.Cell(i + 1, j + 1).Value = GRGCurrentReport[i].GetValue(j).ToString();
                    }

                }
            }


            //Open save file dialog for saving data
            if (saveFileDialog.ShowDialog() == true)
                strfilepath = saveFileDialog.FileName;

            //Saving the workbook in the selected path
            wbWorkbook.SaveAs(strfilepath + ".xlsx");
            try
            {
                System.Diagnostics.Process.Start(strfilepath + ".xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            lstIncludedReportFields.Items.Clear();
        }

        /// <summary>
        /// Deletes a report template from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteReportButtonHandler(object sender, EventArgs e)
        {
            // Gets the report template from the button.
            Button btnSender = (Button)sender;
            ReportTemplate udtReportTemplate = (ReportTemplate)btnSender.Tag;

            using (DirectorPortalDatabase.DatabaseContext dbContext = new DirectorPortalDatabase.DatabaseContext())
            {
                // Deletes all ReportFields belonging to the specified ReportType.
                ReportField[] rgReportFields = dbContext.ReportFields.Where(x => x.TemplateId == udtReportTemplate.Id).ToArray();
                dbContext.RemoveRange(rgReportFields);

                // Deletes the ReportTemplate itself.
                dbContext.Remove(udtReportTemplate);
                dbContext.SaveChanges();
            }

            // Updates the local report template list to account for the deletion that occurred.
            GetReportTemplateList();
        }

        public GenerateReportsPage()
        {
            InitializeComponent();

            // Updates the model info dictionary to account for changes in extra fields.
            MetadataHelper.RefreshModelInfo();

            GRGCurrentReport = new List<string[]>();
            GRGReportTypeItems = new ComboBoxItem[MetadataHelper.IntNumberOfModels];

            for (int i = 0; i < MetadataHelper.IntNumberOfModels; i++)
            {
                // Gets info on the i-th database model.
                Type typeModelType = MetadataHelper.GetModelTypeByIndex(i);
                MetadataHelper.ModelInfo udtModelInfo = MetadataHelper.GetModelInfo(typeModelType);

                // Stores model information in a new ComboBoxItem.
                ComboBoxItem cbiModelItem = new ComboBoxItem();
                cbiModelItem.Content = udtModelInfo.StrHumanReadableName;
                cbiModelItem.Tag = udtModelInfo;

                // Adds the ComboBoxItem to the array.
                GRGReportTypeItems[i] = cbiModelItem;
            }

            cboReportType.ItemsSource = GRGReportTypeItems;

            GetReportTemplateList();
        }

        /// <summary>
        /// This method is called after the user enters the name of a new report template and clicks the "Save Template" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveReportTemplateNameButtonHandler(object sender, RoutedEventArgs e)
        {
            // Gets the name of the new template from the TextBox.
            string strNewTemplateName = txtReportTemplateName.Text;

            // Validates the name.
            if (!string.IsNullOrWhiteSpace(strNewTemplateName))
            {
                strNewTemplateName = strNewTemplateName.Trim();

                // Creates a model instance to represent the report template.
                ReportTemplate udtTemplate = new ReportTemplate
                {
                    ReportTemplateName = strNewTemplateName
                };

                using (DirectorPortalDatabase.DatabaseContext dbContext = new DirectorPortalDatabase.DatabaseContext())
                {
                    // Inserts the template record into the database.
                    dbContext.ReportTemplates.Add(udtTemplate);
                    dbContext.SaveChanges();

                    // Creates a model instance for each field in the template.
                    foreach (ListBoxItem lbiIncludedItem in lstIncludedReportFields.Items)
                    {
                        ModelAndField udtModelAndField = (ModelAndField)lbiIncludedItem.Tag;
                        ReportField udtReportField = new ReportField
                        {
                            // This ID links to the template record.
                            TemplateId = udtTemplate.Id,
                            ModelName = udtModelAndField.UdtModelInfo.TypeModelType.Name,
                            ModelPropertyName = udtModelAndField.UdtDataField.StrPropertyName
                        };

                        dbContext.ReportFields.Add(udtReportField);
                    }
                    dbContext.SaveChanges();
                }

                // Gets the updated report template list.
                GetReportTemplateList();

                // Switches back to the main form.
                spTemplateInput.Visibility = Visibility.Collapsed;
                tbcMainControl.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// This method is called when the user cancels the operation of saving a new report template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonHandler(object sender, RoutedEventArgs e)
        {
            // Switches back to the main form.
            spTemplateInput.Visibility = Visibility.Collapsed;
            tbcMainControl.Visibility = Visibility.Visible;


            txtReportTemplateName.Text = "";
        }

        /// <summary>
        /// Called when a user clicks on a report field in the left list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstReportFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only does anything if an item was selected, rather than unselected.
            if (lstReportFields.SelectedItem != null)
            {
                // Extracts the field from the selected listbox item.
                ListBoxItem lbiSelectedItem = (ListBoxItem)lstReportFields.SelectedItem;
                FieldHelper.IDataField udtField = (FieldHelper.IDataField)lbiSelectedItem.Tag;

                // Creates an object to identify this report item.
                ModelAndField udtModelAndField = new ModelAndField(GUdtSelectedReportType, udtField);

                bool blnFound = false;
                // Iterates over the listbox items to see if this report item is already in the list.
                foreach (ListBoxItem lbiReportItemHolder in lstIncludedReportFields.Items)
                {
                    // Gets the report item from the listbox item.
                    ModelAndField udtOtherModelAndField = (ModelAndField)lbiReportItemHolder.Tag;
                    // Compares this with the item to be added.
                    if (udtModelAndField.Equals(udtOtherModelAndField))
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound)
                {
                    // Creates a new listbox item for the other listbox.
                    ListBoxItem lbiSelectedReportField = new ListBoxItem();
                    lbiSelectedReportField.Content = GUdtSelectedReportType.StrHumanReadableName + ": " + udtField.StrHumanReadableName;

                    // Stores both the current model and field metadata in the listbox item.
                    lbiSelectedReportField.Tag = new ModelAndField(GUdtSelectedReportType, udtField);

                    lstIncludedReportFields.Items.Add(lbiSelectedReportField);
                }

                lstReportFields.UnselectAll();
            }
        }

        /// <summary>
        /// Clears all included fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearFields_Click(object sender, RoutedEventArgs e)
        {
            lstIncludedReportFields.Items.Clear();
        }

        /// <summary>
        /// Clears only the included fields that are selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearSelectedFields_Click(object sender, RoutedEventArgs e)
        {
            while (lstIncludedReportFields.SelectedItems.Count > 0)
            {
                ListBoxItem lbiSelectedItem = (ListBoxItem)lstIncludedReportFields.SelectedItems[0];
                lstIncludedReportFields.Items.Remove(lbiSelectedItem);
            }
        }

        /// <summary>
        /// Moves selected item within the "Included Report Fields" listbox up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReorderUp_Click(object sender, RoutedEventArgs e)
        {
            if(lstIncludedReportFields.SelectedIndex > 0)
            {
                int index = lstIncludedReportFields.SelectedIndex;
                object item = lstIncludedReportFields.SelectedItem;
                lstIncludedReportFields.Items.RemoveAt(lstIncludedReportFields.SelectedIndex);
                lstIncludedReportFields.Items.Insert(index - 1, item);
                lstIncludedReportFields.SelectedIndex = index - 1;
            }
        }

        /// <summary>
        /// Moves selected item within the "Included Report Fields" listbox down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReorderDown_Click(object sender, RoutedEventArgs e)
        {
            if (lstIncludedReportFields.SelectedIndex < lstIncludedReportFields.Items.Count - 1 && lstIncludedReportFields.SelectedIndex != -1)
            {
                int index = lstIncludedReportFields.SelectedIndex;
                object item = lstIncludedReportFields.SelectedItem;
                lstIncludedReportFields.Items.RemoveAt(lstIncludedReportFields.SelectedIndex);
                lstIncludedReportFields.Items.Insert(index + 1, item);
                lstIncludedReportFields.SelectedIndex = index + 1;
            }
        }


    }

}







