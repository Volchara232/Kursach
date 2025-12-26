using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Model
{
    public class Property : BaseModel
    {
        private int _id;
        private string _type;
        private string _district;
        private string _address;
        private int _floor;
        private int _rooms;
        private double _totalArea;
        private double _livingArea;
        private double _kitchenArea;
        private bool _hasLoggia;
        private bool _hasBalcony;
        private string _houseDescription;
        private decimal _price;
        private DealType _dealType;
        private Owner _owner;
        private bool _isAvailable = true;
       
      

     

        public int Id
        {
            get => _id;
            set
            {
                if (SetField(ref _id, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Required(ErrorMessage = "Тип объекта обязателен")]
        [MinLength(2, ErrorMessage = "Тип объекта должен содержать минимум 2 символа")]
        [MaxLength(50, ErrorMessage = "Тип объекта не должен превышать 50 символов")]
        public string Type
        {
            get => _type;
            set
            {
                if (SetField(ref _type, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [MaxLength(100, ErrorMessage = "Район не должен превышать 100 символов")]
        public string District
        {
            get => _district;
            set
            {
                if (SetField(ref _district, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Required(ErrorMessage = "Адрес обязателен")]
        [MinLength(5, ErrorMessage = "Адрес должен содержать минимум 5 символов")]
        [MaxLength(200, ErrorMessage = "Адрес не должен превышать 200 символов")]
        public string Address
        {
            get => _address;
            set
            {
                if (SetField(ref _address, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Range(1, 100, ErrorMessage = "Этаж должен быть от 1 до 100")]
        public int Floor
        {
            get => _floor;
            set
            {
                if (SetField(ref _floor, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Range(1, 10, ErrorMessage = "Количество комнат должно быть от 1 до 10")]
        public int Rooms
        {
            get => _rooms;
            set
            {
                if (SetField(ref _rooms, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Range(10.0, 1000.0, ErrorMessage = "Общая площадь должна быть от 10 до 1000 м²")]
        public double TotalArea
        {
            get => _totalArea;
            set
            {
                if (SetField(ref _totalArea, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Range(5.0, 500.0, ErrorMessage = "Жилая площадь должна быть от 5 до 500 м²")]
        public double LivingArea
        {
            get => _livingArea;
            set
            {
                if (SetField(ref _livingArea, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Range(3.0, 50.0, ErrorMessage = "Площадь кухни должна быть от 3 до 50 м²")]
        public double KitchenArea
        {
            get => _kitchenArea;
            set
            {
                if (SetField(ref _kitchenArea, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public bool HasLoggia
        {
            get => _hasLoggia;
            set => SetField(ref _hasLoggia, value);
        }

        public bool HasBalcony
        {
            get => _hasBalcony;
            set => SetField(ref _hasBalcony, value);
        }

        [MaxLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string HouseDescription
        {
            get => _houseDescription;
            set => SetField(ref _houseDescription, value);
        }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(1000.0, 1000000000.0, ErrorMessage = "Цена должна быть от 1 000 до 1 000 000 000 руб.")]
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetField(ref _price, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Required(ErrorMessage = "Тип сделки обязателен")]
        public DealType DealType
        {
            get => _dealType;
            set
            {
                if (SetField(ref _dealType, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [Required(ErrorMessage = "Владелец обязателен")]
 
        public Owner Owner
        {
            get => _owner;
            set
            {
                if (SetField(ref _owner, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetField(ref _isAvailable, value);
        }

        [JsonIgnore]
        public bool IsValid => Validate();

        private bool Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            
            if (LivingArea > TotalArea)
            {
                results.Add(new ValidationResult("Жилая площадь не может быть больше общей площади"));
            }

            if (KitchenArea > TotalArea)
            {
                results.Add(new ValidationResult("Площадь кухни не может быть больше общей площади"));
            }

            if (LivingArea + KitchenArea > TotalArea)
            {
                results.Add(new ValidationResult("Сумма жилой и кухонной площади не может быть больше общей площади"));
            }

            return Validator.TryValidateObject(this, context, results, true) && results.Count == 0;
        }
        
     

        public bool MatchesOrder(Order order)
        {
            if (order == null) return false;

            // Если объект недоступен - не подходит
            if (!IsAvailable) return false;

            // Проверка типа сделки
            if (order.RequestDealType.HasValue && order.RequestDealType.Value != DealType)
                return false;

            // Проверка района (если указан в заявке)
            if (!string.IsNullOrEmpty(order.District) &&
                !string.IsNullOrEmpty(District) &&
                !string.Equals(order.District, District, StringComparison.OrdinalIgnoreCase))
                return false;

            // Проверка количества комнат (если указано в заявке)
            if (order.Rooms.HasValue && Rooms < order.Rooms.Value)
                return false;

            // Проверка площади (если указана в заявке)
            if (order.TotalAreaMin.HasValue && TotalArea < order.TotalAreaMin.Value)
                return false;

            // Проверка цены (если указана в заявке)
            if (order.PriceMin.HasValue && Price < order.PriceMin.Value)
                return false;

            if (order.PriceMax.HasValue && Price > order.PriceMax.Value)
                return false;

            return true;
        }

        public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;

        public override bool Equals(object obj)
        {
            return obj is Property property &&
                   Id == property.Id &&
                   Address == property.Address;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Address);
        }
    }
}