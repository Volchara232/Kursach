using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Sem3_kurs.Model
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        
        protected static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var pattern = @"^(\+7|8)?[\s\-]?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}$";
            return Regex.IsMatch(phone, pattern);
        }

        protected static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; 
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        protected static bool ValidateNumeric(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return true;
            return Regex.IsMatch(number, @"^\d+$");
        }
    }
}