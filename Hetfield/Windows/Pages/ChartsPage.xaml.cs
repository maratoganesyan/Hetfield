using Hetfield.Tools;
using ScottPlot;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hetfield.Windows.Pages
{
    public partial class ChartsPage : Page
    {
        public ChartsPage()
        {
            InitializeComponent();
            SetSalesStatisticsPlot();
            SetAnnouncementStatisticsPlot();
            SetPlotOrderByDate();
        }
        public void SetSalesStatisticsPlot() =>
            SetPie(DbUtils.GetSalesStatistics, SalesPlot, "Статистика продаж моделей автомобилей Mercedez-Benz.");
        public void SetAnnouncementStatisticsPlot() =>
            SetPie(DbUtils.GetAnnouncementStatistics, AnnouncementPlot, "Статистика выставленных моделей автомобилей Mercedez-Benz.");
        public void SetPie(Func<(double[] Sales, string[] ModelsNames)> func, WpfPlot plot, string Title)
        {
            var Stats = func.Invoke();

            for (int i = 0; i < Stats.Sales.Length; i++)
                Stats.ModelsNames[i] += $" ({Stats.Sales[i] / Stats.Sales.Sum() * 100:.0}%)";

            var pie = plot.Plot.AddPie(Stats.Sales);

            pie.SliceLabels = Stats.Sales.Select(x => x.ToString()).ToArray();
            pie.ShowLabels = true;
            pie.LegendLabels = Stats.ModelsNames;

            pie.Explode = true;

            plot.Plot.Legend();
            plot.Plot.Title(Title);

            pie.Size = .75;
            plot.Refresh();
        }
        public void SetPlotOrderByDate()
        {
            var data = DbUtils.GetOrdersByDates();
            var bar = OrderByDatePlot.Plot.AddBar(values: data.Values, positions: data.DateTimes);
            OrderByDatePlot.Plot.XAxis.DateTimeFormat(true);
            OrderByDatePlot.Refresh();
            bar.BarWidth = 1.0 / 2 * .8;
            OrderByDatePlot.Plot.SetAxisLimits(yMin: 0);
            OrderByDatePlot.Plot.Layout(right: 20);
            OrderByDatePlot.Plot.Title("Количество проданных автомобилей, относительно первой и последней даты продажи.");
            OrderByDatePlot.Plot.XAxis.Label("Дни (от первой, до последней даты продажи)");
            OrderByDatePlot.Plot.YAxis.Label("Количество проданных автомобилей");
            OrderByDatePlot.Refresh();
        }
    }
}
