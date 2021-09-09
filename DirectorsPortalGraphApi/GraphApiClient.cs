using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using DirectorsPortal.GraphHelpers;
using System.Linq;

namespace DirectorsPortal
{
    /// <summary>
    /// class containing async methods for making calls to Graph Api Client
    /// </summary>
    public class GraphApiClient
    {

        /// <summary>
        /// Testing method sends pre defined message
        /// </summary>
        /// <returns></returns>
        //static void Main(string[] args)
        //{
        //    //Task objSending = SendMail();
        //    //objSending.Wait();
        //    //Console.WriteLine(objSending.Status);
        //    Console.ReadLine();
        //}

        /// <summary>
        /// Get Message by ID method
        /// </summary>
        /// <returns>a string containing the specified message</returns>
        public static async Task<string> GetEmail(String strID)
        {
            GraphServiceClient mbjGraphClient = AuthenticationHelper.GetAuthenticatedClient();

            var objGraphResult = await mbjGraphClient.Me.Messages[strID].Request().GetAsync();

            return objGraphResult.ToString();
        }

        /// <summary>
        /// Send Mail immediately usign message created in method
        /// </summary>
        /// <returns></returns>
        public static async Task SendMail(String strSubject, String[] arrRecipient, String strBody, List<string> strFilePath, List<string> strFileExtension, List<string> strFileName)
        {
            //string send = null;
            //defines message object 

            List<Recipient> lstRecipients = new List<Recipient>();

            MessageAttachmentsCollectionPage attachments = new MessageAttachmentsCollectionPage();

            for (int i = 0; i <= arrRecipient.Length - 1; i++)
            {
                lstRecipients.Add(
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = arrRecipient[i]
                            }
                        }
                );
            }
            for (int i = 0; i < strFilePath.Count; i++)
            {
                byte[] contentBytes = System.IO.File.ReadAllBytes(strFilePath[i]);

                attachments.Add(new FileAttachment
                {
                    ODataType = "#microsoft.graph.fileAttachment",
                    ContentBytes = contentBytes,
                    ContentId = strFileName[i],
                    Name = strFileName[i] + strFileExtension[i]
                });

            }

            var objMessage = new Message
            {
                Subject = strSubject,
                Body = new ItemBody
                
                {
                    ContentType = BodyType.Html,
                    Content = strBody
                },
                ToRecipients = lstRecipients,

                Attachments = attachments
                //{ 
                //    new Recipient
                //    {
                //        EmailAddress = new EmailAddress
                //        {
                //            //Address = strRecipient
                //        }
                //    }
                //}
            };

            var saveToSentItems = true;

            GraphServiceClient objGraphClient = AuthenticationHelper.GetAuthenticatedClient();
            await objGraphClient.Me.SendMail(objMessage, SaveToSentItems: saveToSentItems).Request().PostAsync();
        }

        /// <summary>
        /// Send Mail immediately usign message created in method
        /// </summary>
        /// <returns></returns>
        public static async Task SendMail(String strSubject, String[] arrRecipient, String strBody)
        {
            //string send = null;
            //defines message object 
            //byte[] contentBytes = System.IO.File.ReadAllBytes(strFilePath);

            List<Recipient> lstRecipients = new List<Recipient>();
            MessageAttachmentsCollectionPage attachments = new MessageAttachmentsCollectionPage();

            for (int i = 0; i <= arrRecipient.Length - 1; i++)
            {
                lstRecipients.Add(
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = arrRecipient[i]
                            }
                        }
                );
            }
            //attachments.Add(new FileAttachment
            //{
            //    ODataType = "#microsoft.graph.fileAttachment",
            //    ContentBytes = contentBytes,
            //    ContentId = strFileName,
            //   Name = strFileName + strFileExtension
            //});

            var objMessage = new Message
            {
                Subject = strSubject,
                Body = new ItemBody

                {
                    ContentType = BodyType.Html,
                    Content = strBody
                },
                ToRecipients = lstRecipients,

                Attachments = attachments
                //{ 
                //    new Recipient
                //    {
                //        EmailAddress = new EmailAddress
                //        {
                //            //Address = strRecipient
                //        }
                //    }
                //}
            };

            var saveToSentItems = true;

            GraphServiceClient objGraphClient = AuthenticationHelper.GetAuthenticatedClient();
            await objGraphClient.Me.SendMail(objMessage, SaveToSentItems: saveToSentItems).Request().PostAsync();
        }

        /// <summary>
        /// create message and save to drafts to be sent later
        /// </summary>
        /// <returns>Message created as a string</returns>
        private static async Task<string> CreateMessage()
        {
            var objMessage = new Message
            {
                Subject = "Did you see this cool thing",
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = "They were <b>AWESOME<b>!"
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "kjmoore4@svsu.edu"
                        }
                    }
                }
            };

            GraphServiceClient objGraphClient = AuthenticationHelper.GetAuthenticatedClient();
            var objGraphResult = await objGraphClient.Me.Messages.Request().AddAsync(objMessage);
            return objGraphResult.ToString();
        }

        /// <summary>
        /// sends message by ID
        /// </summary>
        /// <returns></returns>
        private static async Task SendMessage()
        {
            GraphServiceClient objGraphClient = AuthenticationHelper.GetAuthenticatedClient();

            await objGraphClient.Me.Messages["{ID}"].Send().Request().PostAsync();
        }

        /// <summary>
        /// Get folder by ID or name
        /// </summary>
        /// <returns>Folder items as a string</returns>
        public static async Task<string> GetFolder(String strFolderName)
        {
            GraphServiceClient objGraphClient = AuthenticationHelper.GetAuthenticatedClient();

            var objGraphResult = await objGraphClient.Me.MailFolders[strFolderName].Request().GetAsync();

            return objGraphResult.ToString();
        }

    }
}

