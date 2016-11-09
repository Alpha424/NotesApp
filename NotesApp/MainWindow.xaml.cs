using System;
using System.Linq;
using System.Windows;

namespace NotesApp
{
    public partial class MainWindow : Window
    {
        private DBModelDataContext dataContext = null;
        public MainWindow()
        {
            InitializeComponent();
            dataContext = new DBModelDataContext();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var result = from user in dataContext.users
                where
                user.username == username_field.Text && user.password == password_field.Password
                select user;
            
            if (result.Count() > 0)
            {
                MessageBox.Show("SUCCESS!");
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль!", "");
            }
            
        }
    }
}
