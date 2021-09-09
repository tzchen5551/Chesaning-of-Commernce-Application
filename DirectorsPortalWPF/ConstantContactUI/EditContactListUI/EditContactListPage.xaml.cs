using DirectorsPortalConstantContact;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectorsPortalWPF.ConstantContactUI.EditContactListUI
{
    /// <summary>
    /// Interaction logic for EditContactListPage.xaml
    /// </summary>
    public partial class EditContactListPage : Page
    {
        private ConstantContact gObjConstContact;
        private ContactList clEditList;
        private string GStrListName;
        private Frame ContactListFrame;

        private ConstantContactPage objCallBackPage;

        public EditContactListPage(ConstantContact ccHelper, string GStrCList, Frame ContactListFrame, ConstantContactPage objPage)
        {
            InitializeComponent();
            gObjConstContact = ccHelper;
            GStrListName = GStrCList;
            clEditList = gObjConstContact.gdctContactLists.Values.FirstOrDefault(x => x.name.Equals(GStrCList));
            Load_Data(clEditList);
            this.ContactListFrame = ContactListFrame;
            objCallBackPage = objPage;
        }

        /// <summary>
        /// Loads pre existing information into the form
        /// </summary>
        /// <param name="clEditList"></param>
        private void Load_Data(ContactList clEditList)
        {
            txtContactListName.Text = clEditList.name;
            txtNotes.Text = clEditList.description;
            foreach (Contact item in clEditList.glstMembers)
            {
                lstContacts.Items.Add(item);
            }

        }

        /// <summary>
        /// Checks to see if a Contact List Name is already being used
        /// </summary>
        /// <param name="GStrDupeName"></param>
        /// <returns></returns>
        private Boolean checkName(string GStrDupeName)
        {
            if (GStrDupeName.Equals(clEditList.name))
                return true;
            else
                foreach(ContactList contactList in gObjConstContact.gdctContactLists.Values)
                {
                    if (GStrDupeName.Equals(contactList.name))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Writes changes made to the corresponding Contact List
        /// Returns to the Constant Contact Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_List(object sender, RoutedEventArgs e)
        {
            if (!checkName(txtContactListName.Text))
            {
                MessageBox.Show("Error Contact List name in use, please use a different name","Error");
                return;
            }
            clEditList.name = txtContactListName.Text;
            clEditList.description = txtNotes.Text;
            gObjConstContact.Update(clEditList);
            clEditList = gObjConstContact.FindListByName(txtContactListName.Text);
            foreach (Contact contactMember in lstContacts.Items)
            {
                Contact contactVal = gObjConstContact.gdctContacts.Values.FirstOrDefault(x => x.strFullname.Equals(contactMember.strFullname));
                if(!clEditList.glstMembers.Contains(contactVal))
                    gObjConstContact.AddContactToContactList(clEditList, contactVal);
            }

            for (int i = clEditList.glstMembers.Count()-1;i>=0;i--)
            {
                Contact contactVal = clEditList.glstMembers[i];
                if (!lstContacts.Items.Contains(contactVal))
                {
                    gObjConstContact.RemoveContactFromContactList(clEditList, contactVal);
                }

                    
            }

            ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, objCallBackPage));

        }

        /// <summary>
        /// Searches stored data for contacts that match the given substring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Database(object sender, TextChangedEventArgs e)
        {
            lstPopup.Items.Clear();
            string strSearchTerm = txtAddContacts.Text;

            popSearch.IsOpen = true;


            foreach (Contact objContact in gObjConstContact.gdctContacts.Values)
            {
                
                if (objContact.strFullname.ToLower().Contains(strSearchTerm.ToLower()) && !CheckAlreadyInList(objContact))
                {
                    lstPopup.Items.Add(objContact);
                }
            }
        }

        /// <summary>
        /// given a contact, check if it is already in the list
        /// </summary>
        /// <param name="objContact">Contact to check</param>
        /// <returns></returns>
        private bool CheckAlreadyInList(Contact objContact)
        {
            foreach (Contact objTemp in lstContacts.Items)
            {
                if (objContact.strFullname.Equals(objTemp.strFullname))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets called on the click of the "Remove Member" button on the edit page.
        /// Will remove member from listbox
        /// </summary>
        /// <param name="sender">The Save button object that has called the function.</param>
        /// <param name="e">The button press event</param>
        private void Remove_Contact(object sender, RoutedEventArgs e)
        {
            lstContacts.Items.Remove(lstContacts.SelectedItem);
        }

        /// <summary>
        /// Closes the search box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hide_Search(object sender, RoutedEventArgs e)
        {
            popSearch.IsOpen = false;
        }

        /// <summary>
        /// Adds contact to list box and closes the search box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Contact_To_List(object sender, SelectionChangedEventArgs e)
        {
            if (lstPopup.SelectedIndex >= 0)
            {
                foreach (Contact objContact in lstContacts.Items)
                {
                    if (objContact.strFullname!=((Contact)lstPopup.SelectedItem).strFullname)
                    {
                        lstContacts.Items.Add(lstPopup.SelectedItem);
                        txtAddContacts.Clear();
                        popSearch.IsOpen = false;
                        return;
                    }
                }
                if (lstContacts.Items.Count == 0)
                {
                    lstContacts.Items.Add(lstPopup.SelectedItem);
                    txtAddContacts.Clear();
                    popSearch.IsOpen = false;
                    return;
                }
                
            }
        }

        /// <summary>
        /// Gets called on the click of the "Cancel" button on the edit page.
        /// Will return the user to the Constant Contact Page
        /// </summary>
        /// <param name="sender">The Cancel button object that has called the function.</param>
        /// <param name="e">The button press event</param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, objCallBackPage));
        }

        /// <summary>
        /// remove a contactlist from constant contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete {txtContactListName.Text}?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                gObjConstContact.RemoveContactList(gObjConstContact.FindListByName(txtContactListName.Text));
                ContactListFrame.Navigate(new AddContactListUI.AddContactListPage(gObjConstContact, ContactListFrame, objCallBackPage));

                objCallBackPage.LoadContactLists(gObjConstContact);
            }

        }
    }
}
