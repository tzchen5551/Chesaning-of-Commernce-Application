using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DirectorPortalDatabase;
using DirectorPortalDatabase.Models;
using DirectorsPortalWPF.SettingsUI;

/// <summary>
/// File Purpose:
///     This file defines the logic for the 'Todo' screen in the Directors Portal application. The
///     To do page displays a list of outstanding tasks that need to be completed by the end user. These
///     actions could include:
///         - Reviewing a new member data change
///         - Performing a DB backup
///         - Reviewing a new member request.
///     
/// </summary>

namespace DirectorsPortalWPF.TodoUI
{
    /// <summary>
    /// Interaction logic for TodoPage.xaml
    /// </summary>
    public partial class TodoPage : Page
    {

        private readonly DatabaseContext GdbContext;

        /// <summary>
        /// Initialize the Page and contents within the Page
        /// </summary>
        public TodoPage()
        {
            InitializeComponent();
            GdbContext = new DatabaseContext();
            List<Todo> rgTodos = GdbContext.TodoListItems.Where(e => e.MarkedAsDone.Equals(false)).ToList();

            foreach (Todo tDoCurrentTodo in rgTodos)
            {

                StackPanel sPanelCard = CreateStackPanelCard();
                StackPanel sPanelCardContent = new StackPanel();
                Button btnDone = CreateButton("Done");

                btnDone.Click += (sender, e) => MarkAsDone(sender, e, sPanelCard, tDoCurrentTodo);

                TextBlock txtBoxCardContent = new TextBlock
                {
                    Text = $"{tDoCurrentTodo.Title} - {tDoCurrentTodo.Description}",
                    Margin = new Thickness(5, 5, 5, 0)
                };

                TextBlock txtBoxCardClicker = new TextBlock
                {
                    Text = "",
                    Margin = new Thickness(5, 0, 5, 5),
                    FontSize = 10
                };

                sPanelCardContent.Children.Add(txtBoxCardContent);
                sPanelCardContent.Children.Add(txtBoxCardClicker);

                sPanelCard.Children.Add(btnDone);
                sPanelCard.Children.Add(sPanelCardContent);

                sPanelTodoList.Children.Add(sPanelCard);
                lblNumberOfTodo.Content = $"{sPanelTodoList.Children.Count} Number of TODO";
            }

            CheckNoTodo();
            btnMarkAllDone.Click += (sender, e) => MarkAllAsDone(sender, e, rgTodos);

        }

        /// <summary>
        /// Intended to show a Todo item that simply states there is no items in the Todo list only
        /// if there are no todo waiting for action in the database.
        /// </summary>
        /// <returns>Returns a StackPanel containing the contents to show a Todo item indicating there are no Todo items</returns>
        private void CheckNoTodo()
        {
            if (sPanelTodoList.Children.Count == 0)
            {
                StackPanel sPanelCard = CreateStackPanelCard();
                StackPanel sPanelCardContent = new StackPanel();

                TextBlock txtBoxCardContent = new TextBlock
                {
                    Text = "TODO is empty",
                    Margin = new Thickness(10, 10, 10, 10)
                };

                sPanelCardContent.Children.Add(txtBoxCardContent);
                sPanelCard.Children.Add(sPanelCardContent);
                lblNumberOfTodo.Content = "0 Number of TODO";

                sPanelTodoList.Children.Add(sPanelCard);
            }
        }

        /// <summary>
        /// Invokes when a Todo item's 'Done' button is clicked. Essentially marks an item as done.
        /// </summary>
        /// <param name="sender">Done button from the selected task</param>
        /// <param name="e">The Click event</param>
        /// <param name="sPanelCard">The Stack Panel card for the Todo item</param>
        private void MarkAsDone(object sender, RoutedEventArgs e, StackPanel sPanelCard, Todo tDoCurrentTodo)
        {

            tDoCurrentTodo.MarkedAsDone = true;
            GdbContext.SaveChanges();

            sPanelTodoList.Children.Remove(sPanelCard);
            sPanelCard.Children.Clear();

            lblNumberOfTodo.Content = $"{sPanelTodoList.Children.Count} Number of TODO";
            updateTodo();
            CheckNoTodo();      // If there is no Todo items, notify the end user.

            GC.Collect();           // Initiate garbage collection so rogue stack panel children isn't floating around in heap.
        }

        /// <summary>
        /// Invokes when the'Mark All As Done' button is clicked. Essentially marks all item as done.
        /// </summary>
        /// <param name="sender">Done button from the selected task</param>
        /// <param name="e">The Click event</param>
        /// <param name="rgTodoAll">All of the todo items in the database</param>
        private void MarkAllAsDone(object sender, RoutedEventArgs e, List<Todo> rgTodoAll)
        {
            foreach (Todo tDoCurrentTodo in rgTodoAll)
            {
                tDoCurrentTodo.MarkedAsDone = true;
                sPanelTodoList.Children.Clear();
                   
            }

            CheckNoTodo();          // If there is no Todo items, notify the end user.

            GdbContext.SaveChanges();
            GC.Collect();

        }

        /// <summary>
        /// Creates a new Stack Panel to be used on the 'Todo' screen. This Stack Panel represents a 'Card'
        /// That will contain the contents of a single Todo item.
        /// </summary>
        /// <returns>Returns a generated StackPanel object with pre-set formatting</returns>
        private StackPanel CreateStackPanelCard()
        {
            StackPanel sPanelNewStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Background = Brushes.White,
                Margin = new Thickness(50, 1, 50, 1)
            };

            return sPanelNewStackPanel;
        }


        /// <summary>
        /// Creates a new Button to  be used on the 'Todo' screen.
        /// </summary>
        /// <param name="strButtonText">Returns a generated Button object with pre-set formatting</param>
        /// <returns></returns>
        private Button CreateButton(string strButtonText)
        {
            Button btnNewButton = new Button()
            {
                Content = strButtonText,

                Margin = new Thickness(5, 5, 5, 5),
                Template = (ControlTemplate)Application.Current.Resources["xtraSmallButtonGrey"],
            };
            return btnNewButton;
        }



        private void updateTodo()
        {
            List<Todo> lstTodos = GdbContext.TodoListItems.Where(e => e.MarkedAsDone.Equals(false)).ToList();
            foreach (Todo tDoCurrentTodo in lstTodos)
            {

                StackPanel sPanelCard = CreateStackPanelCard();
                StackPanel sPanelCardContent = new StackPanel();
                Button btnDone = CreateButton("Done");

                btnDone.Click += (sender, e) => MarkAsDone(sender, e, sPanelCard, tDoCurrentTodo);

                TextBlock txtBoxCardContent = new TextBlock
                {
                    Text = $"{tDoCurrentTodo.Title} - {tDoCurrentTodo.Description}",
                    Margin = new Thickness(5, 5, 5, 0)
                };

                TextBlock txtBoxCardClicker = new TextBlock
                {
                    Text = "",
                    Margin = new Thickness(5, 0, 5, 5),
                    FontSize = 10
                };

                sPanelCardContent.Children.Add(txtBoxCardContent);
                sPanelCardContent.Children.Add(txtBoxCardClicker);

                sPanelCard.Children.Add(btnDone);
                sPanelCard.Children.Add(sPanelCardContent);

                sPanelTodoList.Children.Add(sPanelCard);
                lblNumberOfTodo.Content = $"{sPanelTodoList.Children.Count} Number of TODO";
            }
        }
        /// <summary>
        /// Opens a pop-up window that displays the current frames help information. 
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">The Click event</param>
        public void HelpButtonHandler(object sender, EventArgs e)
        {
            HelpUI.HelpScreenWindow helpWindow = new HelpUI.HelpScreenWindow();
            helpWindow.Show();
            helpWindow.tabs.SelectedIndex = 6;

        }
    }
}
