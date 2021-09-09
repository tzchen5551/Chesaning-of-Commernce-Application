using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class Email
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The ID of the person the email belongs to
        /// </summary>
        public int ContactPersonId { get; set; }
        public virtual ContactPerson ContactPerson { get; set; }
        /// <summary>
        /// The email address
        /// </summary>
        public string EmailAddress { get; set; }
        public virtual List<EmailGroupMember> EmailGroups { get; set; }
    }
}
