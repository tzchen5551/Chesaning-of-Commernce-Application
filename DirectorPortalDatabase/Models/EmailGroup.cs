using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class EmailGroup
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The name of the email group assigned by the user
        /// </summary>
        public string GroupName { get; set; }
        public virtual List<EmailGroupMember> Emails { get; set; }
        /// <summary>
        /// Notes that the user may add to a specific email group
        /// </summary>
        public string Notes { get; set; }
    }
}
