using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DirectorPortalDatabase.Models
{
    public class ContactPerson
    {
        /// <summary>
        /// The Primary Key of the contact person in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The name of the contact person
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A list of the emails associated with a contact person.
        /// </summary>
        public virtual List<Email> Emails { get; set; }
        /// <summary>
        /// A list of the phone numbers associated with the contact person.
        /// </summary>
        public virtual List<PhoneNumber> PhoneNumbers { get; set; }
        public virtual List<BusinessRep> BusinessReps { get; set; }
    }
}
