using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private notes selectedNote
        {
            get { return notesListBox.SelectedIndex == -1 ? null : availableNotes[notesListBox.SelectedIndex]; }
        }

        private List<notes> availableNotes = null;
        public void reloadNotesList()
        {
            notesListBox.Items.Clear();
            availableNotes.Clear();
            availableNotes = currentUser.GetAvailableNotes(dataContext, true) as List<notes>;
            if(availableNotes == null) return;
            availableNotes.Sort((n1, n2) => DateTime.Compare(n2.lastedit.Value, n1.lastedit.Value));
            foreach (notes note in availableNotes)
            {
                notesListBox.Items.Add(new
                {
                    Content = note.text,
                    LastEditDateTime = $"Last edit: {note.lastedit}"
                });
            }


        }
        public NotesWindow(DBModelDataContext dataContext, users currentUser)
        {
            InitializeComponent();
            availableNotes = new List<notes>();
            noteContentGrid.Visibility = Visibility.Hidden;
            this.dataContext = dataContext;
            this.currentUser = currentUser;
            usernameLabel.Content = currentUser.username;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reloadNotesList();
        }
        private void notesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedIndex == -1)
            {
                noteContentGrid.Visibility = Visibility.Hidden;
                deleteNoteButton.IsEnabled = false;
            }
            else
            {
                noteContentGrid.Visibility = Visibility.Visible;
                deleteNoteButton.IsEnabled = selectedNote.createdby == currentUser.id;
                noteContentBox.Text = selectedNote.text;
                lasteditLabel.Content = $"Last edit: {selectedNote.lastedit}";
                if (selectedNote.createdby == currentUser.id)
                {
                    createdorsharedbyLabel.Content = "Created by you";
                    shareButton.Visibility = Visibility.Visible;
                }
                else
                {
                    createdorsharedbyLabel.Content = string.Format("Shared by: {0}", selectedNote.createdby.GetUsernameByID(dataContext));
                    shareButton.Visibility = Visibility.Hidden;
                }
            }
            
        }

        private void shareButton_Click(object sender, RoutedEventArgs e)
        {
            SharingOptionsDialog sharingOptionsDialog = new SharingOptionsDialog(dataContext, selectedNote, currentUser);
            sharingOptionsDialog.ShowDialog();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
             new AuthWindow().Show();
             Close();
        }

        private void createNoteButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime nowTime = DateTime.Now;
            notes newNote = new notes
            {
                createdby = currentUser.id,
                lastedit = nowTime,
                text = string.Empty
            };
            dataContext.notes.InsertOnSubmit(newNote);
            dataContext.SubmitChanges();
            dataContext.Refresh(RefreshMode.OverwriteCurrentValues);
            reloadNotesList();
            notesListBox.SelectedIndex = 0;
        }

        private void saveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            selectedNote.text = noteContentBox.Text;
            selectedNote.lastedit = DateTime.Now;
            dataContext.SubmitChanges();
            reloadNotesList();
        }

        private void deleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure you want to delete selected note?", null,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                dataContext.notes.DeleteOnSubmit(selectedNote);
                dataContext.SubmitChanges();
                reloadNotesList();
            }
        }
    }
}
