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
    public partial class CarEnginesAddAndChange : Window
    {
        bool _changeMode;

        CarEnginePage _page;

        int id;
        public CarEnginesAddAndChange(CarEnginePage page, CarEngines engines = null)
        {
            InitializeComponent();
            _page = page;
            if(engines != null)
            {
                id = engines.IdCarEngine;
                CarEngineNameTextBox.Text = engines.EngineName;
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
            if(CarEngineNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Введите название двигателя").ShowDialog();
                return false;
            }
            if(DbUtils.db.CarEngines.ToList().Any(ce => Helper.DbCompare(ce.EngineName, CarEngineNameTextBox.Text) && ce.IdCarEngine != id))
            {
                new MessageBoxWindow("Такой двигатель уже существет").ShowDialog();
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
                CarEngines carEngines;
                if (_changeMode)
                    carEngines = DbUtils.db.CarEngines.FirstOrDefault(c => c.IdCarEngine == id);
                else
                    carEngines = new CarEngines();

                carEngines.EngineName = CarEngineNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(carEngines);

                DbUtils.SaveChanges();
                
                _page.CarEnginesDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarEngines>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
