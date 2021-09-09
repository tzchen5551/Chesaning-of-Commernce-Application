using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class EmailGroupMember
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Forign key from EmailGroup database
        /// </summary>
        public int GroupId { get; set; }
        public virtual EmailGroup Group { get; set; }
        /// <summary>
        /// Forign key from Email database
        /// </summary>
        public int EmailId { get; set; }
        public virtual Email Email { get; set; }
    }
}
