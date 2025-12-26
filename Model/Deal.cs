using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Model
{
    public class Deal : BaseModel
    {
        private int _id;
        private Order _order;
        private Property _property;
        private DealType _dealType;
        private decimal _amount;
        private decimal _agencyFee;
        private DateTime _date = DateTime.Now;
        private bool _isCompleted;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        [JsonIgnore]
        public Order Order
        {
            get => _order;
            set => SetField(ref _order, value);
        }

        [JsonIgnore]
        public Property Property
        {
            get => _property;
            set => SetField(ref _property, value);
        }

        [Required(ErrorMessage = "Тип сделки обязателен")]
        public DealType DealType
        {
            get => _dealType;
            set => SetField(ref _dealType, value);
        }

        [Range(1000.0, 1000000000.0, ErrorMessage = "Сумма сделки должна быть от 1 000 до 1 000 000 000 руб.")]
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetField(ref _amount, value))
                {
                    CalculateAgencyFee();
                }
            }
        }

        public decimal AgencyFee
        {
            get => _agencyFee;
            private set => SetField(ref _agencyFee, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetField(ref _date, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetField(ref _isCompleted, value);
        }

        [JsonIgnore]
        public bool IsValid => Validate();

        private const decimal FeeRate = 0.02m;

        private bool Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }

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