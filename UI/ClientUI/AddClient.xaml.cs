using System.Windows;
using Sem3_kurs.Model;
using System.ComponentModel;
using System.Linq;

namespace Sem3_kurs
{
    public partial class AddClientWindow : Window
    {
        public Client NewClient { get; private set; }

        public AddClientWindow()
        {
            InitializeComponent();
            NewClient = new Client();
            DataContext = NewClient;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Форматируем телефон
            if (!string.IsNullOrWhiteSpace(NewClient.Phone))
            {
                NewClient.Phone = Helpers.ValidatorHelper.FormatPhone(NewClient.Phone);
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