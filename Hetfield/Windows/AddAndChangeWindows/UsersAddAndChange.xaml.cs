using Hetfield.Entities;
using Hetfield.Tools;
using Hetfield.Windows.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    // Нужно доделать валидацию номера телефона и почты
    public partial class UsersAddAndChange : Window
    {
        string path;

        UsersTable usersPage;

        ClientsTable clientsTable;

        EmployeeWindow employeeWindow;

        bool changeMode;

        int id;

        private void Init(Users? user, bool ChangeMode)
        {
            InitializeComponent();
            RolesComboBox.ItemsSource = DbUtils.GetTableAllValues<Roles>();
            GenderComboBox.ItemsSource = DbUtils.GetTableAllValues<Genders>();
            changeMode = false;
            if (ChangeMode)
            {
                changeMode = true;
                SurnameTextBox.Text = user.Surname;
                NameTextBox.Text = user.Name;
                PatronymicTextBox.Text = user.Patronymic;
                DateOfBirthPicker.SelectedDate = user.DateOfBirth;
                EmailTextBox.Text = user.Email;
                PhoneNumberTextBox.Text = user.PhoneNumber;
                LoginTextBox.Text = user.Login;
                PasswordTextBox.Text = user.Password;
                RepeatPasswordTextBox.Text = user.Password;
                RolesComboBox.SelectedItem = user.IdRoleNavigation;
                GenderComboBox.SelectedItem = user.IdGenderNavigation;
                UserPhoto.ImageSource = new ImageSourceConverter().ConvertFrom(user.Photo) as ImageSource;
                id = user.IdUser;
            }
            else
            {
                RolesComboBox.SelectedItem = DbUtils.db.Roles.First(r => r.RoleName == DbUtils.Roles.Client);
                GenderComboBox.SelectedIndex = 1;
            }
        }
        public UsersAddAndChange(Users? user, bool ChangeMode, UsersTable table, bool EnabledRoleChange = false)
        {
            Init(user, ChangeMode);
            usersPage = table;
            RolesComboBox.IsEnabled = EnabledRoleChange;
        }

        public UsersAddAndChange(Users? user, bool ChangeMode, ClientsTable table, bool EnabledRoleChange = false)
        {
            Init(user, ChangeMode);
            clientsTable = table;
            RolesComboBox.IsEnabled = EnabledRoleChange;
        }

        public UsersAddAndChange(Users? user, bool ChangeMode, EmployeeWindow window)
        {
            Init(user, ChangeMode);
            RolesComboBox.IsEnabled = false;
            employeeWindow = window;
        }
        private void Minim_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataValidation())
            {
                Users user;
                try
                {
                    if (changeMode)
                        user = DbUtils.db.Users.FirstOrDefault(p => p.IdUser == id);
                    else
                        user = new Users();
                    user.Surname = SurnameTextBox.Text;
                    user.Name = NameTextBox.Text;
                    user.Patronymic = PatronymicTextBox.Text;
                    user.DateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Now;
                    user.Email = EmailTextBox.Text;
                    user.PhoneNumber = PhoneNumberTextBox.Text;
                    user.Login = LoginTextBox.Text;
                    user.Password = PasswordTextBox.Text;
                    user.IdRole = (RolesComboBox.SelectedItem as Roles).IdRole;
                    user.IdGender = (GenderComboBox.SelectedItem as Genders).IdGender;
                    if(path == null && !changeMode)
                        user.Photo = File.ReadAllBytes("/Images/DefaultUserPhoto.jpg");
                    if (path != null)
                        user.Photo = File.ReadAllBytes(path);

                    if (!changeMode)
                        DbUtils.db.Users.Add(user);

                    DbUtils.db.SaveChanges();

                    if (employeeWindow != null)
                        employeeWindow.SetAccess(user);
                    if (usersPage != null)
                        usersPage.UsersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Users>();
                    if (clientsTable != null)
                        usersPage.UsersDataGrid.ItemsSource = DbUtils.db.Users.ToList().Where(u => u.IdRoleNavigation.RoleName == DbUtils.Roles.Client).ToList();

                    new MessageBoxWindow("Данные успешно сохранены").ShowDialog(); 
                }
                catch
                {
                    ErrorMessage.Text = "Ошибка сохранения данных";
                    Error.ShowAsync();
                }

            }
        }

        private void ImageBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageBorder.Opacity = 0.7;
        }

        private void ImageBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageBorder.Opacity = 1;
        }

        private void ImageBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                UserPhoto.ImageSource = new ImageSourceConverter().ConvertFrom(File.ReadAllBytes(path)) as ImageSource;
            }
        }

        private bool DataValidation()
        {
            if(SurnameTextBox.Text.Length == 0)
            {
                ErrorMessage.Text = "Введите фамилию";
                Error.ShowAsync();
                return false;
            }
            if (NameTextBox.Text.Length == 0)
            {
                ErrorMessage.Text = "Введите Имя";
                Error.ShowAsync();
                return false;
            }
            if (DateOfBirthPicker.SelectedDate == null)
            {
                ErrorMessage.Text = "Выберите дату рождения";
                Error.ShowAsync();
                return false;
            }
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegex.IsMatch(EmailTextBox.Text))
            {
                ErrorMessage.Text = "Введите корректно адрес электронной почты";
                Error.ShowAsync();
                return false;
            }
            if(!PhoneNumberTextBox.Text.Contains('_'))
            {
                ErrorMessage.Text = "Номер телефона введен не в корректном формате";
                Error.ShowAsync();
                return false;
            }
            if(LoginTextBox.Text.Length < 8)
            {
                ErrorMessage.Text = "Логин должен содержать минимум 8 символов";
                Error.ShowAsync();
                return false;
            }
            if (DbUtils.db.Users.Any(p => p.Login == LoginTextBox.Text) && !changeMode)
            {
                ErrorMessage.Text = "Такой логин уже существует";
                Error.ShowAsync();
                return false;
            }
            if(PasswordTextBox.Text.Length < 8)
            {
                ErrorMessage.Text = "Пароль должен содержать минимум 8 символов";
                Error.ShowAsync();
                return false;
            }
            if (PasswordTextBox.Text != RepeatPasswordTextBox.Text)
            {
                ErrorMessage.Text = "Пароли должны совпадать";
                Error.ShowAsync();
                return false;
            }
            if(RolesComboBox.SelectedItem == null)
            {
                ErrorMessage.Text = "Роль не выбрана";
                Error.ShowAsync();
                return false;
            }
            if(GenderComboBox.SelectedItem == null)
            {
                ErrorMessage.Text = "Пол не выбран";
                Error.ShowAsync();
                return false;
            }
            return true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Error.Hide();
        }
    }
}
