using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sem3_kurs.Model;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Collections
{
    public class StatisticsManager
    {
        private readonly IEnumerable<Order> _orders;
        private readonly IEnumerable<Property> _properties;
        private readonly IEnumerable<Deal> _deals;

        public StatisticsManager(IEnumerable<Order> orders,
                                 IEnumerable<Property> properties,
                                 IEnumerable<Deal> deals)
        {
            _orders = orders;
            _properties = properties;
            _deals = deals;
        }

        public string GetMostPopularPropertyTypeByDemand(DealType dealType)
        {
            return _orders
                .Where(o => o.RequestDealType == dealType)
                .GroupBy(o => o.RequestType)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key.ToString())
                .FirstOrDefault();
        }

        public string GetMostPopularPropertyTypeByOffers(DealType dealType)
        {
            return _properties
                .Where(p => p.DealType == dealType)
                .GroupBy(p => p.Type)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        public decimal GetTotalProfit(DateTime from, DateTime to)
        {
            return _deals
                .Where(d => d.Date >= from && d.Date <= to)
                .Sum(d => d.AgencyFee);
        }

        public decimal CompareProfit(DateTime from1, DateTime to1,
                                     DateTime from2, DateTime to2)
        {
            var p1 = GetTotalProfit(from1, to1);
            var p2 = GetTotalProfit(from2, to2);
            return p1 - p2; 
        }
    }

}
