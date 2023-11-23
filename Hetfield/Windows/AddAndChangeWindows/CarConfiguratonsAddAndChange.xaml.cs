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
    /// Interaction logic for CarConfigurationsAddAndChange.xaml
    /// </summary>
    public partial class CarConfigurationsAddAndChange : Window
    {
        bool _changeMode;

        CarConfigurationsPage _page;

        int id;
        public CarConfigurationsAddAndChange(CarConfigurationsPage page, CarConfigurations Configurations = null)
        {
            InitializeComponent();
            _page = page;
            if(Configurations != null)
            {
                id = Configurations.IdCarConfiguration;
                CarConfigurationsNameTextBox.Text = Configurations.CarConfigurationName;
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
            if(CarConfigurationsNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Введите название двигателя").ShowDialog();
                return false;
            }
            if(DbUtils.db.CarConfigurations.ToList().
                Any(ce => Helper.DbCompare(ce.CarConfigurationName, CarConfigurationsNameTextBox.Text) && ce.IdCarConfiguration != id))
            {
                new MessageBoxWindow("Такой запись уже существует").ShowDialog();
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
                CarConfigurations carConfigurations;
                if (_changeMode)
                    carConfigurations = DbUtils.db.CarConfigurations.FirstOrDefault(c => c.IdCarConfiguration == id);
                else
                    carConfigurations = new CarConfigurations();

                carConfigurations.CarConfigurationName = CarConfigurationsNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(carConfigurations);

                DbUtils.SaveChanges();
                
                _page.CarConfigurationsDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarConfigurations>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
