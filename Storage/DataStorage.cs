
using Sem3_kurs.Collections;
using Sem3_kurs.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sem3_kurs
{
    public class DataStorage
    {
        private readonly string _filePath;

        public DataStorage(string filePath)
        {
            _filePath = filePath;
        }

        public void Save(EstateAgency agency)
        {
            try
            {
                var data = new AgencyData
                {
                    Clients = agency.ClientsList.Clients.ToList(),
                    Owners = agency.OwnersList.Owners.ToList(),
                    Properties = agency.PropertiesList.ToList(),
                    Orders = agency.OrdersList.ToList(),
                    Deals = agency.DealsList.ToList(),
                    Receipts = agency.ReceiptsList.ToList()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve, 
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(_filePath, json);

                Console.WriteLine($"Данные сохранены в: {_filePath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении данных: {ex.Message}", ex);
            }
        }

        public EstateAgency Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"Файл не найден: {_filePath}");
                    return new EstateAgency();
                }

                Console.WriteLine($"Загрузка данных из: {_filePath}");
                var json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("Файл пуст");
                    return new EstateAgency();
                }

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve 
                };

                var data = JsonSerializer.Deserialize<AgencyData>(json, options);

                if (data == null)
                {
                    Console.WriteLine("Не удалось десериализовать данные");
                    return new EstateAgency();
                }

                var agency = new EstateAgency();

                var clientsById = new Dictionary<string, Client>();
                var ownersById = new Dictionary<string, Owner>();
                var propertiesById = new Dictionary<int, Property>();

                // 1. Загружаем клиентов
                foreach (var client in data.Clients)
                {
                    agency.ClientsList.AddClient(client);
                    clientsById[GetClientKey(client)] = client;
                }

                // 2. Загружаем владельцев
                foreach (var owner in data.Owners)
                {
                    agency.OwnersList.AddOwner(owner);
                    ownersById[GetOwnerKey(owner)] = owner;
                }

                // 3. Загружаем свойства
                foreach (var property in data.Properties)
                {
                    agency.PropertiesList.Add(property);
                    propertiesById[property.Id] = property;
                }

                // 4. Загружаем заявки
                foreach (var order in data.Orders)
                {
                    agency.OrdersList.Add(order);
                }

                // 5. Восстанавливаем связи КЛИЕНТ-ЗАЯВКА
                foreach (var order in data.Orders)
                {
                    if (order.Client != null)
                    {
                        var clientKey = GetClientKey(order.Client);
                        if (clientsById.TryGetValue(clientKey, out var client))
                        {
                            order.Client = client;
                            // Добавляем заявку в коллекцию клиента, если ее там еще нет
                            if (!client.Orders.Any(o => o.Id == order.Id))
                            {
                                client.Orders.Add(order);
                            }
                        }
                    }
                }

                // 6. Восстанавливаем связи ВЛАДЕЛЕЦ-ОБЪЕКТ
                foreach (var property in data.Properties)
                {
                    if (property.Owner != null)
                    {
                        var ownerKey = GetOwnerKey(property.Owner);
                        if (ownersById.TryGetValue(ownerKey, out var owner))
                        {
                            property.Owner = owner;
                            // Добавляем объект в коллекцию владельца, если его там еще нет
                            if (!owner.Properties.Any(p => p.Id == property.Id))
                            {
                                owner.Properties.Add(property);
                            }
                        }
                    }
                }

                // 7. Загружаем сделки и восстанавливаем их связи
                foreach (var deal in data.Deals)
                {
                    // Восстанавливаем связь с заявкой
                    if (deal.Order != null)
                    {
                        var order = data.Orders.FirstOrDefault(o => o.Id == deal.Order.Id);
                        if (order != null)
                        {
                            deal.Order = order;
                        }
                    }

                    // Восстанавливаем связь с объектом
                    if (deal.Property != null)
                    {
                        if (propertiesById.TryGetValue(deal.Property.Id, out var property))
                        {
                            deal.Property = property;
                        }
                    }

                    agency.DealsList.Add(deal);
                }

                // 8. Загружаем квитанции
                foreach (var receipt in data.Receipts)
                {
                    if (receipt.Client != null)
                    {
                        var clientKey = GetClientKey(receipt.Client);
                        if (clientsById.TryGetValue(clientKey, out var client))
                        {
                            receipt.Client = client;
                        }
                    }
                    agency.ReceiptsList.Add(receipt);
                }

                Console.WriteLine($"Загружено: {data.Clients.Count} клиентов, {data.Properties.Count} объектов, {data.Orders.Count} заявок");
                return agency;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                throw new Exception($"Ошибка при загрузке данных: {ex.Message}", ex);
            }
        }

        // Методы для создания уникальных ключей
        private string GetClientKey(Client client)
        {
            return $"{client.FullName}_{client.Phone}";
        }

        private string GetOwnerKey(Owner owner)
        {
            return $"{owner.FullName}_{owner.Phone}";
        }

        private class AgencyData
        {
            public List<Client> Clients { get; set; } = new();
            public List<Owner> Owners { get; set; } = new();
            public List<Property> Properties { get; set; } = new();
            public List<Order> Orders { get; set; } = new();
            public List<Deal> Deals { get; set; } = new();
            public List<Receipt> Receipts { get; set; } = new();
        }
    }
}
