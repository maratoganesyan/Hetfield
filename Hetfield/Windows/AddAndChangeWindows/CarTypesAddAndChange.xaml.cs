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
    public partial class CarTypesAddAndChange : Window
    {
        bool _changeMode;

        CarTypesPage _page;

        int id;
        public CarTypesAddAndChange(CarTypesPage page, CarTypes type = null)
        {
            InitializeComponent();
            _page = page;
            if(type != null)
            {
                id = type.IdCarType;
                CarTypeNameTextBox.Text = type.TypeName;
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
            if(CarTypeNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Поля не заполнены").ShowDialog();
                return false;
            }
            if(DbUtils.db.CarTypes.ToList().Any(ce => 
                Helper.DbCompare(ce.TypeName, CarTypeNameTextBox.Text) && ce.IdCarType != id))
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
                CarTypes type;
                if (_changeMode)
                    type = DbUtils.db.CarTypes.FirstOrDefault(c => c.IdCarType == id);
                else
                    type = new CarTypes();

                type.TypeName = CarTypeNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(type);

                DbUtils.SaveChanges();
                
                _page.CarTypesDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarTypes>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
