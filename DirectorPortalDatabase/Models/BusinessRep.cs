using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DirectorPortalDatabase.Models
{
    public class BusinessRep
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The id of the business that the representative
        /// belongs to in the database
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        /// <summary>
        /// The representative's id in the table
        /// </summary>
        public int ContactPersonId { get; set; }
        public virtual ContactPerson ContactPerson { get; set; }
    }
}
