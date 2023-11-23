using Hetfield.Tools;
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
using System.Windows.Shapes;
using Wpf.Ui.Animations;
using Hetfield.Tools.ModelsForGeneration;
using Hetfield.Entities;

namespace Hetfield.Windows
{
    /// <summary>
    /// Interaction logic for ReportsGenerateWindow.xaml
    /// </summary>
    public partial class ReportsGenerateWindow : Window
    {
        enum ReportType
        {
            Staff,
            Order
        }

        ReportType type;
        public ReportsGenerateWindow()
        {
            InitializeComponent();
            StaffComboBox.ItemsSource = DbUtils.db.Users.ToList().Where(u => u.IdRoleNavigation.RoleName == DbUtils.Roles.SalesManager).ToList();
            StaffComboBox.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 2000);

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void OrdersMI_Click(object sender, RoutedEventArgs e)
        {
            StaffComboBox.Visibility = Visibility.Collapsed;
            OrdersMI.Opacity = 0.7;
            StaffMI.Opacity = 1;
            type = ReportType.Order;
        }

        private void StaffMI_Click(object sender, RoutedEventArgs e)
        {
            StaffComboBox.Visibility = Visibility.Visible;
            OrdersMI.Opacity = 1;
            StaffMI.Opacity = 0.7;
            type = ReportType.Staff;
        }

        private void GenerateReprotButton_Click(object sender, RoutedEventArgs e)
        {
            DateOnly startDate = new DateOnly(StartDatePicker.SelectedDate!.Value.Year, StartDatePicker.SelectedDate.Value.Month, StartDatePicker.SelectedDate.Value.Day);
            DateOnly endDate = new DateOnly(EndDatePicker.SelectedDate!.Value.Year, EndDatePicker.SelectedDate.Value.Month, EndDatePicker.SelectedDate.Value.Day);
            if (type == ReportType.Order)
            {
                var orders = DbUtils.db.Orders.ToList()
                    .Where(o => o.DateOfOrder <= EndDatePicker.SelectedDate && StartDatePicker.SelectedDate <= o.DateOfOrder).ToList();
                if(orders.Count() == 0)
                {
                    new MessageBoxWindow("сделки в данный промежуток времени отсутсвуют").ShowDialog();
                    return;
                }
               
                ReportGeneration.DoAWorkReportAsync(new WorkReportModel(orders, startDate, endDate), this, LoadProgressRing);
            }
            else
            {
                
                var staff = StaffComboBox.SelectedItem as Users;
                var ordersOfManager = staff.OrdersIdStaffNavigation.ToList()
                    .Where(o => o.DateOfOrder < EndDatePicker.SelectedDate && StartDatePicker.SelectedDate < o.DateOfOrder).ToList();
                if (ordersOfManager.Count() == 0)
                {
                    new MessageBoxWindow("Данный сотрудник не совершал сделок в данный промежуток времени").ShowDialog();
                    return;
                }
                ReportGeneration.DoAEmployeeReportAsync(new EmployeeReportModel(staff, ordersOfManager, startDate, endDate), this, LoadProgressRing);
            }
        }
    }
}
