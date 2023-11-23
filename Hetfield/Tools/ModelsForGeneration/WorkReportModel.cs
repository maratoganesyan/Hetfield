using Hetfield.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hetfield.Tools.ModelsForGeneration
{
    internal class WorkReportModel
    {

        public DateTime DateOfDrawingUp { get; set; }
        public List<OrderModel> Orders { get; set; }
        public decimal TotalCost { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public WorkReportModel(List<Orders> orders, DateOnly StartDate, DateOnly EndDate)
        {
            Orders = orders.Select(o => new OrderModel(o)).ToList();
            DateOfDrawingUp = DateTime.Now;
            CurrentDate = DateTime.Now;
            TotalCost = Orders.Sum(o => o.FinalPrice);
            this.StartDate = StartDate;
            this.EndDate = EndDate;
        }
    }
}
