using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class PhoneNumber
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The ID of the person the phone number belongs to
        /// </summary>
        public int ContactPersonId { get; set; }
        public virtual ContactPerson ContactPerson { get; set; }
        /// <summary>
        /// The actual phone number
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Any notes about the phone number
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// The type of phone number
        /// </summary>
        public PhoneType GEnumPhoneType { get; set; }
    }

    public enum PhoneType
    {
        //None = 0,
        Mobile = 0,
        Office = 1,
        Fax = 2
    }
}
