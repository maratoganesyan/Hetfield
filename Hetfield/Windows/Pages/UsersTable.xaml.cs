using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.AddAndChangeWindows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Animations;

namespace Hetfield.Windows.Pages
{
    /// <summary>
    /// Interaction logic for UsersTable.xaml
    /// </summary>
    public partial class UsersTable : Page
    {
        public UsersTable()
        {
            InitializeComponent();
            UsersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Users>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 2000);
        }

        private void SearchTextBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            UsersDataGrid.ItemsSource = DbUtils.GetSearchingValues<Users>(SearchTextBox.Text);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        { 
            UsersAddAndChange window = new UsersAddAndChange(null, false, this, true);
            window.ShowDialog();
        }

        private void ChangeUser_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as Users;
            new UsersAddAndChange(user, true, this, true).ShowDialog();
        }
    }
}
