using System.Windows.Controls;
using Sem3_kurs.Enums;
namespace Sem3_kurs.Model
{    public class Property
    {
        public int Id { get; set; }
        public string Type { get; set; }      
        public string District { get; set; }
        public string Address { get; set; }

        public int Floor { get; set; }
        public int Rooms { get; set; }
        public double TotalArea { get; set; }
        public double LivingArea { get; set; }
        public double KitchenArea { get; set; }

        public bool HasLoggia { get; set; }
        public bool HasBalcony { get; set; }

        public string HouseDescription { get; set; } 

        public decimal Price { get; set; }
        public DealType DealType { get; set; }

        public Owner Owner { get; set; }
        public bool IsAvailable { get; set; } = true;

        public string GetShortDescription()
        {
            return $"{Type}, {Rooms}к, {TotalArea} м², {District}, {Price} руб.";
        }

        public bool MatchesOrder(Order order)
        {
            if (order == null) return false;

            if (!string.IsNullOrEmpty(order.District) &&
                !string.Equals(order.District, District, StringComparison.OrdinalIgnoreCase))
                return false;

            if (order.Rooms.HasValue && Rooms < order.Rooms.Value)
                return false;

            if (order.TotalAreaMin.HasValue && TotalArea < order.TotalAreaMin.Value)
                return false;

            if (order.PriceMin.HasValue && Price < order.PriceMin.Value)
                return false;

            if (order.PriceMax.HasValue && Price > order.PriceMax.Value)
                return false;

            if (order.RequestDealType.HasValue && order.RequestDealType.Value != DealType)
                return false;

            return true;
        }

        public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
    }

}
