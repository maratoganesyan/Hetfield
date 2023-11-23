using Hetfield.Entities;
using Hetfield.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hetfield.Tools
{
    internal static class DbUtils
    {

        public static Db db;

        static DbUtils()
        {
            try
            {
                db = new Db();
            }
            catch (Exception ex)
            {
                new MessageBoxWindow("Ошибка подключения к базе").ShowDialog();
                Application.Current.Shutdown();
            }
        }
        public static List<T> GetTableAllValues<T>() where T : class
        {
            return db.Set<T>().ToList();
        }

        public static List<T> GetSearchingValues<T>(string searchText) where T : class
        {
            return db.Set<T>().ToList().Where(p => p.ToString().ToLower().Contains(searchText.ToLower())).ToList();
        }

        public static void AddData<T>(T values) where T: class => db.Set<T>().Add(values);

        public static void SaveChanges() => db.SaveChanges();

        public static class Roles
        {
            public const string Admin = "Администратор"; 

            public const string Director = "Директор"; 

            public const string Client = "Клиент"; 

            public const string SalesManager = "Менеджер по объявлениям"; 
        }

        public static class CarStatuses
        {
            public const string InProcessing = "В обработке";

            public const string Saled = "Продана";

            public const string Exposed = "Выставлена";
        }

        public static class OrderStatuses
        {
            public const string InProcessing = "В обработке";

            public const string Finished = "Успешно завершена";

            public const string Deleted = "Удалена";
        }

        public static (double[] Values, double[] DateTimes) GetOrdersByDates()
        {
            Dictionary<double, double> statistics = new();
            DateTime First = db.Orders.Select(x => x.DateOfOrder).ToList().Min();
            int CountOfDays = (int)(db.Orders.Select(x => x.DateOfOrder).ToList().Max() - First).TotalDays + 1;
            List<Orders> Orders = db.Orders.ToList();
            for (int i = 0; i < CountOfDays; i++)
            {
                DateTime temp = First.AddDays(i);
                statistics.Add(temp.ToOADate(),
                    Orders
                    .Where(o => DateOnly.FromDateTime(o.DateOfOrder) == DateOnly.FromDateTime(temp))
                    .Count());
            }
            return (statistics.Values.ToArray(), statistics.Keys.ToArray());
        }

        public static (double[] Sales, string[] ModelsNames) GetSalesStatistics()
        {
            Dictionary<string, double> statistics = new();
            foreach (CarsPassport CP in db.CarsPassport.ToList())
            {
                if (statistics.ContainsKey(CP.CarModel.Substring(0, CP.CarModel.IndexOf(' '))))
                    continue;
                int CountOfSales = db.Orders.ToList()
                    .Where(o => o.IdCarNavigation.IdCarPassportNavigation.CarModel.Substring(0, o.IdCarNavigation.IdCarPassportNavigation.CarModel.IndexOf(' ')) 
                    == CP.CarModel.Substring(0, CP.CarModel.IndexOf(' '))).
                    Where(o => o.IdOrderStatusNavigation.OrderStatusName == DbUtils.OrderStatuses.Finished).Count();
                if (CountOfSales != 0)
                    statistics.Add(CP.CarModel.Substring(0, CP.CarModel.IndexOf(' ')), CountOfSales);
            }
            return (statistics.Values.ToArray(), statistics.Keys.ToArray());
        }

        public static (double[] Sales, string[] ModelsNames) GetAnnouncementStatistics()
        {
            Dictionary<string, double> statistics = new();
            foreach (CarsPassport CP in db.CarsPassport.ToList())
            {
                if (statistics.ContainsKey(CP.CarModel.Substring(0, CP.CarModel.IndexOf(' '))))
                    continue;
                int CountOfSupplies = db.Cars.ToList()
                    .Where(g => g.IdCarPassportNavigation.CarModel.Substring(0, g.IdCarPassportNavigation.CarModel.IndexOf(' ')) 
                    == CP.CarModel.Substring(0, CP.CarModel.IndexOf(' '))).ToList()
                    .Where(g => g.IdCarStatusNavigation.CarStatusName == DbUtils.CarStatuses.Exposed 
                            || g.IdCarStatusNavigation.CarStatusName == DbUtils.CarStatuses.InProcessing)
                    .Count();
                if (CountOfSupplies != 0)
                    statistics.Add(CP.CarModel.Substring(0, CP.CarModel.IndexOf(' ')), CountOfSupplies);
            }
            return (statistics.Values.ToArray(), statistics.Keys.ToArray());
        }

    }
}
