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
    /// Interaction logic for CarColorsAddandChange.xaml
    /// </summary>
    public partial class CarColorsAddandChange : Window
    {
        bool _canConvert;

        bool _changeMode;

        CarColorsPage page;

        int id;
        public CarColorsAddandChange(CarColorsPage page, CarColors? carColors = null)
        {
            InitializeComponent();
            this.page = page;
            if(carColors != null)
            {
                id = carColors.IdCarColors;
                ColorNameTextBox.Text = carColors.ColorName;
                HexTextBox.Text = carColors.Hex;
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

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Brush brush = (Brush) new BrushConverter().ConvertFromString(HexTextBox.Text);
                ColorView.Fill = brush;
                _canConvert = true;
            }
            catch(Exception ex)
            {
                ColorView.Fill = Brushes.Transparent;
                _canConvert = false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private bool Validate()
        {
            if (!_canConvert)
            {
                new MessageBoxWindow("Hex введен неверно").ShowDialog();
                return false;
            }
            if(ColorNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Введите название цвета").ShowDialog();
                return false;
            }
            if(DbUtils.db.CarColors.Any(p => (p.ColorName == ColorNameTextBox.Text || p.Hex == HexTextBox.Text) && p.IdCarColors != id))
            {
                new MessageBoxWindow("Такие данные уже есть в базе данных").ShowDialog();
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
                CarColors carColors;
                if (_changeMode)
                    carColors = DbUtils.db.CarColors.FirstOrDefault(c => c.IdCarColors == id);
                else
                    carColors = new CarColors();

                carColors.ColorName = ColorNameTextBox.Text;
                carColors.Hex = HexTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(carColors);

                DbUtils.SaveChanges();
                page.CarColorsDataGrid.ItemsSource = DbUtils.GetTableAllValues<CarColors>();
                Close();
                
            }
            catch(Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
