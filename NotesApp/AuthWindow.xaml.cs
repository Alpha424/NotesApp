using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotesApp
{
    public partial class AuthWindow : Window
    {
        private DBModelDataContext dataContext = null;
        public AuthWindow()
        {
            InitializeComponent();
            if (!NetConnection.CheckForInternetConnection())
            {
                MessageBox.Show("This application requires network connection!\nApplication will be closed", "No Internet Connection", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
                return;
            }
            try
            {
                dataContext = new DBModelDataContext();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            username_field.TextChanged += (sender, args) => loginButton.IsEnabled =
                        registerButton.IsEnabled = username_field.Text.Trim().Length > 0 && password_field.Password.Trim().Length > 0;
            password_field.PasswordChanged += (sender, args) => loginButton.IsEnabled =
                        registerButton.IsEnabled = username_field.Text.Trim().Length > 0 && password_field.Password.Trim().Length > 0;
            if (Properties.Settings.Default.savePassword)
            {
                savePasswordCheckbox.IsChecked = true;
                username_field.Text = Properties.Settings.Default.username;
                password_field.Password = Properties.Settings.Default.password;
            }
        }

        private void authWith(DBModelDataContext context, users user)
        {
            Properties.Settings.Default.savePassword = savePasswordCheckbox.IsChecked.HasValue && savePasswordCheckbox.IsChecked.Value;
            if (savePasswordCheckbox.IsChecked.HasValue && savePasswordCheckbox.IsChecked.Value)
            {
                Properties.Settings.Default.username = user.username;
                Properties.Settings.Default.password = password_field.Password;
            }
            Properties.Settings.Default.Save();
            NotesWindow notesWindow = new NotesWindow(context, user);
            notesWindow.Show();
            this.Close();
        }
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = from user in dataContext.users
                          where user.username == username_field.Text && user.password == password_field.Password.HashMD5()
                          select user;

                if (res.Count() > 0)
                {
                    authWith(dataContext, res.First());
                }
                else
                {
                    MessageBox.Show("Invalid username or password!",
                        "ERROR",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dataContext.users.Any(u => u.username == username_field.Text))
                {
                    dataContext.users.InsertOnSubmit(new users
                    {
                        username = username_field.Text,
                        password = password_field.Password.HashMD5()
                    });
                    dataContext.SubmitChanges();
                    authWith(dataContext, dataContext.users.First(
                        u => u.username == username_field.Text && u.password == password_field.Password.HashMD5()));
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User with this username already exists!",
                        "ERROR",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
