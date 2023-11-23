using Castle.Core.Resource;
using Hetfield.Entities;
using Hetfield.Tools;
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
using Hetfield.Windows.Pages;
using Hetfield.Windows.AddAndChangeWindows;
using System.IO;

namespace Hetfield.Windows
{
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow(Users user)
        {
            InitializeComponent();
            SetAccess(user);
        }

        public void SetAccess(Users user)
        {
            EmployeeDataPanel.DataContext = null;
            EmployeeDataPanel.DataContext = user;
            Spr.Visibility = Visibility.Visible;
            UsersMI.Visibility = Visibility.Visible;
            CarsMI.Visibility = Visibility.Visible;
            ChartMI.Visibility = Visibility.Visible;
            ClientsMI.Visibility = Visibility.Visible;
            DocumentsMI.Visibility = Visibility.Visible;
            if (user.IdRoleNavigation.RoleName == DbUtils.Roles.Admin)
            {
                ClientsMI.Visibility = Visibility.Collapsed;
                MI_Click(UsersMI, null);
                return;
            }
            if (user.IdRoleNavigation.RoleName == DbUtils.Roles.SalesManager)
            {
                UsersMI.Visibility = Visibility.Collapsed;
                ChartMI.Visibility = Visibility.Collapsed;
                Spr.Visibility = Visibility.Collapsed;
                DocumentsMI.Visibility = Visibility.Collapsed;
                MI_Click(ClientsMI, null);
                return;
            }
            if (user.IdRoleNavigation.RoleName == DbUtils.Roles.Director)
            {
                Spr.Visibility = Visibility.Collapsed;
                UsersMI.Visibility = Visibility.Collapsed;
                CarsMI.Visibility = Visibility.Collapsed;
                MI_Click(ClientsMI, null);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minim_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Helper.SaveUserInSystemPath, string.Empty);
            new AuthWindow().Show();
            this.Close();
        }

        private void MI_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as FrameworkElement;
            if (menuItem.Name == CarsMI.Name)
            {
                MainContent.Content = new CarsPage();
                return;
            }
            if (menuItem.Name == ChartMI.Name)
            {
                MainContent.Content = new ChartsPage();
                return;
            }
            if(menuItem.Name == OrdersMI.Name)
            {
                MainContent.Content = new OrdersPage(this);
                return;
            }
            Uri uri = new Uri(menuItem.Tag.ToString(), UriKind.RelativeOrAbsolute);
            MainContent.Content = null;
            MainContent.Content = Application.LoadComponent(uri);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var employee = EmployeeDataPanel.DataContext as Users;
            UsersAddAndChange window = new UsersAddAndChange(employee, true, this);
            window.ShowDialog();
        }

        private void DocumentsMI_Click(object sender, RoutedEventArgs e)
        {
            new ReportsGenerateWindow().Show();
        }
    }
}
