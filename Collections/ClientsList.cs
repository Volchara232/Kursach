using Sem3_kurs.Model;
namespace Sem3_kurs.Collections
{
    public class ClientsList
    {
        public List<Client> Clients { get; } = new List<Client>();

        public void AddClient(Client client) => Clients.Add(client);

        public IEnumerable<Client> FindByLastName(string lastName)
        {
            return Clients.Where(c => c.FullName.Split(' ')[0]
                .Equals(lastName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Client> GetClientsWithActiveOrders()
        {
            return Clients.Where(c => c.GetActiveOrders().Any());
        }
    }
}

