using System;
using System.Collections.Generic;
using System.Linq;
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
                Id = DealsList.Count > 0 ? DealsList.Max(d => d.Id) + 1 : 1,
                Order = order,
                Property = property,
                DealType = property.DealType,
                Amount = amount,
                IsCompleted = true,
                Date = DateTime.Now
            };
            deal.CalculateAgencyFee();

            DealsList.Add(deal);

        
            if (property.DealType == DealType.Sale)
            {
                TransferOwnership(property, order.Client);
                property.SetAvailability(false); 
            }
            else if (property.DealType == DealType.Rent)
            {
               
                property.SetAvailability(false); 
                                                 
            }

            order.UpdateStatus(OrderStatus.Completed);

            CreateCommissionReceipt(deal);

            return deal;
        }

        private void TransferOwnership(Property property, Client newOwner)
        {
            if (property == null || newOwner == null) return;

            var oldOwner = property.Owner;

   
            if (oldOwner != null)
            {
                oldOwner.Properties.Remove(property);
            }

            Owner newPropertyOwner;
            var existingOwner = OwnersList.Owners
                .FirstOrDefault(o => o.FullName == newOwner.FullName && o.Phone == newOwner.Phone);

            if (existingOwner != null)
            {
               newPropertyOwner = existingOwner;
            }
            else
            {
               
                newPropertyOwner = new Owner
                {
                    FullName = newOwner.FullName,
                    Address = newOwner.Address,
                    Phone = newOwner.Phone,
                    Email = newOwner.Email
                };
                OwnersList.AddOwner(newPropertyOwner);
            }

            property.Owner = newPropertyOwner;
            newPropertyOwner.Properties.Add(property);

            property.SetAvailability(true);
            Console.WriteLine($"Собственность передана: {oldOwner?.FullName ?? "Неизвестно"} -> {newPropertyOwner.FullName}");
        }
        public Owner CreateOwnerFromClient(Client client)
        {
            if (client == null) return null;

            var existingOwner = OwnersList.Owners
                .FirstOrDefault(o => o.FullName == client.FullName && o.Phone == client.Phone);

            if (existingOwner != null)
            {
                return existingOwner;
            }

            var newOwner = new Owner
            {
                FullName = client.FullName,
                Address = client.Address,
                Phone = client.Phone,
                Email = client.Email
            };

            OwnersList.AddOwner(newOwner);
            return newOwner;
        }   
        private void CreateCommissionReceipt(Deal deal)
        {
            var receipt = new Receipt
            {
                Number = GenerateReceiptNumber(),
                Client = deal.Order.Client,
                Amount = deal.AgencyFee,
                Date = DateTime.Now
            };

            ReceiptsList.Add(receipt);
        }

        private string GenerateReceiptNumber()
        {
            return $"R{DateTime.Now:yyyyMMddHHmmss}";
        }

        
        public List<PropertyMatch> FindMatchingPropertiesWithScore(Order order)
        {
            var matches = new List<PropertyMatch>();

            foreach (var property in PropertiesList.Where(p => p.IsAvailable))
            {
                int score = CalculateMatchScore(order, property);

                if (score >= 60) 
                {
                    matches.Add(new PropertyMatch
                    {
                        Property = property,
                        MatchScore = score,
                        MatchReason = GetMatchReason(order, property, score)
                    });
                }
            }

            return matches.OrderByDescending(m => m.MatchScore).ToList();
        }

        private int CalculateMatchScore(Order order, Property property)
        {
            int score = 0;
            int maxScore = 100;

    
            if (order.RequestDealType == property.DealType)
                score += 30;
            
            if (!string.IsNullOrEmpty(order.District) &&
                !string.IsNullOrEmpty(property.District) &&
                order.District.Equals(property.District, StringComparison.OrdinalIgnoreCase))
                score += 20;

            if (!order.Rooms.HasValue || property.Rooms >= order.Rooms.Value)
                score += 20;

            if ((!order.PriceMax.HasValue || property.Price <= order.PriceMax.Value) &&
                (!order.PriceMin.HasValue || property.Price >= order.PriceMin.Value))
                score += 15;

            
            if ((!order.TotalAreaMax.HasValue || property.TotalArea <= order.TotalAreaMax.Value) &&
                (!order.TotalAreaMin.HasValue || property.TotalArea >= order.TotalAreaMin.Value))
                score += 15;

            return (score * 100) / maxScore; 
        }

        private string GetMatchReason(Order order, Property property, int score)
        {
            var reasons = new List<string>();

            if (order.RequestDealType == property.DealType)
                reasons.Add("тип сделки");

            if (!string.IsNullOrEmpty(order.District) &&
                order.District.Equals(property.District, StringComparison.OrdinalIgnoreCase))
                reasons.Add("район");

            if (!order.Rooms.HasValue || property.Rooms >= order.Rooms.Value)
                reasons.Add("количество комнат");

            return $"Совпадение по: {string.Join(", ", reasons)} ({score}%)";
        }
    }

    public class PropertyMatch
    {
        public Property Property { get; set; }
        public int MatchScore { get; set; }
        public string MatchReason { get; set; }
    }
}