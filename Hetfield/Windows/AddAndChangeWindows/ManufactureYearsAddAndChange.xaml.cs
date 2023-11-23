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
    public partial class ManufactureYearsAddAndChange : Window
    {

        ManufactureYearsPage _page;
        public ManufactureYearsAddAndChange(ManufactureYearsPage page)
        {
            InitializeComponent();
            _page = page;
            YearNumberBox.Max = DateTime.Now.Year;
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
            if (DbUtils.db.ManufactureYears.ToList().Any(ce => ce.YearValue == Convert.ToInt32(YearNumberBox.Value)))
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
                ManufactureYears year = new ManufactureYears();

                year.YearValue = Convert.ToInt32(YearNumberBox.Value);
                DbUtils.AddData(year);
                DbUtils.SaveChanges();
                _page.ManufactureYearsDataGrid.ItemsSource = DbUtils.GetTableAllValues<ManufactureYears>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
