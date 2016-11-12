using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotesApp
{
    /// <summary>
    /// Логика взаимодействия для NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        private DBModelDataContext dataContext = null;
        private users currentUser = null;
        private notes selectedNote = null;
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
        public NotesWindow(DBModelDataContext dataContext, users currentUser)
        {
            InitializeComponent();
            noteContentGrid.Visibility = Visibility.Hidden;
            this.dataContext = dataContext;
            this.currentUser = currentUser;
            this.Title = $"Notes - {currentUser.username}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reloadNotesList();
        }
        private void notesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedNote = currentUser.notes[((ListBox) sender).SelectedIndex];
            if(noteContentGrid.Visibility == Visibility.Hidden) noteContentGrid.Visibility = Visibility.Visible;
            noteContentBox.Text = selectedNote.text;
            lasteditLabel.Content = selectedNote.lastedit;
            if (selectedNote.createdby == currentUser.id)
            {
                createdorsharedbyLabel.Content = "Created by you";
                shareButton.Visibility = Visibility.Visible;
            }
            else
            {
                createdorsharedbyLabel.Content = string.Format("Shared by: {0}", from u in dataContext.users
                                                                                 where u.id == selectedNote.createdby
                                                                                 select u.username);
                shareButton.Visibility = Visibility.Hidden;
            }
        }

        private void shareButton_Click(object sender, RoutedEventArgs e)
        {
            SharingOptionsDialog sharingOptionsDialog = new SharingOptionsDialog(dataContext, selectedNote, currentUser);
            sharingOptionsDialog.ShowDialog();
        }
    }
}
