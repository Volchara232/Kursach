using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Sem3_kurs.Helpers
{
    public static class ValidatorHelper
    {
        public static bool ValidateTextBox(TextBox textBox, string pattern, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.ToolTip = "Поле обязательно для заполнения";
                textBox.Background = System.Windows.Media.Brushes.LightPink;
                return false;
            }

            if (!Regex.IsMatch(textBox.Text, pattern))
            {
                textBox.ToolTip = errorMessage;
                textBox.Background = System.Windows.Media.Brushes.LightPink;
                return false;
            }

            textBox.ToolTip = null;
            textBox.Background = System.Windows.Media.Brushes.White;
            return true;
        }

        public static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var pattern = @"^(\+7|8|7)?[\s\-]?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}$";
            return Regex.IsMatch(phone, pattern);
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool ValidateNumeric(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return true;
            return Regex.IsMatch(number, @"^\d+$");
        }

        public static bool ValidateRegistrationNumber(string regNumber)
        {
            if (string.IsNullOrWhiteSpace(regNumber)) return true;
            var pattern = @"^\d{1,4}/\d{4}$";
            return Regex.IsMatch(regNumber, pattern);
        }

        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return phone;

            var digits = Regex.Replace(phone, @"\D", "");

            if (digits.Length == 11)
            {
                if (digits.StartsWith("8"))
                    digits = "7" + digits.Substring(1);
                return $"+{digits[0]} ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 2)}-{digits.Substring(9, 2)}";
            }
            else if (digits.Length == 10)
            {
                return $"+7 ({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 2)}-{digits.Substring(8, 2)}";
            }

            return phone; 
        }
    }
}