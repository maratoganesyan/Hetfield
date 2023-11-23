using Hetfield.Entities;
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
using Hetfield.Tools;
using Wpf.Ui.Animations;
using System.IO;
using Hetfield.Windows.Pages;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Hetfield.Windows
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        private async void LoadDb()
        {
            await Task.Run(() => DbUtils.db.CarPhotos.Load());
            await Task.Run(() => DbUtils.db.Cars.LoadAsync());
            await Task.Run(() => DbUtils.db.CarsPassport.LoadAsync());
            await Task.Run(() => DbUtils.db.Users.LoadAsync());
            string LogAndPass = File.ReadAllText(Helper.SaveUserInSystemPath);
            if (LogAndPass.Length == 0)
            {
                new AuthWindow().Show();
                this.Close();
            }
            else
            {
                string[] LogAndPassArr = LogAndPass.Split('\n');
                var user = DbUtils.db.Users.FirstOrDefault(u => u.Login == LogAndPassArr[0] && u.Password == LogAndPassArr[1]);
                if (user != null)
                {
                    new EmployeeWindow(user).Show();
                    this.Close();
                }
                else
                {
                    new AuthWindow().Show();
                    this.Close();
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 1000);
            Transitions.ApplyTransition(MainPanel, TransitionType.FadeInWithSlide, 2000);
            LoadDb();
        }
    }
}
