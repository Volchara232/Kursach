using System;
using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using Sem3_kurs.Enums;

namespace Sem3_kurs
{
    public partial class StatisticsPage : Page
    {
        private StatisticsManager _statisticsManager;

        public StatisticsPage(StatisticsManager statisticsManager)
        {
            InitializeComponent(); 
            _statisticsManager = statisticsManager;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            try
            {
          
                if (_statisticsManager == null)
                {
                    TxtPopularByDemand.Text = "Статистика не доступна";
                    return;
                }


                var popularRentByDemand = _statisticsManager.GetMostPopularPropertyTypeByDemand(DealType.Rent);
                var popularSaleByDemand = _statisticsManager.GetMostPopularPropertyTypeByDemand(DealType.Sale);

                TxtPopularByDemand.Text = $"Спрос на аренду: {popularRentByDemand ?? "нет данных"}\n" +
                                         $"Спрос на продажу: {popularSaleByDemand ?? "нет данных"}";


                var popularRentByOffers = _statisticsManager.GetMostPopularPropertyTypeByOffers(DealType.Rent);
                var popularSaleByOffers = _statisticsManager.GetMostPopularPropertyTypeByOffers(DealType.Sale);

                TxtPopularByOffers.Text = $"Предложения аренды: {popularRentByOffers ?? "нет данных"}\n" +
                                         $"Предложения продажи: {popularSaleByOffers ?? "нет данных"}";

         
                var fromDate = DateTime.Now.AddMonths(-1);
                var totalProfit = _statisticsManager.GetTotalProfit(fromDate, DateTime.Now);
                TxtTotalProfit.Text = $"Прибыль за последний месяц: {totalProfit:C}";

                TxtTotalDeals.Text = "Всего сделок: (будет реализовано)";
                TxtTotalClients.Text = "Всего клиентов: (будет реализовано)";
                TxtTotalProperties.Text = "Всего объектов: (будет реализовано)";
            }
            catch (Exception ex)
            {
                TxtPopularByDemand.Text = $"Ошибка при загрузке статистики: {ex.Message}";
            }
        }

        private void BtnCalculateProfit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция расчета прибыли за произвольный период будет реализована позже", "Информация");
        }
    }
}