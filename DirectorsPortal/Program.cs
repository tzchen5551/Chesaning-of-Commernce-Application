using DirectorsPortalConstantContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectorsPortal
{
    static class Program
    {
        /// <summary>
        /// entry point for testing Constant Contact Lib
        /// </summary>
        [STAThread]
        static void Main()
        {
            ///Application.EnableVisualStyles();
            ///Application.SetCompatibleTextRenderingDefault(false);
            ///Application.Run(new Form1());
            ///

            //Constant Contact Dev Account
            //Username: edwalk@svsu.edu
            //password: ayC&Aybab6sC422
            //
            // yes this is intentional, this is an accoutn we can all use for dev




            ConstantContact CC = new ConstantContact();
            CC.Authenticate();


            /*Update Contact
            Contact c = CC.FindContactByEmail("aamodt@example.com");
            c.company_name = "walmart";
            Console.WriteLine(c.contact_id);
            CC.Update(c);

            */

            /*update list 
            ContactList cl = CC.FindListByName("usable");
            cl.name = "Usable List";
            CC.Update(cl);
            */



            //add campaign
            //CC.AddCampaign();


            Console.WriteLine("pause");

        }
    }
}
