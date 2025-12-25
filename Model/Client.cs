using System.Windows.Controls;
using Sem3_kurs.Enums;
namespace Sem3_kurs.Model
{
    public class Client
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RegistrationNumber { get; set; }
        public string Email { get; set; }
        public List<Order> Orders { get; } = new List<Order>();

        public void AddOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Orders.Add(order);
        }

        public IEnumerable<Order> GetActiveOrders()
        {
            return Orders.Where(o => o.Status == OrderStatus.New
                                  || o.Status == OrderStatus.InProgress
                                  || o.Status == OrderStatus.OffersPrepared);
        }
    }
}