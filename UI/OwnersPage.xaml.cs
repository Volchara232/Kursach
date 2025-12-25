using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;

namespace Sem3_kurs
{
    public partial class OwnersPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;

        public OwnersPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            OwnersListView.Items.Clear();
            foreach (var owner in _agency.OwnersList.Owners)
            {
                OwnersListView.Items.Add(new
                {
                    owner.FullName,
                    owner.Phone,
                    owner.Email,
                    PropertiesCount = owner.Properties.Count
                });
            }
        }

        private void BtnAddOwner_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления владельца будет реализована позже", "Информация");
        }
    }
}