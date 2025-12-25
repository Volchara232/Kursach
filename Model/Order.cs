using Sem3_kurs.Enums;
namespace Sem3_kurs.Model
{


    public class Order
    {
        public int Id { get; set; }
        public Client Client { get; set; }

        public RequestType RequestType { get; set; }
        public DealType? RequestDealType { get; set; } 
        public string District { get; set; }

        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }

        public int? Rooms { get; set; }
        public double? TotalAreaMin { get; set; }
        public double? TotalAreaMax { get; set; }

        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        public string SpecialConditions { get; set; }
        public bool IsSpecial { get; set; }

        public OrderStatus Status { get; private set; } = OrderStatus.New;

        public DateTime CreatedDate { get; } = DateTime.Now;
        public DateTime LastUpdateDate { get; private set; } = DateTime.Now;

        public List<Property> MatchedProperties { get; } = new List<Property>();

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            LastUpdateDate = DateTime.Now;
        }

        public void SetMatchedProperties(IEnumerable<Property> properties)
        {
            MatchedProperties.Clear();
            MatchedProperties.AddRange(properties);
            UpdateStatus(OrderStatus.OffersPrepared);
        }

        public string GetShortDescription()
        {
            return $"{RequestType}, {District}, {Rooms}к, {PriceMin}-{PriceMax}";
        }
    }
}