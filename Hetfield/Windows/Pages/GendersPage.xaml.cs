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
    /// Interaction logic for Roles.xaml
    /// </summary>
    public partial class GendersPage : Page
    {
        public GendersPage()
        {
            InitializeComponent();
            GendersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Genders>();
        }

        private void SearchTextBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            GendersDataGrid.ItemsSource = DbUtils.GetSearchingValues<Genders>(SearchTextBox.Text);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 2000);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new GendersAddAndChange(this).ShowDialog();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var g = (sender as FrameworkElement).DataContext as Genders;
            new GendersAddAndChange(this, g).ShowDialog();
        }
    }
}
