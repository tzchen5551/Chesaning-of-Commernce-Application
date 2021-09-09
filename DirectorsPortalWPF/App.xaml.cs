using System;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Linq;

namespace DirectorsPortalWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppDomain Domain;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public App()
        {
            this.Domain = AppDomain.CurrentDomain;
            this.Domain.UnhandledException += new UnhandledExceptionEventHandler(HandleUncaught);
               
        }
        static void HandleUncaught(object sender, UnhandledExceptionEventArgs args)
        {
            string strBasename = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string strFname = strBasename + "\\DirectorsPortalUnhandled_Error.txt";

            long size;
            //rollover to a new file after 10 mb file size
            string strFLine = "Current Rollover Count = 0";

            if (File.Exists(strFname)){
                FileInfo objInfo = new FileInfo(strFname);
                size = (objInfo.Length/1024)/1024;
                strFLine = File.ReadLines(strFname).First();
                if (size >= 10)
                {

                    //current rollover count
                    int intCount = -2;
                    string strError = "";
                    try
                    {
                        intCount = Convert.ToInt32(strFLine.Split('=')[1].Trim());
                    }
                    catch (Exception)
                    {
                        strError = " :: ERROR";
                    }

                    strFLine = strFLine.Split('=')[0] + '=' + ++intCount + strError;
                    File.WriteAllText(strFname, strFLine);
                    File.AppendAllText(strFname, "\n\n");
                }
            }
            else
            {
                File.WriteAllText(strFname, strFLine);
                File.AppendAllText(strFname, "\n\n");
            }

            Exception e = (Exception)args.ExceptionObject;
            string strTime = DateTime.Now.ToString();
            File.AppendAllText(strFname, $"------------------------------BEGIN {strTime}------------------------------\n");
            File.AppendAllText(strFname, $"{e.Message}\n");
            File.AppendAllText(strFname, $"{e.StackTrace}\n");
            File.AppendAllText(strFname, $"------------------------------END {strTime}--------------------------------\n");
        }
    }
    
}
