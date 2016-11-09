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
        public NotesWindow(DataContext dataContext, users currentUser)
        {
            InitializeComponent();
            this.dataContext = dataContext;
            this.currentUser = currentUser;
        }

    }
}
