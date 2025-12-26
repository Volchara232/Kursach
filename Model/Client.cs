using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Model
{
    public class Client : BaseModel
    {
        private string _fullName;
        private string _address;
        private string _phone;
        private string _registrationNumber;
        private string _email;

        [Required(ErrorMessage = "ФИО обязательно")]
        [MinLength(5, ErrorMessage = "ФИО должно содержать не менее 5 символов")]
        [MaxLength(100, ErrorMessage = "ФИО не должно превышать 100 символов")]
        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetField(ref _fullName, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [MaxLength(200, ErrorMessage = "Адрес не должен превышать 200 символов")]
        public string Address
        {
            get => _address;
            set => SetField(ref _address, value);
        }

        [Required(ErrorMessage = "Телефон обязателен")]
        [CustomValidation(typeof(Client), nameof(ValidatePhoneNumber))]
        public string Phone
        {
            get => _phone;
            set
            {
                if (SetField(ref _phone, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [CustomValidation(typeof(Client), nameof(ValidateRegistrationNumber))]
        public string RegistrationNumber
        {
            get => _registrationNumber;
            set
            {
                if (SetField(ref _registrationNumber, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [CustomValidation(typeof(Client), nameof(ValidateEmailAddress))]
        public string Email
        {
            get => _email;
            set
            {
                if (SetField(ref _email, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        [JsonIgnore]
        public ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();

        [JsonIgnore]
        public bool IsValid => Validate();

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

        // Методы валидации
        public static ValidationResult ValidatePhoneNumber(string phone, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new ValidationResult("Телефон обязателен");

            if (!ValidatePhone(phone))
                return new ValidationResult("Неверный формат телефона. Пример: +7 (999) 123-45-67");

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateRegistrationNumber(string regNumber, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(regNumber))
                return ValidationResult.Success; // Не обязателен

            // Формат: цифры/год
            var pattern = @"^\d{1,4}/\d{4}$";
            if (!Regex.IsMatch(regNumber, pattern))
                return new ValidationResult("Неверный формат регистрационного номера. Пример: 123/2024");

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateEmailAddress(string email, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ValidationResult.Success; 

            if (!ValidateEmail(email))
                return new ValidationResult("Неверный формат email");

            return ValidationResult.Success;
        }

        private bool Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }

        public override bool Equals(object obj)
        {
            if (obj is Client client)
            {
                return !string.IsNullOrEmpty(RegistrationNumber) &&
                       !string.IsNullOrEmpty(client.RegistrationNumber) &&
                       RegistrationNumber == client.RegistrationNumber ||
                       (!string.IsNullOrEmpty(FullName) && FullName == client.FullName &&
                        !string.IsNullOrEmpty(Phone) && Phone == client.Phone);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(RegistrationNumber))
                return RegistrationNumber.GetHashCode();

            return HashCode.Combine(FullName, Phone);
        }
    }
}