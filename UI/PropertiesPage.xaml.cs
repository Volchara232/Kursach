using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;

namespace Sem3_kurs
{
    public partial class PropertiesPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;

        public PropertiesPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            PropertiesListView.Items.Clear();
            foreach (var property in _agency.PropertiesList)
            {
                PropertiesListView.Items.Add(new
                {
                    property.Type,
                    property.District,
                    property.Rooms,
                    property.TotalArea,
                    property.Price,
                    property.DealType,
                    property.IsAvailable
                });
            }
        }

        private void BtnAddProperty_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления объекта будет реализована позже", "Информация");
        }

        private void BtnFilterAvailable_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция фильтрации будет реализована позже", "Информация");
        }
    }
}