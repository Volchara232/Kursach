using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class OrdersPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;

        public OrdersPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            OrdersListView.Items.Clear();
            foreach (var order in _agency.OrdersList)
            {
                OrdersListView.Items.Add(new
                {
                    order.Id,
                    ClientName = order.Client?.FullName ?? "Не указан",
                    order.RequestType,
                    order.District,
                    order.PriceMin,
                    order.Status,
                    CreatedDate = order.CreatedDate.ToShortDateString()
                });
            }
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления заявки будет реализована позже", "Информация");
        }

        private void BtnShowActive_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция фильтрации будет реализована позже", "Информация");
        }
    }
}