using Hetfield.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hetfield.Tools.ModelsForGeneration
{
    internal class PaidContractsModel
    {

        public PaidContractsModel(Orders order)
        {
            DateOfDrawingUp = DateTime.Now;
            Buyer = order.IdBuyerNavigation.Surname + " " + order.IdBuyerNavigation.Name + " " + order.IdBuyerNavigation.Patronymic;
            Owner = order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Surname + " " +
                order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Name + " " +
                order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Patronymic;
            MarkAndModel = "Mercedez-Benz " + order.IdCarNavigation.IdCarPassportNavigation.CarModel;
            VIN = order.IdCarNavigation.IdCarPassportNavigation.VinNumber;
            CarType = order.IdCarNavigation.IdCarPassportNavigation.IdCarTypeNavigation.TypeName;
            ManufactureYear = order.IdCarNavigation.IdCarPassportNavigation.CarManufactureYear;
            CarColor = order.IdCarNavigation.IdCarPassportNavigation.IdCarColorNavigation.ColorName;
            CarPower = order.IdCarNavigation.IdCarPassportNavigation.CarPower;
            EngineDisplacement = order.IdCarNavigation.IdCarPassportNavigation.EngineDisplacement;
            PassportSeriesAndNumber = order.IdCarNavigation.IdCarPassportNavigation.PassportSeriasAndNumber;
            Transmission = order.IdCarNavigation.IdTranssmissionNavigation.TranssmissionName;
            Configuration = order.IdCarNavigation.IdCarConfigurationNavigation.CarConfigurationName;
            Mileage = order.IdCarNavigation.Mileage;
            TankCapacity = order.IdCarNavigation.TankCapacity;
            TotalPrice = order.FinalPrice;
            BuyerInitials = order.IdBuyerNavigation.Surname + " " + order.IdBuyerNavigation.Name.ToUpper().First() 
                            + ". " + order.IdBuyerNavigation.Patronymic.ToUpper().First() + ".";
            OwnerInitials = order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Surname + " " +
                order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Name.ToUpper().First() + ". " +
                order.IdCarNavigation.IdCarPassportNavigation.IdOwnerNavigation.Patronymic.ToUpper().First() + ".";

        }

        public DateTime DateOfDrawingUp { get; set; }
        public string Buyer { get; set; }
        public string Owner { get; set; }
        public string MarkAndModel { get; set; }
        public string VIN { get; set; }
        public string CarType { get; set; }
        public int ManufactureYear { get; set; }
        public string CarColor { get; set; }
        public int CarPower { get; set; }
        public int EngineDisplacement { get; set; }
        public string PassportSeriesAndNumber { get; set; }
        public string Transmission { get; set; }
        public string Configuration { get; set; }
        public int Mileage { get; set; }
        public int TankCapacity { get; set; }
        public decimal TotalPrice { get; set; }
        public string OwnerInitials { get; set; }
        public string BuyerInitials { get; set; }

        public static string GetStringOfEmpty(string Source) => Source != null ? Source : "";
        private static string GetFirstChar(string Source) => Source == "" ? "" : $"{Source.First()}.";


    }
}
