using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class CategoryRef
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
        /// The category id from the table
        /// </summary>
        public int CategoryId { get; set; }
        public virtual Categories Category { get; set; }
    }
}
