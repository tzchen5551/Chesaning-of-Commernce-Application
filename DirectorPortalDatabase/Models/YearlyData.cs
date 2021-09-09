using DirectorPortalDatabase.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    public class YearlyData : HasExtraFields
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The year that the data represents
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// The business that the data represents
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        /// <summary>
        /// Shows the amount of dues that have been
        /// paid in a term by the business.
        /// </summary>
        public double DuesPaid { get; set; }
        /// <summary>
        /// The amount of money in raffle tickets that
        /// has been returned
        /// </summary>
        public double TicketsReturned { get; set; }
        /// <summary>
        /// The amount of credit that a business has
        /// </summary>
        public double Credit { get; set; }
        /// <summary>
        /// The term length of the data
        /// </summary>
        public TermLength TermLength { get; set; }
        /// <summary>
        /// The ballot number that the business gets
        /// </summary>
        public int BallotNumber { get; set; }
    }

    /// <summary>
    /// The length of the term of "yearly" data.
    /// </summary>
    public enum TermLength
    {
        Annually = 0,
        Semiannually = 1,
        Quarterly = 2,
        Monthly = 3
    }
}
