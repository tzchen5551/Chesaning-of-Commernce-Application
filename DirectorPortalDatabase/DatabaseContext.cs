using DirectorPortalDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.IO;

namespace DirectorPortalDatabase
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base()
        {
        }

        /// <summary>
        /// Holds data about extra fields on tables.
        /// </summary>
        internal DbSet<AdditionalFields> AdditionalFields { get; set; }
        /// <summary>
        /// Addresses table. Represents a list of addresses so that each business can
        /// have a mailing address and a physical address
        /// </summary>
        public DbSet<Address> Addresses { get; set; }
        /// <summary>
        /// Businesses table. Represents the basic information about all of the businesses
        /// that are members of the chamber of commerce.
        /// </summary>
        public DbSet<Business> Businesses { get; set; }
        /// <summary>
        /// Represents the link between a business and a contact person.
        /// Allows for a business to have multiple contact people,
        /// while a person can also represent multiple businesses.
        /// </summary>
        public DbSet<BusinessRep> BusinessReps { get; set; }
        /// <summary>
        /// Represents the person that is a point of contact for a business.
        /// Stores their basic information, as well as phone numbers or emails.
        /// </summary>
        public DbSet<ContactPerson> ContactPeople { get; set; }
        /// <summary>
        /// A table with email addresses. Links an email to a contact person.
        /// </summary>
        public DbSet<Email> Emails { get; set; }
        /// <summary>
        /// A table that stores a group of emails
        /// </summary>
        public DbSet<EmailGroup> EmailGroups { get; set; }
        /// <summary>
        /// A relational table between Emails and EmailGroups 
        /// </summary>
        public DbSet<EmailGroupMember> EmailGroupMembers { get; set; }
        /// <summary>
        /// A table with phone numbers. Links a phone number to a contact person.
        /// Each phone number has a different type assosciated with it to help
        /// identify it against the others.
        /// </summary>
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        /// <summary>
        /// The information that changes about a business every year. Stores
        /// information about raffle tickets, ballots, and dues.
        /// </summary>
        public DbSet<YearlyData> BusinessYearlyData { get; set; }
        /// <summary>
        /// Stores a list of todo items.
        /// </summary>
        public DbSet<Todo> TodoListItems { get; set; }
        /// <summary>
        /// These are saved templates that can generate reports.
        /// </summary>
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        /// <summary>
        /// These are the fields that are contained within report templates. 
        /// </summary>
        public DbSet<ReportField> ReportFields { get; set; }
        /// Stores the various buissness type categories
        /// </summary>
        public DbSet<Categories> Categories { get; set; }

        /// <summary>
        /// Represents the link between a categories and buisnesses.
        /// Allows for a business to have multiple category types,
        /// while categories can be applicable to multiple businesses.
        /// </summary>
        public DbSet<CategoryRef> CategoryRef { get; set; }

        /// <summary>
        /// Represents an itemized payment posted by a contact of a business,
        /// on the business's behalf. 
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// Represents a line item on a Payment.
        /// </summary>
        public DbSet<PaymentItem> PaymentItems { get; set; }

        /// <summary>
        /// Pulls the connection string from the App.config file,
        /// then manipulates it to remove the %APPDATA% and replace it
        /// with the value of <code>Environment.GetFolderPath(ApplicationData)</code>.
        /// This gives an easy to find folder for the database to reside in, while
        /// not being immediately accessible to any person that's directly looking
        /// for it.
        /// </summary>
        /// <returns>
        /// The connection string with the %APPDATA% replaced with the
        /// actual file path to the AppData/Roaming folder.
        /// </returns>
        private static string GetConnectionString()
        {
            string strConnectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
            string newConnectionString = strConnectionString.Replace("%APPDATA%",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            // Console.WriteLine(newConnectionString); // Debug purposes, show the altered connection string
            return newConnectionString;
        }

        /// <summary>
        /// Needs a good implementation still
        /// Currently just returns a hardcoded value
        /// 
        /// Parses out the folder from the connection string to create the path to it.
        /// </summary>
        /// <param name="strConnectionString">
        /// The connection string to parse the folder path from
        /// </param>
        /// <returns>
        /// The folder path as specified by the connection string
        /// </returns>
        private static string GetFolderPathFromConnectionString(string strConnectionString)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChamberOfCommerce", "DirectorsPortal");
        }
        public static string GetFolderPath()
        {
            return GetFolderPathFromConnectionString(GetConnectionString());
        }

        /// <summary>
        /// Gets automatically run when the database context is being created.
        /// Pulls the connection string from the App.config file, then creates
        /// the folder that the database should reside in (because apparently
        /// EntityFramework doesn't do this by default).
        /// 
        /// Then sets up the database to use sqlite
        /// </summary>
        /// <param name="optionsBuilder">
        /// Options object that lets the database be configured
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string strConnectionString = GetConnectionString();
            Directory.CreateDirectory(GetFolderPathFromConnectionString(strConnectionString));
            optionsBuilder.UseSqlite(strConnectionString);
        }

    }
}
