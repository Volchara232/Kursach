using System;
using System.Collections.Generic;
using System.Linq;
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

        // НОВЫЕ МЕТОДЫ ДЛЯ ПОЛНОЙ СТАТИСТИКИ

        public int GetTotalDealsCount()
        {
            return _deals.Count();
        }

        public int GetTotalClientsCount()
        {
            return _orders.Select(o => o.Client).Distinct().Count();
        }

        public int GetTotalPropertiesCount()
        {
            return _properties.Count();
        }

        public int GetAvailablePropertiesCount()
        {
            return _properties.Count(p => p.IsAvailable);
        }

        public decimal GetTotalCommission()
        {
            return _deals.Sum(d => d.AgencyFee);
        }

        public Dictionary<string, int> GetDealsByMonth(int year)
        {
            return _deals
                .Where(d => d.Date.Year == year)
                .GroupBy(d => d.Date.Month)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => new DateTime(year, g.Key, 1).ToString("MMMM"),
                    g => g.Count()
                );
        }

        public Dictionary<string, decimal> GetCommissionByMonth(int year)
        {
            return _deals
                .Where(d => d.Date.Year == year)
                .GroupBy(d => d.Date.Month)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => new DateTime(year, g.Key, 1).ToString("MMMM"),
                    g => g.Sum(d => d.AgencyFee)
                );
        }

        public int GetCompletedDealsCount()
        {
            return _deals.Count(d => d.IsCompleted);
        }

        public int GetActiveOrdersCount()
        {
            return _orders.Count(o => o.Status == OrderStatus.New ||
                                     o.Status == OrderStatus.InProgress ||
                                     o.Status == OrderStatus.OffersPrepared);
        }

        public string GetBestClient()
        {
            var clientDeals = _deals
                .Where(d => d.Order?.Client != null)
                .GroupBy(d => d.Order.Client.FullName)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            return clientDeals != null ?
                $"{clientDeals.Key} ({clientDeals.Count()} сделок)" :
                "Нет данных";
        }

        public string GetMostProfitableDealType()
        {
            var dealTypeProfit = _deals
                .GroupBy(d => d.DealType)
                .Select(g => new
                {
                    DealType = g.Key,
                    TotalProfit = g.Sum(d => d.AgencyFee)
                })
                .OrderByDescending(x => x.TotalProfit)
                .FirstOrDefault();

            return dealTypeProfit != null ?
                $"{dealTypeProfit.DealType} ({dealTypeProfit.TotalProfit:C})" :
                "Нет данных";
        }

        public Dictionary<string, int> GetPropertyTypeDistribution()
        {
            return _properties
                .GroupBy(p => p.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }
    }
}