using System.Windows;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class AddClientWindow : Window
    {
        public Client NewClient { get; private set; }

        public AddClientWindow()
        {
            InitializeComponent();
            NewClient = new Client();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            NewClient.FullName = TxtFullName.Text.Trim();
            NewClient.Address = TxtAddress.Text.Trim();
            NewClient.Phone = TxtPhone.Text.Trim();
            NewClient.Email = TxtEmail.Text.Trim();
            NewClient.RegistrationNumber = TxtRegistrationNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(NewClient.FullName))
            {
                MessageBox.Show("Введите ФИО клиента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(NewClient.Phone))
            {
                MessageBox.Show("Введите телефон клиента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}