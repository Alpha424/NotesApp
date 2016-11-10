using System.Data.Linq;
using System.Windows;

namespace NotesApp
{
    /// <summary>
    /// Логика взаимодействия для NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        private DataContext dataContext = null;
        private users currentUser = null;

        public void reloadNotesList()
        {
            notesListBox.Items.Clear();
            foreach (notes userNote in currentUser.notes)
            {
                notesListBox.Items.Add(new
                {
                    Content = userNote.text,
                    LastEditDateTime = userNote.lastedit
                });
            }
        }
        public NotesWindow(DataContext dataContext, users currentUser)
        {
            InitializeComponent();
            this.dataContext = dataContext;
            this.currentUser = currentUser;
            this.Title = $"Notes - {currentUser.username}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reloadNotesList();
        }
    }
}
