using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Model
{
    public class Order : BaseModel
    {
        private int _id;
        private Client _client;
        private RequestType _requestType;
        private DealType? _requestDealType;
        private string _district;
        private int? _floorMin;
        private int? _floorMax;
        private int? _rooms;
        private double? _totalAreaMin;
        private double? _totalAreaMax;
        private decimal? _priceMin;
        private decimal? _priceMax;
        private string _specialConditions;
        private bool _isSpecial;
        private OrderStatus _status = OrderStatus.New;
        private DateTime _createdDate = DateTime.Now;
        private DateTime _lastUpdateDate = DateTime.Now;
        private ObservableCollection<Property> _matchedProperties = new ObservableCollection<Property>();

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        
        public Client Client
        {
            get => _client;
            set => SetField(ref _client, value);
        }

        [Required(ErrorMessage = "Тип заявки обязателен")]
        public RequestType RequestType
        {
            get => _requestType;
            set => SetField(ref _requestType, value);
        }

        public DealType? RequestDealType
        {
            get => _requestDealType;
            set => SetField(ref _requestDealType, value);
        }

        [MaxLength(100, ErrorMessage = "Район не должен превышать 100 символов")]
        public string District
        {
            get => _district;
            set => SetField(ref _district, value);
        }

        [Range(1, 100, ErrorMessage = "Минимальный этаж должен быть от 1 до 100")]
        public int? FloorMin
        {
            get => _floorMin;
            set => SetField(ref _floorMin, value);
        }

        [Range(1, 100, ErrorMessage = "Максимальный этаж должен быть от 1 до 100")]
        public int? FloorMax
        {
            get => _floorMax;
            set => SetField(ref _floorMax, value);
        }

        [Range(1, 10, ErrorMessage = "Количество комнат должно быть от 1 до 10")]
        public int? Rooms
        {
            get => _rooms;
            set => SetField(ref _rooms, value);
        }

        [Range(10.0, 1000.0, ErrorMessage = "Минимальная площадь должна быть от 10 до 1000 м²")]
        public double? TotalAreaMin
        {
            get => _totalAreaMin;
            set => SetField(ref _totalAreaMin, value);
        }

        [Range(10.0, 1000.0, ErrorMessage = "Максимальная площадь должна быть от 10 до 1000 м²")]
        public double? TotalAreaMax
        {
            get => _totalAreaMax;
            set => SetField(ref _totalAreaMax, value);
        }

        [Range(1000.0, 1000000000.0, ErrorMessage = "Минимальная цена должна быть от 1 000 до 1 000 000 000 руб.")]
        public decimal? PriceMin
        {
            get => _priceMin;
            set => SetField(ref _priceMin, value);
        }

        [Range(1000.0, 1000000000.0, ErrorMessage = "Максимальная цена должна быть от 1 000 до 1 000 000 000 руб.")]
        public decimal? PriceMax
        {
            get => _priceMax;
            set => SetField(ref _priceMax, value);
        }

        [MaxLength(500, ErrorMessage = "Особые условия не должны превышать 500 символов")]
        public string SpecialConditions
        {
            get => _specialConditions;
            set => SetField(ref _specialConditions, value);
        }

        public bool IsSpecial
        {
            get => _isSpecial;
            set => SetField(ref _isSpecial, value);
        }

        public OrderStatus Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;

        }

        public DateTime LastUpdateDate
        {
            get => _lastUpdateDate;
            set => SetField(ref _lastUpdateDate, value);
        }

        [JsonIgnore]
        public ObservableCollection<Property> MatchedProperties
        {
            get => _matchedProperties;
        }

        [JsonIgnore]
        public bool IsValid => Validate();

        private bool Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();

          
            if (PriceMin.HasValue && PriceMax.HasValue && PriceMax.Value < PriceMin.Value)
            {
                results.Add(new ValidationResult("Максимальная цена должна быть больше или равна минимальной"));
            }

            if (TotalAreaMin.HasValue && TotalAreaMax.HasValue && TotalAreaMax.Value < TotalAreaMin.Value)
            {
                results.Add(new ValidationResult("Максимальная площадь должна быть больше или равна минимальной"));
            }

            return Validator.TryValidateObject(this, context, results, true) && results.Count == 0;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            LastUpdateDate = DateTime.Now;
        }

        public void SetMatchedProperties(IEnumerable<Property> properties)
        {
            MatchedProperties.Clear();
            foreach (var property in properties)
            {
                MatchedProperties.Add(property);
            }
            UpdateStatus(OrderStatus.OffersPrepared);
        }

        public string GetShortDescription()
        {
            return $"{RequestType}, {District}, {Rooms}к, {PriceMin}-{PriceMax}";
        }
    }
}