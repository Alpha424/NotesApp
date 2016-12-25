using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace NotesApp
{
    public partial class SharingOptionsDialog : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private DBModelDataContext dataContext = null;
        private notes currentNote = null;
        private users currentUser = null;
        IEnumerable<shared> sharingsToInsert = new List<shared>();
        IEnumerable<shared> sharingToDelete = new List<shared>();
        public SharingOptionsDialog(DBModelDataContext dataContext, notes currentNote, users currentUser)
        {
            try
            {
                InitializeComponent();
                usernameBox.TextChanged += (sender, args) => addButton.IsEnabled = ((TextBox)sender).Text.Trim().Length > 0;
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

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void usersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeButton.IsEnabled = usersList.HasItems && usersList.SelectedItem != null;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredUserName = usernameBox.Text.Trim();
            if (!dataContext.users.Any(user => user.username.ToLower() == enteredUserName))
            {
                MessageBox.Show("There is no user with such username!", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            if (usersList.Items.Contains(enteredUserName))
            {
                MessageBox.Show("User with such name already exists in the sharing list!", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            if (String.Equals(currentUser.username, enteredUserName, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("You cannot share notes with yourself!", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            ((List<shared>)sharingsToInsert).Add(new shared
            {
                noteid = currentNote.id,
                sharedby = currentUser.id,
                sharedwith = dataContext.users.First(user => user.username == enteredUserName).id
            });
            usersList.Items.Add(usernameBox.Text);
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataContext.shared.InsertAllOnSubmit(sharingsToInsert);
                dataContext.shared.DeleteAllOnSubmit(sharingToDelete);
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
            ((List<shared>)sharingToDelete).AddRange(dataContext.shared.Where(sh => sh.noteid == currentNote.id && sh.sharedby == currentUser.id && sh.sharedwith == deletingUserID));
            usersList.Items.RemoveAt(usersList.SelectedIndex);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            sharingsToInsert = null;
            sharingToDelete = null;
            Close();
        }
    }
}
