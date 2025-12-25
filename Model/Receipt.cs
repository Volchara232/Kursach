
namespace Sem3_kurs.Model
{
    public class Receipt
    {
        public string Number { get; set; } 
        public Client Client { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string ToText()
        {
            return $"Квитанция №{Number} от {Date:d}\n" +
                   $"Клиент: {Client?.FullName}\n" +
                   $"Сумма: {Amount} руб.";
        }
    }

}
