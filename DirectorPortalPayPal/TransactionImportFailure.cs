namespace DirectorPortalPayPal
{
    /// <summary>
    /// The context of a failed PayPal CSV Transaction import.
    /// </summary>
    public class TransactionImportFailure
    {
        /// <summary>
        /// The possible reason why the import failed.
        /// </summary>
        public enum FailureReasons
        {
            /// <summary>
            /// If an exception occurred, or an otherwise catch-all reason
            /// </summary>
            GeneralFailure,
            /// <summary>
            /// If the Transaction's FromEmail wasn't found in the database.
            /// </summary>
            FromEmailNotFound,
            /// <summary>
            /// If the FromEmail was found, but it could relate to several different Businesses.
            /// </summary>
            MoreThanOneRelatedBusiness,
            /// <summary>
            /// If the FromEmail was found, but there was no Business associated with the email.
            /// </summary>
            NoRelatedBusiness,
            /// <summary>
            /// The TransactionId already existed in the database.
            /// </summary>
            Duplicate
        }

        public Transaction Transaction { get; }
        public FailureReasons FailureReason { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="transaction">The transaction that failed to import.</param>
        /// <param name="failureReason">The reason why the transaction failed to import.</param>
        public TransactionImportFailure(Transaction transaction,
            FailureReasons failureReason)
        {
            Transaction = transaction;
            FailureReason = failureReason;
        }
    }
}