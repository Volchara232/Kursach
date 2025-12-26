using System;
using System.Windows;
using Sem3_kurs.Model;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sem3_kurs
{
    public partial class AddOwnerWindow : Window
    {
        public Owner NewOwner { get; private set; }

        public AddOwnerWindow()
        {
            InitializeComponent();
            NewOwner = new Owner();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Заполняем объект из полей формы
            NewOwner.FullName = TxtFullName.Text.Trim();
            NewOwner.Address = TxtAddress.Text.Trim();
            NewOwner.Phone = TxtPhone.Text.Trim();
            NewOwner.Email = TxtEmail.Text.Trim();

            // Проверяем валидность
            if (!NewOwner.IsValid)
            {
                string errorMessage = GetValidationErrors(NewOwner);
                MessageBox.Show($"Исправьте ошибки в данных:\n{errorMessage}", "Ошибка валидации");
                return;
            }

            // Форматируем телефон
            NewOwner.Phone = Helpers.ValidatorHelper.FormatPhone(NewOwner.Phone);

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Вспомогательный метод для получения ошибок валидации
        private string GetValidationErrors(object obj)
        {
            var context = new ValidationContext(obj);
            var results = new System.Collections.Generic.List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                return string.Join("\n", results.Select(r => r.ErrorMessage));
            }

            return "";
        }
    }
}