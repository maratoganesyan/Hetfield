using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Tools.ModelsForGeneration;
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
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        EmployeeWindow window;
        public OrdersPage(EmployeeWindow window)
        {
            InitializeComponent();
            OrdersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Orders>();
            this.window = window;
        }

        private void ChangeOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = (sender as FrameworkElement).DataContext as Orders;
            new OrdersAddAndChange(this, order).Show();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 2000);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new OrdersAddAndChange(this).Show();
        }

        private void SearchTextBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            OrdersDataGrid.ItemsSource = DbUtils.GetSearchingValues<Orders>(SearchTextBox.Text);
        }

        private void SalesContractGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var order = (sender as FrameworkElement).DataContext as Orders;
            ReportGeneration.DoAPaidContractAsync(new PaidContractsModel(order), window, window.LoadRing);
            OrdersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Orders>();
        }

    }
}
