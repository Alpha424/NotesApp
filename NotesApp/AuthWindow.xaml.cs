using System.Linq;
using System.Windows;

namespace NotesApp
{
    public partial class AuthWindow : Window
    {
        private DBModelDataContext dataContext = null;
        public AuthWindow()
        {
            InitializeComponent();
            username_field.TextChanged += (sender, args) => loginButton.IsEnabled =
                        registerButton.IsEnabled = username_field.Text.Length > 0 && password_field.Password.Length > 0;
            password_field.PasswordChanged += (sender, args) => loginButton.IsEnabled =
                        registerButton.IsEnabled = username_field.Text.Length > 0 && password_field.Password.Length > 0;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataContext.users.Any(u => u.username == username_field.Text && u.password == password_field.Password))
            {
                //success
            }
            else
            {
                MessageBox.Show("Invalid username or password!", 
                    "ERROR", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataContext.users.Any(u => u.username == username_field.Text))
            {
                var user = new users
                {
                    username = username_field.Text,
                    password = password_field.Password
                };
                dataContext.users.InsertOnSubmit(user);
                dataContext.SubmitChanges();
            }
            else
            {
                MessageBox.Show("User with this username already exists!",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
