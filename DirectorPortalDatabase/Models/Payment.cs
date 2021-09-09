using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorPortalDatabase.Models
{
    /// <summary>
    /// Represents an itemized payment posted by a contact of a business,
    /// on the business's behalf. 
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// The Primary Key of the payment in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The business this payment was posted on behalf of.
        /// </summary>
        public virtual Business Business { get; set; }

        /// <summary>
        /// The list of items and quantity of each on this payment.
        /// </summary>
        public virtual List<PaymentItem> Items { get; set; }

        /// <summary>
        /// The date and time when this payment was posted.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The gross amount the customer paid.
        /// </summary>
        public decimal GrossPay { get; set; }

        /// <summary>
        /// The PayPal transaction ID of this payment.
        /// </summary>
        /// <remarks>
        /// Can be empty. Payment could have been posted through means other than PayPal.
        /// </remarks>
        public string PayPalTransactionId { get; set; }

        /// <summary>
        /// The invoice number of this payment.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// The text summary of the payment.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The PayPal
        /// </summary>
        /// <remarks>
        /// Can be empty. Payment could have been posted through means other than PayPal.
        /// </remarks>
        public string PayPalReferenceTxnId { get; set; }

        /// <summary>
        /// The fees, if any, imposed by the payment processor. Usually PayPal.
        /// </summary>
        public decimal ProcessingFees { get; set; }

        /// <summary>
        /// A method for generating the hash code of the payment.
        /// </summary>
        /// <returns>The Payments hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
