using System;
using System.Collections.Generic;
using System.Linq;
using Sem3_kurs.Model;

namespace Sem3_kurs.Collections
{
    public class ClientsList
    {
        public List<Client> Clients { get; } = new List<Client>();

        public void AddClient(Client client) => Clients.Add(client);

        public void RemoveClient(Client client) => Clients.Remove(client);

        public IEnumerable<Client> FindByLastName(string lastName)
        {
            return Clients.Where(c => c.FullName.Split(' ')[0]
                .Equals(lastName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Client> FindByPhone(string phone)
        {
            return Clients.Where(c => c.Phone.Contains(phone));
        }

        public Client FindById(string id)
        {
            return Clients.FirstOrDefault(c => c.RegistrationNumber == id);
        }

        public IEnumerable<Client> GetClientsWithActiveOrders()
        {
            return Clients.Where(c => c.GetActiveOrders().Any());
        }

        public IEnumerable<Client> SearchClients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return Clients;

            searchText = searchText.ToLower();
            return Clients.Where(c =>
                c.FullName.ToLower().Contains(searchText) ||
                c.Phone.Contains(searchText) ||
                c.Email?.ToLower().Contains(searchText) == true ||
                c.RegistrationNumber?.Contains(searchText) == true);
        }
    }
}