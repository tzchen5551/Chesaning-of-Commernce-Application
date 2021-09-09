using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorPortalPayPal
{
    /// <summary>
    /// Represents one row/line of a PayPal CSV export.
    /// </summary>
    public class Transaction
    {
        public bool IsValid { get; }
        public string Date { get; }
        public string Time { get; }
        public string TimeZone { get; }
        public string Name { get; }
        public string Type { get; }
        public string Status { get; }
        public string Currency { get; }
        public string Gross { get; }
        public string Fee { get; }
        public string Net { get; }
        public string FromEmail { get; }
        public string ToEmail { get; }
        public string TransactionId { get; }
        public string ShippingAddress { get; }
        public string AddressStatus { get; }
        public string ItemTitle { get; }
        public string ItemId { get; }
        public string ShippingAndHandlingAmount { get; }
        public string InsuranceAmount { get; }
        public string SalesTax { get; }
        public string ReferenceTxnId { get; }
        public string InvoiceNumber { get; }
        public string CustomNumber { get; }
        public string Quantity { get; }
        public string ReceiptId { get; }
        public string Balance { get; }
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string Country { get; }
        public string ContactPhoneNumber { get; }
        public string Subject { get; }
        public string Note { get; }
        public string CountryCode { get; }
        public string BalanceImpact { get; }

        /// <summary>
        /// Default constructor of a transaction.
        /// </summary>
        /// <param name="headers">The header row of the PayPal CSV.</param>
        /// <param name="csvLine">The data row of the PayPal CSV.</param>
        public Transaction(string[] headers, string[] csvLine)
        {
            IsValid = true;

            try
            {
                Date = csvLine[Array.IndexOf(headers, HeaderStrings.Date)];
                Time = csvLine[Array.IndexOf(headers, HeaderStrings.Time)];
                TimeZone = csvLine[Array.IndexOf(headers, HeaderStrings.TimeZone)];
                Name = csvLine[Array.IndexOf(headers, HeaderStrings.Name)];
                Type = csvLine[Array.IndexOf(headers, HeaderStrings.Type)];
                Status = csvLine[Array.IndexOf(headers, HeaderStrings.Status)];
                Currency = csvLine[Array.IndexOf(headers, HeaderStrings.Currency)];
                Gross = csvLine[Array.IndexOf(headers, HeaderStrings.Gross)];
                Fee = csvLine[Array.IndexOf(headers, HeaderStrings.Fee)];
                Net = csvLine[Array.IndexOf(headers, HeaderStrings.Net)];
                FromEmail = csvLine[Array.IndexOf(headers, HeaderStrings.FromEmail)];
                ToEmail = csvLine[Array.IndexOf(headers, HeaderStrings.ToEmail)];
                TransactionId = csvLine[Array.IndexOf(headers, HeaderStrings.TransactionId)];
                ShippingAddress = csvLine[Array.IndexOf(headers, HeaderStrings.ShippingAddress)];
                AddressStatus = csvLine[Array.IndexOf(headers, HeaderStrings.AddressStatus)];
                ItemTitle = csvLine[Array.IndexOf(headers, HeaderStrings.ItemTitle)];
                ItemId = csvLine[Array.IndexOf(headers, HeaderStrings.ItemId)];
                ShippingAndHandlingAmount = csvLine[Array.IndexOf(headers, HeaderStrings.ShippingAndHandlingAmount)];
                InsuranceAmount = csvLine[Array.IndexOf(headers, HeaderStrings.InsuranceAmount)];
                SalesTax = csvLine[Array.IndexOf(headers, HeaderStrings.SalesTax)];
                ReferenceTxnId = csvLine[Array.IndexOf(headers, HeaderStrings.ReferenceTransactionId)];
                InvoiceNumber = csvLine[Array.IndexOf(headers, HeaderStrings.InvoiceNumber)];
                CustomNumber = csvLine[Array.IndexOf(headers, HeaderStrings.CustomNumber)];
                Quantity = csvLine[Array.IndexOf(headers, HeaderStrings.Quantity)];
                ReceiptId = csvLine[Array.IndexOf(headers, HeaderStrings.ReceiptId)];
                Balance = csvLine[Array.IndexOf(headers, HeaderStrings.Balance)];
                AddressLine1 = csvLine[Array.IndexOf(headers, HeaderStrings.AddressLine1)];
                AddressLine2 = csvLine[Array.IndexOf(headers, HeaderStrings.AddressLine2)];
                City = csvLine[Array.IndexOf(headers, HeaderStrings.City)];
                State = csvLine[Array.IndexOf(headers, HeaderStrings.State)];
                ZipCode = csvLine[Array.IndexOf(headers, HeaderStrings.ZipCode)];
                Country = csvLine[Array.IndexOf(headers, HeaderStrings.Country)];
                ContactPhoneNumber = csvLine[Array.IndexOf(headers, HeaderStrings.ContactPhoneNumber)];
                Subject = csvLine[Array.IndexOf(headers, HeaderStrings.Subject)];
                Note = csvLine[Array.IndexOf(headers, HeaderStrings.Note)];
                CountryCode = csvLine[Array.IndexOf(headers, HeaderStrings.CountryCode)];
                BalanceImpact = csvLine[Array.IndexOf(headers, HeaderStrings.BalanceImpact)];
            }
            catch (Exception ex)
            {
                IsValid = false;
            }
        }

        /// <summary>
        /// The strings of the headers of each column in the PayPal CSV export.
        /// Maps each value to its member in the Transaction object while constructing.
        /// </summary>
        public class HeaderStrings
        {
            public static readonly string Date = "Date";
            public static readonly string Time = "Time";
            public static readonly string TimeZone = "TimeZone";
            public static readonly string Name = "Name";
            public static readonly string Type = "Type";
            public static readonly string Status = "Status";
            public static readonly string Currency = "Currency";
            public static readonly string Gross = "Gross";
            public static readonly string Fee = "Fee";
            public static readonly string Net = "Net";
            public static readonly string FromEmail = "From Email Address";
            public static readonly string ToEmail = "To Email Address";
            public static readonly string TransactionId = "Transaction ID";
            public static readonly string ShippingAddress = "Shipping Address";
            public static readonly string AddressStatus = "Address Status";
            public static readonly string ItemTitle = "Item Title";
            public static readonly string ItemId = "Item ID";
            public static readonly string ShippingAndHandlingAmount = "Shipping and Handling Amount";
            public static readonly string InsuranceAmount = "Insurance Amount";
            public static readonly string SalesTax = "Sales Tax";
            public static readonly string ReferenceTransactionId = "Reference Txn ID";
            public static readonly string InvoiceNumber = "Invoice Number";
            public static readonly string CustomNumber = "Custom Number";
            public static readonly string Quantity = "Quantity";
            public static readonly string ReceiptId = "Receipt ID";
            public static readonly string Balance = "Balance";
            public static readonly string AddressLine1 = "Address Line 1";
            public static readonly string AddressLine2 = "Address Line 2/District/Neighborhood";
            public static readonly string City = "Town/City";
            public static readonly string State = "State/Province/Region/County/Territory/Prefecture/Republic";
            public static readonly string ZipCode = "Zip/Postal Code";
            public static readonly string Country = "Country";
            public static readonly string ContactPhoneNumber = "Contact Phone Number";
            public static readonly string Subject = "Subject";
            public static readonly string Note = "Note";
            public static readonly string CountryCode = "Country Code";
            public static readonly string BalanceImpact = "Balance Impact";
        }
    }
}
