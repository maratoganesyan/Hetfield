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
using System.Security;
using Wpf.Ui.Animations;
using Hetfield.Tools;
using Wpf.Ui.Controls;
using System.IO;

namespace Hetfield.Windows
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        string rightAnswerForCheck;

        int attemptCount = 0;
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Minim_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 2000);
            Transitions.ApplyTransition(MainPanel, TransitionType.FadeInWithSlide, 2000);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void GenerateCheckingDialog()
        {
            Random rand = new Random();
            Number1.Text = rand.Next(0, 100).ToString();
            Number2.Text = rand.Next(0, 100).ToString();
            rightAnswerForCheck = (int.Parse(Number1.Text) + int.Parse(Number2.Text)).ToString();
            AnswerBox.Text = "";
        }

        private async void SaveUserInSystem()
        {
            File.WriteAllText(Helper.SaveUserInSystemPath, LoginTextBox.Text + '\n' + PasswordBox.Password);
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (DbUtils.db.Users.Any(p => p.Login == LoginTextBox.Text && 
                                       p.Password == PasswordBox.Password && 
                                       (p.IdRoleNavigation.RoleName == DbUtils.Roles.Admin ||
                                        p.IdRoleNavigation.RoleName == DbUtils.Roles.SalesManager ||
                                        p.IdRoleNavigation.RoleName == DbUtils.Roles.Director)))
            {
                if ((bool)SaveMeSwitch.IsChecked)
                    SaveUserInSystem();
                var employee = DbUtils.db.Users.Single(p => p.Login == LoginTextBox.Text && p.Password == PasswordBox.Password);
                new EmployeeWindow(employee).Show();
                this.Close();
            }
            else if(DbUtils.db.Users.Any(p => p.Login == LoginTextBox.Text &&
                                       p.Password == PasswordBox.Password))
            {
                new MessageBoxWindow("Нет прав в системе").ShowDialog();
            }
            else
            {
                attemptCount++;
                if (attemptCount == 3)
                {
                    GenerateCheckingDialog();
                    Checking.ShowAsync();
                }
                else
                {
                    var win = new MessageBoxWindow("Логин или пароль введены неверно. Повторите попытку ещё раз");
                    win.ShowDialog();
                }
            }
        }

        private void GetAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if(AnswerBox.Text == rightAnswerForCheck)
            {
                Checking.Hide();
                attemptCount = 0;
            }
            else
            {
                GenerateCheckingDialog();
            }
        }
    }
}
