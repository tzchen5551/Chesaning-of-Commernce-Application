using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// File Purpose:
///     This file is designed to dynamically write an HTML document.
///     Data from the database is loaded into the HTML document to populate currrent Members to
///     be displayed on the Chamber of Commerce website.
///         
/// </summary>

namespace DirectorsPortalWPF.ValidateWebsite
{

    /// <summary>
    /// Preview generator responsible for writing an HTML file with
    /// latest membership data.
    /// </summary>
    class HtmlPreviewGenerator
    {

        // Writer will be used to write an HTML file
        private StreamWriter GWriter;

        /// <summary>
        /// Method writes an HTML document using the StreamWriter and dynamically adds Member
        /// details using the Entity Framework Database.
        /// </summary>
        public void GeneratePreview()
        {
            try
            {
                // Using the StreamWriter to Write to the MembershipTemplate.html file (under the resources folder)
                using (GWriter = new StreamWriter(GetTemplateLocation(), false))
                {
                    // Featured businesses header
                    GWriter.WriteLine("<h3 style=\"text-align: center; font-family: sans-serif; color:#448eb8; font-weight: bold;" +
                        " font-size: 1cm\">Chamber Member Businesses</h3>");

                    // Div to align content within margins
                    GWriter.WriteLine("<div style=\"margin-left: 10%; margin-right:10% \">");

                    WriteButtons();
                    PrintMembersAZ();
                    PrintMembersCategory();
                    PrintMembersAssociate();
                    GWriter.WriteLine("</div>");

                    WriteJavaScript();
                }
            }
            catch (IOException)
            {
                throw new IOException("Refresh failure! Please try refreshing again.");
            }

        }

        /// <summary>
        /// Prints a table containing members from the database in Alphabetical order.
        /// </summary>
        private void PrintMembersAZ()
        {
            List<string> rgAlpha = new List<string>() {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };

            // Second table to hold Member details A_Z
            GWriter.WriteLine("<table id=\"Members A-Z\" style=\"font-family: Arial, Arial, Helvetica, sans-serif; margin: auto;\">");
            GWriter.WriteLine($"<tr><td><strong>All Members from A to Z</strong></td><tr>");
            using (var dbContext = new DatabaseContext())     // Database context will be used to query membership details
            {
                List<Business> rgAllBusinesses = dbContext.Businesses
                    .Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers).OrderBy(x => x.BusinessName).ToList();  // List of all businesses in DB

                // Iterate through each Member and put them in the table
                foreach (string strAlpha in rgAlpha)
                {
                    // Only print the Alphabetic character IF there are businesses that start with that Alphabetic character
                    List<Business> rgValidBusinessesForAlpha = rgAllBusinesses.FindAll(e => e.BusinessName.ToUpper().StartsWith(strAlpha));
                    if (rgValidBusinessesForAlpha.Count > 0)
                    {
                        GWriter.WriteLine($"<tr><td>{strAlpha}</td><tr>");
                    }
                    int intNumberOfItems = 3;  // The number of Members per row
                    foreach (Business busCurrentBusiness in rgValidBusinessesForAlpha)
                    {
                        if (busCurrentBusiness.BusinessName.ToUpper().StartsWith(strAlpha))
                        {
                            //BusinessRep rgCurrentBusinessRep = dbContext.BusinessReps.Where(e => e.BusinessId.Equals(busCurrentBusiness.Id)).First();
                            BusinessRep rgCurrentBusinessRep = busCurrentBusiness.BusinessReps.FirstOrDefault();
                            //ContactPerson rgCurrentContactPerson = dbContext.ContactPeople.Where(e => e.Id.Equals(rgCurrentBusinessRep.ContactPersonId)).First();
                            //ContactPerson rgCurrentContactPerson = rgCurrentBusinessRep.ContactPerson;

                            // Phone numbers, addresses, and email addresses for current business
                            //List<PhoneNumber> rgCurrentBusPhones = dbContext.PhoneNumbers.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();
                            //List<PhoneNumber> rgCurrentBusPhones = rgCurrentContactPerson.PhoneNumbers.ToList();
                            //List<Address> rgCurrentBusAddresses = dbContext.Addresses.Where(e => e.Id.Equals(busCurrentBusiness.PhysicalAddressId)).ToList();
                            //Address rgCurrentBusAddresses = busCurrentBusiness.PhysicalAddress;
                            //List<Email> rgCurrentBusEmails = dbContext.Emails.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();
                            //List<Email> rgCurrentBusEmails = rgCurrentContactPerson.Emails.ToList();

                            if (intNumberOfItems == 3)             // Create a new row if there are three items to add to the row
                                GWriter.WriteLine("<tr>");

                            PrintMemberToTemplate(busCurrentBusiness);

                            if (intNumberOfItems == 1)             // If this is the last of the three items in the row, then close the row
                            {
                                GWriter.WriteLine("</tr>");
                                intNumberOfItems = 4;              // 4 minus 3 (from the decremator below) equals 3 items per row
                            }
                        }
                        intNumberOfItems--;    // Next item in the row
                    }
                }
            }
            GWriter.WriteLine("</table>");
        }

        /// <summary>
        /// Prints a table containing members from the database and all members are grouped under a business
        /// Category.
        /// </summary>
        private void PrintMembersCategory()
        {
            // Third table to hold Member details by Category
            GWriter.WriteLine("<table id=\"Category\" hidden=\"hidden\" style=\"font-family: Arial, Arial, Helvetica, sans-serif; margin: auto;\">");
            GWriter.WriteLine("<tr><td><strong>Members by Category</strong></td><tr>");

            using (var dbContext = new DatabaseContext())     // Database context will be used to query membership details
            {
                List<Business> rgAllBusinesses = dbContext.Businesses
                    .Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers).OrderBy(x => x.BusinessName).ToList(); // List of all businesses in DB

                List<Categories> rgAllCategories = dbContext.Categories.ToList();
                List<CategoryRef> rgAllCategoryRefs = dbContext.CategoryRef.ToList();

                foreach (Categories cat in rgAllCategories)
                {
                    //Loop through all references to current category and store each business to be output
                    List<CategoryRef> rgCurrentCategoryRefs = rgAllCategoryRefs.Where(cr => cr.CategoryId.Equals(cat.Id)).ToList();
                    List<Business> rgValidBusinessesMatchingCategory = new List<Business>();
                    foreach (CategoryRef catref in rgCurrentCategoryRefs)
                    {
                        rgValidBusinessesMatchingCategory.Add(rgAllBusinesses.Where(b => b.Id.Equals(catref.BusinessId)).FirstOrDefault());
                    }
                    

                    if (rgValidBusinessesMatchingCategory.Count > 0) 
                    {
                        GWriter.WriteLine($"<tr><td>{cat.Category}</td><tr>");
                    }

                    // Iterate through each Member and put them in the table
                    int intNumberOfItems = 3;  // The number of Members per row
                    foreach (Business busCurrentBusiness in rgValidBusinessesMatchingCategory)
                    {
                        CategoryRef categorySearch = busCurrentBusiness.CategoryRefs.Find(x => x.Category.Category.Equals(cat.Category));

                        if (categorySearch.Category.Category.Equals(cat.Category))
                        {
/*                            BusinessRep rgCurrentBusinessRep = dbContext.BusinessReps.Where(e => e.BusinessId.Equals(busCurrentBusiness.Id)).First();
                            ContactPerson rgCurrentContactPerson = dbContext.ContactPeople.Where(e => e.Id.Equals(rgCurrentBusinessRep.ContactPersonId)).First();

                            // Phone numbers, addresses, and email addresses for current business
                            List<PhoneNumber> rgCurrentBusPhones = dbContext.PhoneNumbers.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();
                            List<Address> rgCurrentBusAddresses = dbContext.Addresses.Where(e => e.Id.Equals(busCurrentBusiness.Id)).ToList();
                            List<Email> rgCurrentBusEmails = dbContext.Emails.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();*/

                            if (intNumberOfItems == 3)             // Create a new row if there are three items to add to the row
                                GWriter.WriteLine("<tr>");

                            PrintMemberToTemplate(busCurrentBusiness);

                            if (intNumberOfItems == 1)             // If this is the last of the three items in the row, then close the row
                            {
                                GWriter.WriteLine("</tr>");
                                intNumberOfItems = 4;              // 4 minus 3 (from the decremator below) equals 3 items per row
                            }
                        }
                        intNumberOfItems--;    // Next item in the row
                    }
                }
            }
            GWriter.WriteLine("</table>");
        }

        /// <summary>
        /// Prints a table only containing Associate members from the database in Alphabetical order.
        /// </summary>
        private void PrintMembersAssociate()
        {
            // Fourth table to hold Member details only for Associate members
            GWriter.WriteLine("<table id=\"Associates\" hidden=\"hidden\" style=\"font-family: Arial, Arial, Helvetica, sans-serif; margin: auto;\">");
            GWriter.WriteLine("<tr><td><strong>Associate Members</strong></td><tr>");

            using (var dbContext = new DatabaseContext())     // Database context will be used to query membership details
            {
                List<Business> rgAllBusinesses = dbContext.Businesses.Include(x => x.MailingAddress)
                    .Include(x => x.PhysicalAddress)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.Emails)
                    .Include(x => x.BusinessReps)
                    .ThenInclude(x => x.ContactPerson)
                    .ThenInclude(x => x.PhoneNumbers)
                    .Where(e => (int)e.MembershipLevel == 3).ToList();  // List of all associate businesses in DB

                rgAllBusinesses = rgAllBusinesses.OrderBy(e => e.BusinessName).ToList();

                // Iterate through each Member and put them in the table
                int intNumberOfItems = 3;  // The number of Members per row
                foreach (Business busCurrentBusiness in rgAllBusinesses)
                {
/*                    BusinessRep rgCurrentBusinessRep = dbContext.BusinessReps.Where(e => e.BusinessId.Equals(busCurrentBusiness.Id)).First();
                    ContactPerson rgCurrentContactPerson = dbContext.ContactPeople.Where(e => e.Id.Equals(rgCurrentBusinessRep.ContactPersonId)).First();

                    // Phone numbers, addresses, and email addresses for current business
                    List<PhoneNumber> rgCurrentBusPhones = dbContext.PhoneNumbers.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();
                    List<Address> rgCurrentBusAddresses = dbContext.Addresses.Where(e => e.Id.Equals(busCurrentBusiness.Id)).ToList();
                    List<Email> rgCurrentBusEmails = dbContext.Emails.Where(e => e.ContactPersonId.Equals(rgCurrentContactPerson.Id)).ToList();*/

                    if (intNumberOfItems == 3)             // Create a new row if there are three items to add to the row
                        GWriter.WriteLine("<tr>");

                    PrintMemberToTemplate(busCurrentBusiness);

                    if (intNumberOfItems == 1)             // If this is the last of the three items in the row, then close the row
                    {
                        GWriter.WriteLine("</tr>");
                        intNumberOfItems = 4;              // 4 minus 3 (from the decremator below) equals 3 items per row
                    }

                    intNumberOfItems--;    // Next item in the row
                }
            }
            GWriter.WriteLine("</table>");
        }

        /// <summary>
        /// Writes in the JavaScript into the HTML File. This JavaScript defines the button controls,
        /// essentially hides and unhides the desired contents on the page.
        /// 
        /// JavaScript Layout (For Readability):
        /// 
        ///     function visibility(id) 
        ///     {
        ///         var x = document.getElementById(id);
        ///         
        ///         if (id === "Members A-Z")
        ///         {
        ///             x.hidden = "";
        ///             document.getElementById("Category").hidden = "hidden";
        ///             document.getElementById("Associates").hidden = "hidden";
        ///         }
        ///
        ///         if (id === "Category")
        ///         {
        ///             x.hidden = ""; document.getElementById("Members A-Z").hidden = "hidden";
        ///             document.getElementById("Associates").hidden = "hidden";
        ///         }
        ///    
        ///         if (id === "Associates")
        ///         {
        ///             x.hidden = ""; document.getElementById("Members A-Z").hidden = "hidden";
        ///             document.getElementById("Category").hidden = "hidden";
        ///         }
        ///     }
        ///     
        /// </summary>
        private void WriteJavaScript()
        {
            // JavaScript to allow the buttons to toggle between Members A-Z, Category, and Associate.
            GWriter.WriteLine("<script>");
            GWriter.WriteLine(
                "function visibility(id) {" +
                "var x = document.getElementById(id);" +
                "if (id === \"Members A-Z\") {" +
                "x.hidden = \"\";" +
                "document.getElementById(\"Category\").hidden = \"hidden\";" +
                "document.getElementById(\"Associates\").hidden = \"hidden\";" +
                "}" +
                "if (id === \"Category\") {" +
                "x.hidden = \"\";" +
                "document.getElementById(\"Members A-Z\").hidden = \"hidden\";" +
                "document.getElementById(\"Associates\").hidden = \"hidden\";" +
                "}" +
                "if (id === \"Associates\") {" +
                "x.hidden = \"\";" +
                "document.getElementById(\"Members A-Z\").hidden = \"hidden\";" +
                "document.getElementById(\"Category\").hidden = \"hidden\";" +
                "}" +
                "}"
                );
            GWriter.WriteLine("</script>");
        }

        /// <summary>
        /// Writes the buttons that will display above the Chamber member details.
        /// Will allow to cycle between Members A-Z, Members by Category, and Associate members.
        /// </summary>
        private void WriteButtons()
        {
            // First Table to hold buttons
            GWriter.WriteLine("<table style=\"font-family: Arial, Arial, Helvetica, sans-serif; margin: auto\">");

            // Include the buttons below...
            GWriter.WriteLine("<tr>");

            // Members A-Z
            GWriter.WriteLine("<td style=\"padding: 10px 10px 10px 10px; text-align:center\">");

            GWriter.WriteLine("<button type=\"button\"  onclick=\"visibility('Members A-Z')\" style=\"display: inline-block; text-decoration:none; " +
                "height: 65px; background-color:#4d97E4; border-radius: 5px; padding: 0px 30px 0px 0px\" href=\"http://www.google.com\">");

            GWriter.WriteLine("<span style=\"color:#fff !important;height: 65px; white-space:nowrap; line-height: " +
                "65px; font-size: 18px; padding: 0px 0px 0px 30px; display: block; font-weight: bold; text-shadow: 0px 1px 1px rgb(0,0,0);" +
                " text-align: center; font-family: Arial, Arial, Helvetica, sans-serif\">Members A-Z</span>");

            GWriter.WriteLine("</button>");
            GWriter.WriteLine("</td>");

            // By Category
            GWriter.WriteLine("<td style=\"padding: 10px 10px 10px 10px; text-align:center\">");

            GWriter.WriteLine("<button type=\"button\"  onclick=\"visibility('Category')\" style=\"display: inline-block; text-decoration:none;" +
                " height: 65px; background-color:#4d97E4; border-radius: 5px; padding: 0px 30px 0px 0px\" href=\"http://www.google.com\">");
            
            GWriter.WriteLine("<span style=\"color:#fff !important;height: 65px; white-space:nowrap; line-height: 65px; font-size: 18px;" +
                " padding: 0px 0px 0px 30px; display: block; font-weight: bold; text-shadow: 0px 1px 1px rgb(0,0,0); text-align: center;" +
                " font-family: Arial, Arial, Helvetica, sans-serif\">Category</span>");

            GWriter.WriteLine("</button>");
            GWriter.WriteLine("</td>");

            // Associates
            GWriter.WriteLine("<td style=\"padding: 10px 10px 10px 10px; text-align:center\">");

            GWriter.WriteLine("<button type=\"button\"  onclick=\"visibility('Associates')\" style=\"display: inline-block; text-decoration:none;" +
                " height: 65px; background-color:#4d97E4; border-radius: 5px; padding: 0px 30px 0px 0px\" href=\"http://www.google.com\">");
            
            GWriter.WriteLine("<span style=\"color:#fff !important;height: 65px; white-space:nowrap; line-height: 65px; font-size: 18px;" +
                " padding: 0px 0px 0px 30px; display: block; font-weight: bold; text-shadow: 0px 1px 1px rgb(0,0,0); text-align: center;" +
                " font-family: Arial, Arial, Helvetica, sans-serif\">Associates</span>");

            GWriter.WriteLine("</button>");
            GWriter.WriteLine("</td>");

            GWriter.WriteLine("</tr>");

            GWriter.WriteLine("</table>");
        }

        /// <summary>
        /// Prints a member and all of the member's details onto the HTML template.
        /// </summary>
        /// <param name="busCurrentBusiness">The current business to be printed onto the template.</param>
        /// <param name="rgCurrentBusPhones">All phone numbers associated with the current business</param>
        /// <param name="rgCurrentBusAddresses">All addresses associated with the current business</param>
        /// <param name="rgCurrentBusEmails">All email addresses associated with the current business</param>
        private void PrintMemberToTemplate(Business busCurrentBusiness)
        {
            if (busCurrentBusiness.MembershipLevel != MembershipLevel.INDIVIDUAL &&
                busCurrentBusiness.MembershipLevel != MembershipLevel.COURTESY &&
                busCurrentBusiness.MembershipLevel != MembershipLevel.NONE)
            {
                GWriter.WriteLine("<td style=\"padding: 10px 10px 10px 10px; color:#6d6d6d\" width=\"25%\">");
                GWriter.WriteLine($"<strong style=\"color: #a88d2e\">{busCurrentBusiness.BusinessName}</strong>");

                if (busCurrentBusiness != null)
                {
                    GWriter.WriteLine($"<br>{busCurrentBusiness.PhysicalAddress?.StreetAddress}");
                    GWriter.WriteLine($"<br>{busCurrentBusiness.PhysicalAddress?.City}, {busCurrentBusiness.PhysicalAddress?.State} {busCurrentBusiness.PhysicalAddress?.ZipCode} ");
                }

                if (busCurrentBusiness.BusinessReps != null && busCurrentBusiness.BusinessReps.Count > 0)
                {

                    GWriter.WriteLine($"<br>{busCurrentBusiness.BusinessReps[0].ContactPerson.PhoneNumbers.Where(x => x.GEnumPhoneType == PhoneType.Office).FirstOrDefault()?.Number}");
                    GWriter.WriteLine($"<br>{busCurrentBusiness.BusinessReps[0].ContactPerson.PhoneNumbers.Where(x => x.GEnumPhoneType == PhoneType.Fax).FirstOrDefault()?.Number}");
                }

                GWriter.Write($"<br>");
                string strMap = "";
                if (busCurrentBusiness.PhysicalAddress != null)
                {
                    strMap = busCurrentBusiness.PhysicalAddress?.StreetAddress.Replace(". ", "+");
                    strMap = strMap.Replace(" ", "+");
                }
                GWriter.Write($"<a href=\"https://www.google.com/maps/search/?api=1&query={strMap}%2C+{busCurrentBusiness.PhysicalAddress?.ZipCode} \" target=\"_blank\">Map</a>");
                if (busCurrentBusiness.BusinessReps != null && busCurrentBusiness.BusinessReps.Count > 0)
                    GWriter.Write($" | <a href=\"mailto:{busCurrentBusiness.BusinessReps[0].ContactPerson.Emails.FirstOrDefault()?.EmailAddress} \">Email</a>");

                if (busCurrentBusiness.Website.Contains("http://"))
                    GWriter.WriteLine($" | <a href=\"{busCurrentBusiness?.Website}\" target=\"_blank\">Web</a>");
                else
                    GWriter.WriteLine($" | <a href=\"http://{busCurrentBusiness?.Website}\" target=\"_blank\">Web</a>");
                GWriter.WriteLine("</td>");
            }
        }

        /// <summary>
        /// Used to get the filepath of the HTML template that will be used to generate the Members screen
        /// on the Weebly website.
        /// </summary>
        /// <returns></returns>
        public string GetTemplateLocation()
        {
            var strExePath = AppDomain.CurrentDomain.BaseDirectory;


            string strTemplateFullPath = Directory.GetParent(strExePath) + "\\Resources\\MembershipTemplate.html";

            Console.WriteLine(strTemplateFullPath);

            return strTemplateFullPath;
        }

    }
}
