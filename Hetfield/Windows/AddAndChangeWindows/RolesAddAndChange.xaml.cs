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
    public partial class RolesAddAndChange : Window
    {
        bool _changeMode;

        RolesPage _page;

        int id;
        public RolesAddAndChange(RolesPage page, Roles role = null)
        {
            InitializeComponent();
            _page = page;
            if(role != null)
            {
                id = role.IdRole;
                RolesNameTextBox.Text = role.RoleName;
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
            if(RolesNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Поля не заполнены").ShowDialog();
                return false;
            }
            if(DbUtils.db.Roles.ToList().Any(ce => 
                Helper.DbCompare(ce.RoleName, RolesNameTextBox.Text) && ce.IdRole != id))
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
                Roles role;
                if (_changeMode)
                    role = DbUtils.db.Roles.FirstOrDefault(c => c.IdRole == id);
                else
                    role = new Roles();

                role.RoleName = RolesNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(role);

                DbUtils.SaveChanges();
                
                _page.RolesDataGrid.ItemsSource = DbUtils.GetTableAllValues<Roles>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
