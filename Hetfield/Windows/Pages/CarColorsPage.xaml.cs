using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.AddAndChangeWindows;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class CarColorsPage : Page
    {
        public CarColorsPage()
        {
            InitializeComponent();
            CarColorsDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarColors>();
        }

        private void SearchTextBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            CarColorsDataGrid.ItemsSource = DbUtils.GetSearchingValues<CarColors>(SearchTextBox.Text);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.SlideLeft, 2000);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new CarColorsAddandChange(this).ShowDialog();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var carColors = (sender as FrameworkElement).DataContext as CarColors;
            new CarColorsAddandChange(this, carColors).ShowDialog();
        }
    }
}
