using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class Categories
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The type category a buisness falls under (Education, Recreation, etc...)
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Updates the text file storing the category list
        /// Adds a new item sent to this method
        /// </summary>
        public static void UpdateCategoryList(string strNewCategory)
        {
            //Replace filename
            string strFilepath = "Buisness_Categories.txt";
            using (StreamWriter swFileOutput = File.AppendText(strFilepath))
            {
                swFileOutput.WriteLine(strNewCategory);
            }
        }
        /// <summary>
        /// Imports all the initial data from the buisness categories file
        /// </summary>
        public static void ImportFile()
        {
            var strExePath = AppDomain.CurrentDomain.BaseDirectory;
            string strFilepath = Directory.GetParent(strExePath) + "\\Business_Categories.txt";

            try
            {
                using (StreamReader srFileInput = File.OpenText(strFilepath))
                {
                    string strImportCategory = "";
                    using (var dbContext = new DatabaseContext())
                    {
                        while ((strImportCategory = srFileInput.ReadLine()) != null)
                        {
                            Categories category = new Categories()
                            {
                                Category = strImportCategory
                            };
                            dbContext.Categories.Add(category);

                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                string strDefaultCatagories = @"Area Information
Dining and Lodging
Education
Manufacturing
Professional Services
Radio/Television/Marketing
Recreation
Retail Shopping
Automotive Dealers
Balloons and Face-Painting
Cabinetry/Floors/Design
Campers/Dealers
Clothing/Shoes
Convenience Stores/Party Stores
Craft Supplies
Distribution Company
Discount Store
Feed Dealers
Gifts
Grocery
Hardware
Jewelers
Lumber Stores
Pharmacies
Pools and Spas
Tux Rental
Special Use Facilities
Marijuana Growing & Processing
Medical and/or Recreational Marijuana Provisioning
Accountants and Tax Services
Advertising and Media
Newspapers/Commercial Printing/Signs
Ambulance Service
Appliance Service/Parts
Attorneys
Automotive/Farm Equipment/RV 
Automotive Parts/Service/Repair/Sales
Classic Cars/Cargo Haulers
Farm Equipment Supplier & Service
Gas/Fuel Stations
RV Parts/Sales/Service
Towing/Hauling
Banks/Credit Unions
Beauty Salons
Contractors - Building
Contractors - Electric
Contractors - Excavating
Contractors - Heating/Cooling
Contractors - Roofing
Dumpster Rental/Recycling Services
Financial Services
Fitness and Tanning
Funeral Homes and Monument Services
Interiors/Design
Health Care
Assisted Living Home/Nursing Care
Dental
Optometrists
Insurance
Lawn Care/Landscaping
Manufactured & Modular Homes
Mortgages
Pools/Spas
Real Estate/Real Estate Appraisal
Screen Printing/Embroidery
Septic Tank Services/Porta Johns
Storage
Veterinarians";

                File.WriteAllText(strFilepath, strDefaultCatagories);
                ImportFile();


            }


        }

    }
}
