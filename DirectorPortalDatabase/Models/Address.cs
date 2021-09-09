using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    /// <summary>
    /// Represents an address for a business. 
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The actual address number and street of the address
        /// </summary>
        public string StreetAddress { get; set; }
        /// <summary>
        /// The city portion of the address
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// The state portion of the address
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// The zip code of the address
        /// </summary>
        public int? ZipCode { get; set; }
        /// <summary>
        /// The zip extended code of the address
        /// </summary>
        public string ZipCodeExt { get; set; }

        /// <summary>
        /// A mehtod for comparing two addresses.
        /// </summary>
        /// <param name="obj">The object to compare to this address.</param>
        /// <returns>A boolean sating if the addresses are equal.</returns>
        public override bool Equals(object obj)
        {
            Address addressToCompare = obj as Address;

            if (StreetAddress.Equals(addressToCompare.StreetAddress) &&
                City.Equals(addressToCompare.City) &&
                State.Equals(addressToCompare.State) &&
                ZipCode == addressToCompare.ZipCode)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// A method for generating the hash code of the address.
        /// </summary>
        /// <returns>The Addresses has code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// A method for checking if an address is empty.
        /// </summary>
        /// <returns>Returtns true if the address is empty.</returns>
        public bool IsEmpty()
        {
            return (StreetAddress.Equals("")
                    && City.Equals("")
                    && StreetAddress.Equals("")
                    && ZipCode == 0);
        }
    }
}
