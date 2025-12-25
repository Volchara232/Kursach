using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sem3_kurs.Model;
using Sem3_kurs.Enums;
namespace Sem3_kurs.Collections
{
    public class EstateAgency
    {
        public ClientsList ClientsList { get; } = new ClientsList();
        public OwnersList OwnersList { get; } = new OwnersList();
        public List<Property> PropertiesList { get; } = new List<Property>();
        public List<Order> OrdersList { get; } = new List<Order>();
        public List<Deal> DealsList { get; } = new List<Deal>();
        public List<Receipt> ReceiptsList { get; } = new List<Receipt>();

        public StatisticsManager GetStatisticsManager()
            => new StatisticsManager(OrdersList, PropertiesList, DealsList);

        public IEnumerable<Property> FindPropertiesForOrder(Order order)
        {
            return PropertiesList
                .Where(p => p.IsAvailable)
                .Where(p => p.MatchesOrder(order))
                .ToList();
        }

        public Deal RegisterDeal(Order order, Property property, decimal amount)
        {
            var deal = new Deal
            {
                Order = order,
                Property = property,
                DealType = property.DealType,
                Amount = amount,
                IsCompleted = true
            };
            deal.CalculateAgencyFee();

            DealsList.Add(deal);
            property.SetAvailability(false);
            order.UpdateStatus(OrderStatus.Completed);

            return deal;
        }
    }

}
