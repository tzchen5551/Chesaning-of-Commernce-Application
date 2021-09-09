using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class AdditionalFields
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Shows the name of the table that the extra field is tied to.
        /// Technically a foreign key, but pulled directly from the class metadata
        /// </summary>
        public string TableName { get; set; }
    }
}
