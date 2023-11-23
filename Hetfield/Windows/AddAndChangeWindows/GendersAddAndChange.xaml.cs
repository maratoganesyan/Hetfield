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
    public partial class GendersAddAndChange : Window
    {
        bool _changeMode;

        GendersPage _page;

        int id;
        public GendersAddAndChange(GendersPage page, Genders gender = null)
        {
            InitializeComponent();
            _page = page;
            if(gender != null)
            {
                id = gender.IdGender;
                GenderNameTextBox.Text = gender.GenderName;
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
            if(GenderNameTextBox.Text.Length == 0)
            {
                new MessageBoxWindow("Поля не заполнены").ShowDialog();
                return false;
            }
            if(DbUtils.db.Genders.ToList().Any(ce => 
                Helper.DbCompare(ce.GenderName, GenderNameTextBox.Text) && ce.IdGender != id))
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
                Genders gender;
                if (_changeMode)
                    gender = DbUtils.db.Genders.FirstOrDefault(c => c.IdGender == id);
                else
                    gender = new Genders();

                gender.GenderName = GenderNameTextBox.Text;

                if (!_changeMode)
                    DbUtils.AddData(gender);

                DbUtils.SaveChanges();
                
                _page.GendersDataGrid.ItemsSource = DbUtils.GetTableAllValues<Genders>();
                Close();

            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка сохранения").ShowDialog();
            }
        }
    }
}
