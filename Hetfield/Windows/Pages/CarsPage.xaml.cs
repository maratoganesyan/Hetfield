using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.AddAndChangeWindows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hetfield.Windows.Pages
{
    /// <summary>
    /// Interaction logic for CarsPage.xaml
    /// </summary>
    public partial class CarsPage : Page
    {

        int countToSkip;

        int allCarsCount;

        List<Cars> carsToOutput;

        CarStatuses carStatuses;
        public CarsPage()
        {
            InitializeComponent();
            CarStatusComboBox.ItemsSource = DbUtils.GetTableAllValues<CarStatuses>();
            UpdateSearchingCarsCount();
        }

        private void UpdateCarsCount(bool left)
        {
            if(left)
                countToSkip -= 3;
            else
                countToSkip += 3;
            if (countToSkip < 0)
            {
                countToSkip = 0;
                return;
            }
            if (countToSkip >= allCarsCount)
            {
                countToSkip -= 3;
                return;
            }
            OutputPruduct.ItemsSource = carsToOutput
                                       .Skip(countToSkip)
                                       .Take(3)
                                       .ToList();
        }

        public async void UpdateSearchingCarsCount()
        {
            countToSkip = -3;
            bool EnableCarStatus = !EnableCarStatusToogleSwitch.IsChecked.Value;
            string text = SearchTextBox.Text.ToLower();
            carsToOutput = await Task.Run(() => DbUtils.db.Cars.ToList()
                .Where(p => p.ToString().ToLower().Contains(text)).ToList()
                .Where(c => c.IdCarStatusNavigation == carStatuses || EnableCarStatus)
                .ToList());
            allCarsCount = carsToOutput.Count();
            UpdateCarsCount(false);
        }

        private void SearchTextBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            UpdateSearchingCarsCount();
        }

        private void GetCarPassport_Click(object sender, RoutedEventArgs e)
        {
            var car = ((sender as FrameworkElement).DataContext as Cars);
            var window = new CarPassportWindow(car.IdCarPassportNavigation);
            window.ShowDialog();
        }

        private void AboutCar_Click(object sender, RoutedEventArgs e)
        {
            var car = ((sender as FrameworkElement).DataContext as Cars);
            new CarMoreInformationWindow(car).Show();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new CarsAddAndChange(this).Show();
        }

        private void ChangeCar_Click(object sender, RoutedEventArgs e)
        {
            var car = (sender as FrameworkElement).DataContext as Cars;
            new CarsAddAndChange(this, car).Show();
        }

        private void LeftCarsButton_Click(object sender, RoutedEventArgs e) => UpdateCarsCount(true);

        private void RightCarsButton_Click(object sender, RoutedEventArgs e) => UpdateCarsCount(false);

        private void CarStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            carStatuses = CarStatusComboBox.SelectedItem as CarStatuses;
            UpdateSearchingCarsCount();
        }

        private void EnableCarStatusToogleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateSearchingCarsCount();
        }
    }
}
