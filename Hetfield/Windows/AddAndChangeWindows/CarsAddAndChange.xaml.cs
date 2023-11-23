using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CarsAddAndChange.xaml
    /// </summary>
    public partial class CarsAddAndChange : Window
    {
        bool _changeMode;

        int id;

        List<CarPhotos> carPhotos;

        CarsPassport carPassport;

        CarsPage _page;

        public CarsAddAndChange(CarsPage page, Cars? car = null)
        {
            InitializeComponent();
            InitComboBoxes();
            _page = page;
            _changeMode = false;
            id = DbUtils.db.CarPhotos.Max(p => p.IdCar) + 1;
            carPassport = new CarsPassport();
            carPhotos = new List<CarPhotos>();
            if (car != null)
                InitChangedCar(car);
        }

        private void InitChangedCar(Cars car)
        {
            //carPassport
            carPassport = car.IdCarPassportNavigation;
            ModelTextBox.Text = carPassport.CarModel;
            VinTextBox.Text = carPassport.VinNumber;
            EngineDisplacmentTextBox.Value = carPassport.EngineDisplacement;
            CarPowerTextBox.Value = carPassport.CarPower;
            CarPassportSeriasAndNumberTextBox.Text = carPassport.PassportSeriasAndNumber;
            PTSDateOfIssue.SelectedDate = carPassport.DateOfIssue;
            YearsComboBox.SelectedItem = carPassport.CarManufactureYearNavigation;
            ColorsComboBox.SelectedItem = carPassport.IdCarColorNavigation;
            CarTypeComboBox.SelectedItem = carPassport.IdCarTypeNavigation;
            OwnerComboBox.SelectedItem = carPassport.IdOwnerNavigation;

            //car
            PriceTextBox.Text = car.Price.ToString();
            MileageTextBox.Value = car.Mileage;
            CarNumberTextBox.Text = car.CarNumber;
            TankCapacityTextBox.Value = car.TankCapacity;
            CarConfigurationComboBox.SelectedItem = car.IdCarConfigurationNavigation;
            CarEngineComboBox.SelectedItem = car.IdEngineNavigation;
            CarStatusComboBox.SelectedItem = car.IdCarStatusNavigation;
            CarTranssmissionComboBox.SelectedItem = car.IdTranssmissionNavigation;

            DescriptionTextBox.AppendText(car.Description);
            //carPhotos
            carPhotos = car.CarPhotos.ToList();
            CarPhotosView.ItemsSource = carPhotos;

            _changeMode = true;
            id = car.IdCar;
        }

        private void InitComboBoxes()
        {
            YearsComboBox.ItemsSource = DbUtils.GetTableAllValues<ManufactureYears>();
            ColorsComboBox.ItemsSource = DbUtils.GetTableAllValues<CarColors>();
            CarTypeComboBox.ItemsSource = DbUtils.GetTableAllValues<CarTypes>();
            OwnerComboBox.ItemsSource = DbUtils.GetSearchingValues<Users>(DbUtils.Roles.Client);
            CarConfigurationComboBox.ItemsSource = DbUtils.GetTableAllValues<CarConfigurations>();
            CarEngineComboBox.ItemsSource = DbUtils.GetTableAllValues<CarEngines>();
            CarStatusComboBox.ItemsSource = DbUtils.GetTableAllValues<CarStatuses>();
            CarTranssmissionComboBox.ItemsSource = DbUtils.GetTableAllValues<CarTranssmissions>();
            YearsComboBox.SelectedIndex = 0;
            ColorsComboBox.SelectedIndex = 0;
            CarTypeComboBox.SelectedIndex = 0;
            OwnerComboBox.SelectedIndex = 0;
            CarConfigurationComboBox.SelectedIndex = 0;
            CarEngineComboBox.SelectedIndex = 0;
            CarStatusComboBox.SelectedIndex = 0;
            CarTranssmissionComboBox.SelectedIndex = 0;
        }

        #region visual back
        private void Minim_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void LeftPageSwapButton_Click(object sender, RoutedEventArgs e)
        {
            if (PTSGrid.Visibility == Visibility.Visible)
                return;
            CarAboutGrid.Visibility = Visibility.Collapsed;
            PTSGrid.Visibility = Visibility.Visible;
        }

        private void RightPageSwapButton_Click(object sender, RoutedEventArgs e)
        {

            if (CarAboutGrid.Visibility == Visibility.Visible)
                return;
            PTSGrid.Visibility = Visibility.Collapsed;
            CarAboutGrid.Visibility = Visibility.Visible;
        }

        #endregion

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";
                CarPhotos NewPhoto = new CarPhotos();
                NewPhoto.IdCar = id;
                NewPhoto.IdPhoto = carPhotos.Count() + 1;
                NewPhoto.Photo = File.ReadAllBytes(openFileDialog.FileName);
                carPhotos.Add(NewPhoto);
                CarPhotosView.ItemsSource = null;
                CarPhotosView.ItemsSource = carPhotos;
            }
        }

        private void ChangePhoto_CLick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";
                CarPhotos photo = button.DataContext as CarPhotos;
                photo.Photo = File.ReadAllBytes(openFileDialog.FileName);
                carPhotos[photo.IdPhoto - 1] = photo;
                CarPhotosView.ItemsSource = null;
                CarPhotosView.ItemsSource = carPhotos;
            }
        }

        private void SavePhotos()
        {
            if (_changeMode)
            {
                foreach (var item in carPhotos)
                {
                    CarPhotos? carPhoto = DbUtils.db.CarPhotos.FirstOrDefault(p => p.IdCar == item.IdCar && p.IdPhoto == item.IdPhoto);
                    if(carPhoto == null)
                    {
                        DbUtils.AddData(item);
                    }
                    else
                    {
                        carPhoto.Photo = item.Photo;
                    }
                }

                DbUtils.SaveChanges();
            }
            else
            {
                foreach (var item in carPhotos)
                {
                    DbUtils.AddData(item);
                }
                DbUtils.SaveChanges();
            }
        }

        private void SavePTS()
        {
            if (_changeMode)
                carPassport = DbUtils.db.CarsPassport.First(p => p.IdCarPassport == id);
            else
                carPassport = new CarsPassport();

            carPassport.CarPower = Convert.ToInt32(CarPowerTextBox.Value);
            carPassport.CarModel = ModelTextBox.Text;
            carPassport.EngineDisplacement = Convert.ToInt32(EngineDisplacmentTextBox.Value);
            carPassport.VinNumber = VinTextBox.Text;
            carPassport.PassportSeriasAndNumber = CarPassportSeriasAndNumberTextBox.Text;
            carPassport.DateOfIssue = PTSDateOfIssue.SelectedDate.Value;
            carPassport.CarManufactureYear = (YearsComboBox.SelectedItem as ManufactureYears).YearValue;
            carPassport.IdCarColor = (ColorsComboBox.SelectedItem as CarColors).IdCarColors;
            carPassport.IdCarType = (CarTypeComboBox.SelectedItem as CarTypes).IdCarType;
            carPassport.IdOwner = (OwnerComboBox.SelectedItem as Users).IdUser;

            if(!_changeMode)
                DbUtils.AddData(carPassport);

            DbUtils.SaveChanges();
            
        }

        private bool Validation()
        {
            if(carPhotos.Count() == 0)
            {
                new MessageBoxWindow("Нет добавленных фотографий").ShowDialog();
                return false;
            }
            if (CarPowerTextBox.Value == 0)
            {
                new MessageBoxWindow("Мощность двигателя не введена").ShowDialog();
                return false;
            }
            if (ModelTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Модель не введена").ShowDialog();
                return false;
            }
            if (EngineDisplacmentTextBox.Value == 0)
            {
                new MessageBoxWindow("Объем двигателя не введен").ShowDialog();
                return false;
            }
            if (VinTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Vin не введен").ShowDialog();
                return false;
            }
            if (CarPassportSeriasAndNumberTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Серия и номер паспорта ТС не введены").ShowDialog();
                return false;
            }
            if(!decimal.TryParse(PriceTextBox.Text, out decimal check))
            {
                new MessageBoxWindow("Цена введена некорректно").ShowDialog();
                return false;
            }
            if (TankCapacityTextBox.Value == 0)
            {
                new MessageBoxWindow("Объем бензобака не введен").ShowDialog();
                return false;
            }
            if (CarNumberTextBox.Text.Length != 9)
            {
                new MessageBoxWindow("Гос номер введен не корректно").ShowDialog();
                return false;
            }
            TextRange range = new TextRange(DescriptionTextBox.Document.ContentStart, DescriptionTextBox.Document.ContentEnd);
            if(range.Text.Replace("\r\n", string.Empty).Length == 0)
            {
                new MessageBoxWindow("Описание не введено").ShowDialog();
                return false;
            }
            return true;
        }

        private void SaveCarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validation())
                    return;
                SavePTS();

                Cars car;
                if (_changeMode)
                    car = DbUtils.db.Cars.First(p => p.IdCar == id);
                else
                    car = new Cars();

                if(!_changeMode)
                    car.IdCarPassport = DbUtils.db.CarsPassport.OrderBy(p => p.IdCarPassport).Last().IdCarPassport;
                car.Price = decimal.Parse(PriceTextBox.Text);
                car.CarNumber = CarNumberTextBox.Text;
                car.Description = (new TextRange(DescriptionTextBox.Document.ContentStart,
                                    DescriptionTextBox.Document.ContentEnd)).Text.Replace("\r\n", string.Empty);
                car.Mileage = Convert.ToInt32(MileageTextBox.Value);
                car.TankCapacity = Convert.ToInt32(TankCapacityTextBox.Value);
                car.IdCarStatus = (CarStatusComboBox.SelectedItem as CarStatuses).IdCarStatus;
                car.IdEngine = (CarEngineComboBox.SelectedItem as CarEngines).IdCarEngine;
                car.IdCarConfiguration = (CarConfigurationComboBox.SelectedItem as CarConfigurations).IdCarConfiguration;
                car.IdTranssmission = (CarTranssmissionComboBox.SelectedItem as CarTranssmissions).IdTranssmission;

                if (!_changeMode)
                    DbUtils.AddData(car);

                DbUtils.SaveChanges();
                SavePhotos();
                new MessageBoxWindow("Данные успешно сохранены").Show();
                _page.UpdateSearchingCarsCount();
                Close();
            }
            catch(Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
