using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class ReportField
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Foreign key to the ReportTemplate that this ReportField belongs to.
        /// </summary>
        public int TemplateId { get; set; }
        public virtual ReportTemplate Template { get; set; }
        public string ModelName { get; set; }
        public string ModelPropertyName { get; set; }
    }
}
