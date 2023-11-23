using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.Pages;
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

namespace Hetfield.Windows.AddAndChangeWindows
{
    /// <summary>
    /// Interaction logic for OrdersAddAndChange.xaml
    /// </summary>
    public partial class OrdersAddAndChange : Window
    {
        bool _changeMode;

        int id;

        Orders PreviewOrderInChangeMode;

        OrdersPage _page;
        public OrdersAddAndChange(OrdersPage page, Orders order = null)
        {
            InitializeComponent();
            _page = page;
            
            _changeMode = order == null? false : true;
            if (order == null)
                InitComboBoxes();
            else
                InitComboBoxes(order);
        }

        private void InitComboBoxes()
        {
            ClientComboBox.ItemsSource = DbUtils.GetSearchingValues<Users>(DbUtils.Roles.Client);
            ClientComboBox.SelectedIndex = 0;
            OwnerComboBox.ItemsSource = DbUtils.db.Users.ToList().
                                        Where(u => u.CarsPassport.Count() != 0).
                                        ToList().
                                        Where(u => u.CarsPassport.ToList().
                                                Where(cp => cp.Cars.ToList().Where(c => c.IdCarStatusNavigation.CarStatusName
                                                != DbUtils.CarStatuses.Saled).Count() != 0).Count() != 0).ToList();
            OwnerComboBox.SelectedIndex = 0;
            StaffComboBox.ItemsSource = DbUtils.GetSearchingValues<Users>(DbUtils.Roles.SalesManager);
            StaffComboBox.SelectedIndex = 0;
            OrderStatusComboBox.ItemsSource = DbUtils.GetTableAllValues<OrderStatuses>();
            OrderStatusComboBox.SelectedIndex = 0;
        }

        private void InitComboBoxes(Orders order)
        {
            ClientComboBox.ItemsSource = DbUtils.GetSearchingValues<Users>(DbUtils.Roles.Client);
            OwnerComboBox.ItemsSource = DbUtils.db.Users.ToList().
                                        Where(u => u.CarsPassport.Count() != 0).
                                        ToList().
                                        Where(u => u.CarsPassport.ToList().
                                                Where(cp => cp.Cars.ToList().Where(c => c.IdCarStatusNavigation.CarStatusName
                                                != DbUtils.CarStatuses.Saled).Count() != 0 || 
                                                cp.IdOwnerNavigation == order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation).Count() != 0).ToList();
            StaffComboBox.ItemsSource = DbUtils.GetSearchingValues<Users>(DbUtils.Roles.SalesManager);
            OrderStatusComboBox.ItemsSource = DbUtils.GetTableAllValues<OrderStatuses>();
            InitOrder(order);
        }

        private void InitOrder(Orders order)
        {
            PreviewOrderInChangeMode = order;
            id = order.IdOrder;
            ClientComboBox.SelectedItem = order.IdBuyerNavigation;
            StaffComboBox.SelectedItem = order.IdStaffNavigation;
            OrderStatusComboBox.SelectedItem = order.IdOrderStatusNavigation;
            OwnerComboBox.SelectedItem = order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation;
            CarsComboBox.SelectedItem = order.IdCarNavigation;
            PriceTextBox.Text = order.FinalPrice.ToString();
            OrderDatePicker.SelectedDate = order.DateOfOrder;
        }

        private void Minim_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void OwnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => SelectCar();

        private void SelectCar()
        {
            CarsComboBox.ItemsSource = DbUtils.db.Cars.ToList().
                Where(c => c.IdCarPassportNavigation.IdOwnerNavigation == OwnerComboBox.SelectedItem as Users)
                .ToList().Where(c => c.IdCarStatusNavigation.CarStatusName == DbUtils.CarStatuses.InProcessing 
                || c.IdCarStatusNavigation.CarStatusName == DbUtils.CarStatuses.Exposed || c == PreviewOrderInChangeMode.IdCarNavigation)
                .ToList();
            if (CarsComboBox.Items.Count == 0)
                return;
            if (CarsComboBox.SelectedItem == null)
                CarsComboBox.SelectedIndex = 0;
        }

        private void CarsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CarsComboBox.Items.Count == 0)
                return;
            if(CarsComboBox.SelectedItem == null)
                CarsComboBox.SelectedIndex = 0;
            if ((bool)IsPriceLikeCarPrice.IsChecked)
                PriceTextBox.Text = (CarsComboBox.SelectedItem as Cars).Price.ToString();
        }

        private bool Validate()
        {
            if(!decimal.TryParse(PriceTextBox.Text, out decimal check))
            {
                new MessageBoxWindow("Цена не введена или введена некорректно").ShowDialog();
                return false;
            }
            if(CarsComboBox.SelectedItem == null)
            {
                new MessageBoxWindow("Машина не выбрана").ShowDialog();
                return false;
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validate())
                    return;

                Orders order;

                if (_changeMode)
                    order = DbUtils.db.Orders.First(o => o.IdOrder == id);
                else
                    order = new Orders();

                if (_changeMode && PreviewOrderInChangeMode.IdCarNavigation != CarsComboBox.SelectedItem as Cars)
                {
                    Cars car = DbUtils.db.Cars.First(c => c == PreviewOrderInChangeMode.IdCarNavigation);
                    car.IdCarStatus = DbUtils.db.CarStatuses
                        .First(cs => cs.CarStatusName == DbUtils.CarStatuses.Exposed).IdCarStatus;
                    DbUtils.SaveChanges();
                }

                order.IdStaff = (StaffComboBox.SelectedItem as Users).IdUser;
                order.IdBuyer = (ClientComboBox.SelectedItem as Users).IdUser;
                order.FinalPrice = decimal.Parse(PriceTextBox.Text);
                order.DateOfOrder = OrderDatePicker.SelectedDate.Value;
                order.IdCar = (CarsComboBox.SelectedItem as Cars).IdCar;
                order.IdCarNavigation = CarsComboBox.SelectedItem as Cars;
                order.IdOrderStatus = (OrderStatusComboBox.SelectedItem as OrderStatuses).IdOrderStatus;
                order.IdOrderStatusNavigation = OrderStatusComboBox.SelectedItem as OrderStatuses;

                if (order.IdOrderStatusNavigation.OrderStatusName == DbUtils.OrderStatuses.InProcessing)
                    order.IdCarNavigation.IdCarStatus = DbUtils.db.CarStatuses
                        .First(cs => cs.CarStatusName == DbUtils.CarStatuses.InProcessing).IdCarStatus;
                else if (order.IdOrderStatusNavigation.OrderStatusName == DbUtils.OrderStatuses.Finished)
                    order.IdCarNavigation.IdCarStatus = DbUtils.db.CarStatuses
                        .First(cs => cs.CarStatusName == DbUtils.CarStatuses.Saled).IdCarStatus;
                else if (order.IdOrderStatusNavigation.OrderStatusName == DbUtils.OrderStatuses.Deleted)
                    order.IdCarNavigation.IdCarStatus = DbUtils.db.CarStatuses
                        .First(cs => cs.CarStatusName == DbUtils.CarStatuses.Exposed).IdCarStatus;



                if (!_changeMode)
                    DbUtils.AddData(order);

                DbUtils.SaveChanges();
                _page.OrdersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Orders>();
                new MessageBoxWindow("Изменения прошли успешно").ShowDialog();
                this.Close();

            }
            catch(Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }

        private void IsPriceLikeCarPrice_Checked(object sender, RoutedEventArgs e) => PriceTextBox.Text = (CarsComboBox.SelectedItem as Cars).Price.ToString();
    }
}
