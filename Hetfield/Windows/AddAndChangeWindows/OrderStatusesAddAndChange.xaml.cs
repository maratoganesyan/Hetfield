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
    /// Interaction logic for CarEnginesAddAndChange.xaml
    /// </summary>
    public partial class OrderStatusesAddAndChange : Window
    {
        bool _changeMode;

        OrderStatusesPage _page;

        int id;
        public OrderStatusesAddAndChange(OrderStatusesPage page, OrderStatuses status = null)
        {
            InitializeComponent();
            _page = page;
            if(status != null)
            {
                id = status.IdOrderStatus;
                OrderStatusesNameTextBox.Text = status.OrderStatusName;
                _changeMode = true;
            }
            else
            {
                id = -1;
                _changeMode = false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private bool Validation()
        {
            if(OrderStatusesNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Поля не заполнены").ShowDialog();
                return false;
            }
            if(DbUtils.db.OrderStatuses.ToList().Any(ce => 
                Helper.DbCompare(ce.OrderStatusName, OrderStatusesNameTextBox.Text) && ce.IdOrderStatus != id))
            {
                new MessageBoxWindow("Такая запись уже есть в базе").ShowDialog();
                return false;
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validation())
                    return;
                OrderStatuses status;
                if (_changeMode)
                    status = DbUtils.db.OrderStatuses.FirstOrDefault(c => c.IdOrderStatus == id);
                else
                    status = new OrderStatuses();

                status.OrderStatusName = OrderStatusesNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(status);

                DbUtils.SaveChanges();
                
                _page.OrderStatusesDataGrid.ItemsSource = DbUtils.GetTableAllValues<OrderStatuses>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
