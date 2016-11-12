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
using System.Windows.Shapes;

namespace NotesApp
{
    /// <summary>
    /// Логика взаимодействия для SharingOptionsDialog.xaml
    /// </summary>
    public partial class SharingOptionsDialog : Window
    {
        private DBModelDataContext dataContext = null;
        private notes currentNote = null;
        private users currentUser = null;
        public SharingOptionsDialog(DBModelDataContext dataContext, notes currentNote, users currentUser)
        {
            
            InitializeComponent();
            usernameBox.TextChanged += (sender, args) => addButton.IsEnabled = ((TextBox) sender).Text.Trim().Length > 0;
            this.dataContext = dataContext;
            this.currentUser = currentUser;
            this.currentNote = currentNote;
            var usernames = from user in dataContext.users
                join sh in this.dataContext.shared on user.id equals sh.sharedwith
                where sh.sharedby == this.currentUser.id && sh.noteid == this.currentNote.id
                select user.username;
            foreach (var username in usernames)
            {
                usersList.Items.Add(username);
            }


        }

        private void usersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeButton.IsEnabled = usersList.HasItems && usersList.SelectedItem != null;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataContext.users.Any(user => user.username == usernameBox.Text))
            {
                MessageBox.Show("There is no user with such username!", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            if (usersList.Items.Contains(usernameBox.Text))
            {
                MessageBox.Show("User with such name already exists in the sharing list!", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            dataContext.shared.InsertOnSubmit(new shared
            {
                noteid = currentNote.id,
                sharedby = currentUser.id,
                sharedwith = dataContext.users.First(user => user.username == usernameBox.Text).id
            });
            usersList.Items.Add(usernameBox.Text);
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Close();
            }
            
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            int deletingUserID =
                dataContext.users.First(user => user.username == (string)usersList.Items[usersList.SelectedIndex]).id;
            dataContext.shared.DeleteAllOnSubmit(dataContext.shared.Where(sh => sh.noteid == currentNote.id && sh.sharedby == currentUser.id && sh.sharedwith == deletingUserID));
            usersList.Items.RemoveAt(usersList.SelectedIndex);
        }
    }
}
