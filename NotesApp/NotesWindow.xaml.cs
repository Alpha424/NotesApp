using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NotesApp
{
    /// <summary>
    /// Логика взаимодействия для NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        private DBModelDataContext dataContext = null;
        private users currentUser = null;
        private bool _unsavedChanges = false;
        

        private notes selectedNote
        {
            get { return notesListBox.SelectedIndex == -1 ? null : availableNotes[notesListBox.SelectedIndex]; }
        }
        private bool unsavedChanges
        {
            get { return selectedNote != null && !selectedNote.text.Equals(noteContentBox.Text); }
        }
        private List<notes> availableNotes = null;
        public void reloadNotesList()
        {
            try
            {
                notesListBox.Items.Clear();
                availableNotes.Clear();
                availableNotes = currentUser.GetAvailableNotes(dataContext, true) as List<notes>;
                if (availableNotes == null) return;
                availableNotes.Sort((n1, n2) => DateTime.Compare(n2.lastedit.Value, n1.lastedit.Value));
                foreach (notes note in availableNotes)
                {
                    notesListBox.Items.Add(new
                    {
                        Content = note.text,
                        LastEditDateTime = $"Last edit: {note.lastedit}"
                    });
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (unsavedChanges)
            {
                var res = MessageBox.Show("You have unsaved changes.\nDo you want to save changes?", "Unsaved changes",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    selectedNote.text = noteContentBox.Text;
                    selectedNote.lastedit = DateTime.Now;
                    dataContext.SubmitChanges();
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
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
            try
            {
                dataContext.notes.InsertOnSubmit(newNote);
                dataContext.SubmitChanges();
                reloadNotesList();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            notesListBox.SelectedIndex = 0;
            noteContentBox.Focus();
        }

        private void saveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            dataContext.Refresh(RefreshMode.OverwriteCurrentValues, selectedNote);
            dataContext.Refresh(RefreshMode.OverwriteCurrentValues, dataContext.shared);
            selectedNote.text = noteContentBox.Text;
            selectedNote.lastedit = DateTime.Now;
            
            try
            {
                if(selectedNote.createdby == currentUser.id || dataContext.shared.Any(sh => sh.noteid == selectedNote.id && sh.sharedwith == currentUser.id))
                {
                    dataContext.SubmitChanges();
                } else
                {
                    MessageBox.Show("You don't have permission to change this note!", "Access denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    refresh(null, null);
                }
                

            } catch (ChangeConflictException ex)
            {
                foreach(ObjectChangeConflict changeConflict in dataContext.ChangeConflicts)
                {
                    foreach(MemberChangeConflict memberChangeConflict in changeConflict.MemberConflicts)
                    {
                        memberChangeConflict.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                }
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            reloadNotesList();
        }

        private void deleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure you want to delete selected note?", "",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                dataContext.notes.DeleteOnSubmit(selectedNote);
                dataContext.shared.DeleteAllOnSubmit(from sh in dataContext.shared
                    where sh.noteid == selectedNote.id
                    select sh);
                dataContext.SubmitChanges();
                reloadNotesList();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (unsavedChanges)
            {
                var res = MessageBox.Show("You have unsaved changes.\nDo you want to save changes?", "Unsaved changes",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    selectedNote.text = noteContentBox.Text;
                    selectedNote.lastedit = DateTime.Now;
                    dataContext.SubmitChanges();
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            try
            {
                dataContext.Dispose();
                dataContext = null;
                dataContext = new DBModelDataContext();
                reloadNotesList();
                notesListBox.SelectedIndex = -1;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
