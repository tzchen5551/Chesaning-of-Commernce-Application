using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;

namespace DirectorsPortalConstantContact
{
    /// <summary>
    /// Class to manage the OAuth process for Constant Contact
    /// </summary>
    class ConstantContactOAuth
        //[TODO] keep an eye on testing this class. after relocated the methods, the returned JS would fail to close the current tab. no idea how, but i fixed it. 
    {

        readonly private HttpListener mobjLocalListener = new HttpListener();
        private string mstrAccessToken;
        private string mstrRefreshToken;
        private string mstrLocalRoute;
        private string mstrEncodedRedirect;
        readonly private string mstrOAuthUrlPart1 = "https://idfed.constantcontact.com/as/token.oauth2?code=";
        readonly private string mstrOAuthUrlPart2 = "&grant_type=authorization_code&redirect_uri=";
        public string mstrAppAPIKey;
        public string mstrAppAPISecret;
        DateTime mobjLastUse;

        /// <summary>
        /// Constuctor to set initial params for local socket and load the ini data for key and secret
        /// </summary>
        public ConstantContactOAuth()
        {
            //sets socket at 1 minute timeout
            this.mobjLocalListener.TimeoutManager.DrainEntityBody = new TimeSpan(0, 1, 0);
            this.LoadINI();
            try
            {
                this.LoadCacheTokens();
            }
            catch (Exception)
            {
                //if there is an issue loading on start up, this will be handled by a msg box
                Console.WriteLine("Error loading ini on CCOAuth constructor");
            }
        }

        /// <summary>
        /// loads the key and secret from a file in appdata. if the file is missing it will write it with hard coded default values that should be provided to Kate. 
        /// </summary>
        private void LoadINI()
        {
            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCAPIKey.ini";

            
            if (!File.Exists(strFname))
            {
                string[] strDefaults = { "##----------DEFAULT_VALUES----------", "LocalRoute=http://localhost:40000/", "APIKey=e3c18fc5-b03f-4067-aecf-d3ccafa30d41", "APISecret=p6C90WExMXbZXHzJPvt26Q" };
                File.WriteAllLines(strFname, strDefaults, Encoding.UTF8);
            }
            foreach(string strLine in File.ReadAllLines(strFname, Encoding.UTF8))
            {
                if (!strLine.StartsWith("##"))
                {
                    if (strLine.StartsWith("LocalRoute="))
                    {
                        this.MstrLocalRoute = strLine.Split('=')[1].Trim();
                    }
                    else if (strLine.StartsWith("APIKey="))
                    {
                        this.mstrAppAPIKey = strLine.Split('=')[1].Trim();
                    }
                    else if (strLine.StartsWith("APISecret=")){
                        this.mstrAppAPISecret = strLine.Split('=')[1].Trim();
                    }

                }
            }
        }

        /// <summary>
        /// validate and assign the localroute value
        /// </summary>
        public string MstrLocalRoute
        {
            get
            {
                return this.mstrLocalRoute;
            }
            set
            {
                if (value.Contains("localhost"))
                {
                    this.mstrLocalRoute = value;
                    this.mstrEncodedRedirect = value.Remove(value.Length - 1, 1).Replace(":", "%3A").Replace("/", "%2F");
                }
                else
                {
                    throw new Exception("Invalid redirect uri for Constant Contact api V3 URL");
                }
            }
        }

        /// <summary>
        /// property for validating the Access token
        /// </summary>
        public string MstrAccessToken
        {
            get
            {
                return this.mstrAccessToken;
            }
        }

        /// <summary>
        /// property to validate refresh token (depreciated)
        /// </summary>
        public string MstrRefreshToken
        {
            get
            {
                return this.mstrRefreshToken;
            }
        }

        /// <summary>
        /// set the initial data in the httplistener and start chrome process
        /// </summary>
        public void GetAccessToken()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }

            this.SetPrefixes();

            if (!(this.mstrLocalRoute == null || this.mstrAppAPIKey == null || this.mstrAppAPISecret == null))
            {
                this.OpenChromeToRedirect();
                this.ManageListener();
            }
            else
            {
                this.LoadINI();
                if (!(this.mstrLocalRoute == null || this.mstrAppAPIKey == null || this.mstrAppAPISecret == null))
                {
                    throw new FormatException("INI File Malformed or incorrect data");
                }
            }

        }

        /// <summary>
        /// set the local hosting prefix
        /// </summary>
        private void SetPrefixes()
        {
            if (this.mstrLocalRoute != null)
            {
                this.mobjLocalListener.Prefixes.Add(this.mstrLocalRoute);
            }
        }

        /// <summary>
        /// spawn browser process to load Constant Contact login page before redirect to local
        /// </summary>
        private void OpenChromeToRedirect()
        {
            //authorization link
            try
            {
                string strAccessCodeURL = $"https://api.cc.email/v3/idfed?client_id={this.mstrAppAPIKey}&response_type=code&scope=contact_data+campaign_data+account_read+account_update&redirect_uri={this.mstrEncodedRedirect}";
                System.Diagnostics.Process.Start(strAccessCodeURL);

            }
            catch (System.ComponentModel.Win32Exception)
            {
                Console.WriteLine("oof");
            };
        }

        /// <summary>
        /// sit ant wait for redirect to local host
        /// 
        /// [TODO] This should be threaded as it blocks untill responcse.
        ///     {DONE} time out on the local socket handles this
        /// </summary>
        private void ManageListener()
        {

            // Note: The GetContext method blocks while waiting for a request.
            this.mobjLocalListener.Start();
            HttpListenerContext objHttpContext = this.mobjLocalListener.GetContext();
            this.GetAccessCode(objHttpContext.Request);

            // Obtain a response object.
            this.HandleHttpConnectionResponse(objHttpContext.Response);

            //socket was closing too fast for client, resulting in running program, but 404 on client. 
            System.Threading.Thread.Sleep(1000);

            this.mobjLocalListener.Stop();
        }


        /// <summary>
        /// retrieve the access code before access token retrieval
        ///     parse the code from the request and make new post
        /// </summary>
        /// <param name="objRequest"></param>
        private void GetAccessCode(HttpListenerRequest objRequest)
        {
            string strAccesscode = objRequest.QueryString["code"];

            string strOAuthUrl = this.mstrOAuthUrlPart1 + strAccesscode + this.mstrOAuthUrlPart2 + this.mstrEncodedRedirect;

            //Console.Write(OAuthUrl);
            string strHttpResponse = ReadOAuthContents(strOAuthUrl);
            this.ParseTokenFromHTTP(strHttpResponse);

        }

        /// <summary>
        /// handles the http responce to the localhost client
        /// responde with javescript to close tab. 
        /// </summary>
        /// <param name="objRecvResponse">responce object to write the output to</param>
        private void HandleHttpConnectionResponse(HttpListenerResponse objRecvResponse)
        {
            // Construct a response.
            string strResponse = "<script>close();</script><h1>Successful redirect, you can close this tab now.</h1>";
            byte[] bytBufferArray = Encoding.UTF8.GetBytes(strResponse);

            // Get a response stream and write the response to it.
            objRecvResponse.ContentLength64 = bytBufferArray.Length;
            Stream objResponseStream = objRecvResponse.OutputStream;
            objResponseStream.Write(bytBufferArray, 0, bytBufferArray.Length);

            // You must close the output stream.
            objResponseStream.Close();

        }

        /// <summary>
        /// create the final accesstoken post request. 
        /// </summary>
        /// <param name="strUrl">url that is to be posted to. </param>
        /// <returns> the string responce from Constant Contact</returns>
        private string ReadOAuthContents(string strUrl)
        {

            HttpWebRequest objAccessTokenRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            objAccessTokenRequest.Headers["Authorization"] = this.GenerateHeader();

            objAccessTokenRequest.Method = "POST";
            HttpWebResponse objresponse = (HttpWebResponse)objAccessTokenRequest.GetResponse();

            Stream objStreamResponse = objresponse.GetResponseStream();
            StreamReader objStreamRead = new StreamReader(objStreamResponse);
            Char[] chrBufferArray = new Char[256];
            int intCount = objStreamRead.Read(chrBufferArray, 0, 256);

            string strHttpResponse = "";

            while (intCount > 0)
            {
                String stroutputData = new String(chrBufferArray, 0, intCount);
                strHttpResponse += stroutputData;
                intCount = objStreamRead.Read(chrBufferArray, 0, 256);
            }

            return strHttpResponse;

        }

        /// <summary>
        /// paarse the access token from the str response from Constant Contact
        /// </summary>
        /// <param name="strHttpResponse">str response to parse</param>
        private void ParseTokenFromHTTP(string strHttpResponse)
        {

            Dictionary<string, string> dctDecodedJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(strHttpResponse);
            this.mstrAccessToken = dctDecodedJson["access_token"];
            this.mstrRefreshToken = dctDecodedJson["refresh_token"];

        }

        /// <summary>
        /// generate the authentication header for requests
        /// </summary>
        /// <returns>string of herader value</returns>
        private string GenerateHeader()
        {
            return "Basic " + this.Base64Encode($"{this.mstrAppAPIKey}:{this.mstrAppAPISecret}");
        }

        /// <summary>
        /// base64 encode the header as per the http authentication standard
        /// </summary>
        /// <param name="strValue">value to encode</param>
        /// <returns>encoded value</returns>
        private string Base64Encode(string strValue)
        {
            UTF8Encoding objUTF8 = new UTF8Encoding();
            byte[] bytValueArray = objUTF8.GetBytes(strValue);
            return Convert.ToBase64String(bytValueArray);
        }

        /// <summary>
        /// Function to
        /// </summary>
        public void ValidateAuthentication()
        {
            try
            {
                if (this.mstrAccessToken is null || this.mstrRefreshToken is null)
                {
                    if (!this.LoadCacheTokens())
                    {
                        this.RefreshAccessToken();
                    }

                }
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is FormatException || ex is WebException)
            {
                //on fail proceed with auth process
                this.GetAccessToken();

            }
            finally
            {
                this.mobjLastUse = DateTime.Now;
                this.CacheTokens();
            }

        }

        /// <summary>
        /// this will attempt to obtain a new access token with the current refresh token
        /// </summary>
        private void RefreshAccessToken()
        {
            string strUrl = $"https://idfed.constantcontact.com/as/token.oauth2?refresh_token={this.mstrRefreshToken}&grant_type=refresh_token";

            //attempt to refresh current token
            HttpClient client = new HttpClient();
            string strBasicAuth = $"{this.mstrAppAPIKey}:{this.mstrAppAPISecret}";
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {this.Base64Encode(strBasicAuth)}");


            HttpResponseMessage objResponse = client.PostAsync(strUrl, null).Result;


            try
            {
                Dictionary<string, string> JsonResponce = JsonConvert.DeserializeObject<Dictionary<string, string>>(objResponse.Content.ReadAsStringAsync().Result);
                this.mstrAccessToken = JsonResponce["access_token"];
                this.mstrRefreshToken = JsonResponce["refresh_token"];
            } catch (Exception ex) when (ex is Newtonsoft.Json.JsonReaderException || ex is System.Collections.Generic.KeyNotFoundException)
            {
                throw new FormatException();
            }
            

        }

        /// <summary>
        /// saves the refresh and access token to a file
        /// </summary>
        private void CacheTokens()
        {

           
            string strOutString = $"{this.mstrAccessToken}::::{this.mstrRefreshToken}::::{this.mobjLastUse.ToString()}";

            strOutString = this.ObfuscateString(strOutString);

            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCTokenCache.bin";


            File.WriteAllText(strFname, strOutString, Encoding.UTF8);
            
        }

        /// <summary>
        /// returns bool for valid access, if false, need to refresh
        /// </summary>
        private bool LoadCacheTokens()
        {

            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCTokenCache.bin";

            if (!File.Exists(strFname))
            {
                Console.WriteLine("bin cache missing");
                throw new FileNotFoundException();
            }

            string strContents = File.ReadAllText(strFname);

            strContents = this.DeobfuscateString(strContents);

            try
            {
                string[] strDelim = { "::::" , "\n"};
                string[] lstParts = strContents.Split(strDelim, StringSplitOptions.RemoveEmptyEntries);
                string strAccess = lstParts[0];
                string strRefresh = lstParts[1];
                string strTime = lstParts[2];

                this.mstrRefreshToken = strRefresh;

                //check 2 hour delta
                DateTime objDt = DateTime.Parse(strTime);
                TimeSpan objDelta = DateTime.Now - objDt;
                if (String.IsNullOrEmpty(strAccess) || String.IsNullOrEmpty(strRefresh))
                {
                    throw new Exception();
                }
                if (objDelta.TotalHours > 2)
                {
                    return false;
                }
                this.mstrAccessToken = strAccess;
                this.mstrRefreshToken = strRefresh;
                return true;


            }
            catch (Exception)
            {
                throw new FormatException();
            }
            
        }

        /// <summary>
        /// delete constant contact log in token file
        /// </summary>
        public void LogOut()
        {
            string strFname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ChamberOfCommerce\\DirectorsPortal\\CCTokenCache.bin";
            if (File.Exists(strFname)){
                File.Delete(strFname);
                this.mstrAccessToken = null;
                this.mstrRefreshToken = null;
            }
        }

        /// <summary>
        /// obfuscates a string for security
        /// </summary>
        /// <param name="strData">string to obfuscate</param>
        /// <returns></returns>
        private string ObfuscateString(string strData)
        {

            string strB64 = this.Base64Encode(strData);
            int c = 0;
            for (int i = 0; i < 3; i++)
            {
                //Console.WriteLine(c);
                strB64 = strB64.Substring(strB64.Length - 1) + strB64.Substring(0, strB64.Length - 1);
                strB64 = this.Base64Encode(strB64);
                if (strB64.Contains("="))
                {
                    i--;
                }
                if (c >= 27)
                {
                    i += 3;
                }
                c++;
            }
            //Console.WriteLine(strB64);

            return strB64;
        }

        /// <summary>
        /// undoes this.ObfuscateString()
        /// </summary>
        /// <param name="strData">string to clean</param>
        /// <returns></returns>
        private string DeobfuscateString(string strData)
        {
            string strClean = strData;
            int c = 0;
            while (true)
            {
                //Console.WriteLine(c++);
                c++;
                strClean = this.Base64Decode(strClean);
                if (strClean.Contains("::::") || c>= 300)
                {
                    break;
                }
                else
                {
                    strClean = strClean.Substring(1) + strClean.Substring(0, 1);
                }
            }
            //Console.WriteLine(strClean);
            return strClean;
        }

        /// <summary>
        /// base64 decode a string to serialize
        /// </summary>
        /// <param name="strData">string to serialize</param>
        /// <returns></returns>
        private string Base64Decode(string strData)
        {
            byte[] lstBytes = Convert.FromBase64String(strData);
            return Encoding.UTF8.GetString(lstBytes);
        }

    }
}
