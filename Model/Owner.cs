using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sem3_kurs.Model
{
    public class Owner : BaseModel
    {
        private string _fullName;
        private string _address;
        private string _phone;
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
        [CustomValidation(typeof(Owner), nameof(ValidatePhoneNumber))]
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

        [CustomValidation(typeof(Owner), nameof(ValidateEmailAddress))]
        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        [JsonIgnore]
        public ObservableCollection<Property> Properties { get; } = new ObservableCollection<Property>();

        [JsonIgnore]
        public bool IsValid => Validate();

        public void AddProperty(Property property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            property.Owner = this;
            Properties.Add(property);
        }

        public static ValidationResult ValidatePhoneNumber(string phone, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new ValidationResult("Телефон обязателен");

            if (!ValidatePhone(phone))
                return new ValidationResult("Неверный формат телефона. Пример: +7 (999) 123-45-67");

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
            return obj is Owner owner &&
                   Phone == owner.Phone &&
                   FullName == owner.FullName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, Phone);
        }
    }
}