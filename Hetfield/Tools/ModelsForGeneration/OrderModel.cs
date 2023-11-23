using Hetfield.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hetfield.Tools.ModelsForGeneration
{
    internal class OrderModel
    {
        public int idOrder { get; set; }
        public string Owner { get; set; }
        public string Buyer { get; set; }
        public string Staff { get; set; }
        public string CarMarkAndModel { get; set; }
        public decimal CarPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime DateOfOrder { get; set; }
        public OrderModel(Orders order)
        {
            idOrder = order.IdOrder;
            string OwnerPatronymic = GetFirstChar(GetStringOfEmpty(order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Patronymic));
            string BuyerPatronymic = GetFirstChar(GetStringOfEmpty(order.IdBuyerNavigation.Patronymic));
            string StaffPatronymic = GetFirstChar(GetStringOfEmpty(order.IdStaffNavigation.Patronymic));
            Owner = $"{order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Surname} " +
                $"{order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Name.First()}. " +
                $"{OwnerPatronymic}";
            Buyer = $"{order.IdBuyerNavigation.Surname} {order.IdBuyerNavigation.Name.First()}. {BuyerPatronymic}";
            Staff = $"{order.IdStaffNavigation.Surname} {order.IdStaffNavigation.Name.First()}. {StaffPatronymic}";
            CarMarkAndModel = $"Mercedez-Benz {order.IdCarNavigation.IdCarPassportNavigation.CarModel}";
            CarPrice = order.IdCarNavigation.Price;
            FinalPrice = order.FinalPrice;
            DateOfOrder = order.DateOfOrder;
        }
        public static string GetStringOfEmpty(string Source) => Source != null ? Source : "";
        private static string GetFirstChar(string Source) => Source == "" ? "" : $"{Source.First()}.";
    }
}
