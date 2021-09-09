using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorPortalPayPal
{
    /// <summary>
    /// A summary report returned after the CsvParser database import is ran.
    /// </summary>
    public class TransactionImportResult
    {
        public List<TransactionImportFailure> ImportFailures { get; }
        public List<Transaction> ImportSuccesses { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="importFailures">Objects that report the context of each import failure.</param>
        /// <param name="importSuccesses">The transaction objects that were posted into the database.</param>
        public TransactionImportResult( List<TransactionImportFailure> importFailures, 
            List<Transaction> importSuccesses)
        {
            ImportFailures = importFailures;
            ImportSuccesses = importSuccesses;
        }

        /// <summary>
        /// The number of failures reported due to the transaction already being in the database.
        /// </summary>
        /// <returns>Number of Transactions that failed to import due to it already existing in the database.</returns>
        public int DuplicateCount()
        {
            int count = 0;
            foreach(TransactionImportFailure failure in ImportFailures)
            {
                if(failure.FailureReason == TransactionImportFailure.FailureReasons.Duplicate)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// If the import ran completly without complication.
        /// </summary>
        /// <returns>True if all transactions had a clear resolution while importing. False otherwise.</returns>
        public bool Successful()
        {
            foreach(TransactionImportFailure failure in ImportFailures)
            {
                if(failure.FailureReason != TransactionImportFailure.FailureReasons.Duplicate)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
