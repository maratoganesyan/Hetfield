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
    public partial class CarStatusesAddAndChange : Window
    {
        bool _changeMode;

        CarStatusesPage _page;

        int id;
        public CarStatusesAddAndChange(CarStatusesPage page, CarStatuses status = null)
        {
            InitializeComponent();
            _page = page;
            if(status != null)
            {
                id = status.IdCarStatus;
                CarStatusNameTextBox.Text = status.CarStatusName;
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
            if(CarStatusNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Поля не заполнены").ShowDialog();
                return false;
            }
            if(DbUtils.db.CarStatuses.ToList().Any(ce => Helper.DbCompare(ce.CarStatusName, CarStatusNameTextBox.Text) && ce.IdCarStatus != id))
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
                CarStatuses carStatus;
                if (_changeMode)
                    carStatus = DbUtils.db.CarStatuses.FirstOrDefault(c => c.IdCarStatus == id);
                else
                    carStatus = new CarStatuses();

                carStatus.CarStatusName = CarStatusNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(carStatus);

                DbUtils.SaveChanges();
                
                _page.CarStatusesDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarStatuses>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
