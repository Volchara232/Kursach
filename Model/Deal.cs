using Sem3_kurs.Model;
using Sem3_kurs.Enums;
namespace Sem3_kurs.Model
{
    public class Deal
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Property Property { get; set; }
        public DealType DealType { get; set; }

        public decimal Amount { get; set; }          
        public decimal AgencyFee { get; private set; } 
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }

        private const decimal FeeRate = 0.02m;

        public void CalculateAgencyFee()
        {
            AgencyFee = Amount * FeeRate;
        }

        public string GetShortReport()
        {
            return $"{Date:d}: {DealType}, сумма {Amount}, гонорар {AgencyFee}";
        }
    }
}
