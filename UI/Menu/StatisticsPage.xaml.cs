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

                // Популярность по спросу и предложениям
                var popularRentByDemand = _statisticsManager.GetMostPopularPropertyTypeByDemand(DealType.Rent);
                var popularSaleByDemand = _statisticsManager.GetMostPopularPropertyTypeByDemand(DealType.Sale);

                TxtPopularByDemand.Text = $"Спрос на аренду: {popularRentByDemand ?? "нет данных"}\n" +
                                         $"Спрос на продажу: {popularSaleByDemand ?? "нет данных"}";

                var popularRentByOffers = _statisticsManager.GetMostPopularPropertyTypeByOffers(DealType.Rent);
                var popularSaleByOffers = _statisticsManager.GetMostPopularPropertyTypeByOffers(DealType.Sale);

                TxtPopularByOffers.Text = $"Предложения аренды: {popularRentByOffers ?? "нет данных"}\n" +
                                         $"Предложения продажи: {popularSaleByOffers ?? "нет данных"}";

                // Прибыль
                var fromDate = DateTime.Now.AddMonths(-1);
                var totalProfit = _statisticsManager.GetTotalProfit(fromDate, DateTime.Now);
                TxtTotalProfit.Text = $"Прибыль за последний месяц: {totalProfit:C}\n" +
                                     $"Общая прибыль: {_statisticsManager.GetTotalCommission():C}";

                // Общая статистика
                TxtTotalDeals.Text = $"Всего сделок: {_statisticsManager.GetTotalDealsCount()}\n" +
                                   $"Завершенных: {_statisticsManager.GetCompletedDealsCount()}";

                TxtTotalClients.Text = $"Всего клиентов: {_statisticsManager.GetTotalClientsCount()}\n" +
                                     $"Активных заявок: {_statisticsManager.GetActiveOrdersCount()}";

                TxtTotalProperties.Text = $"Всего объектов: {_statisticsManager.GetTotalPropertiesCount()}\n" +
                                        $"Доступных: {_statisticsManager.GetAvailablePropertiesCount()}";

                // Дополнительная информация
                var bestClient = _statisticsManager.GetBestClient();
                var profitableDealType = _statisticsManager.GetMostProfitableDealType();

                // Можно добавить дополнительные TextBlock для этой информации
            }
            catch (Exception ex)
            {
                TxtPopularByDemand.Text = $"Ошибка при загрузке статистики: {ex.Message}";
            }
        }

        private void BtnCalculateProfit_Click(object sender, RoutedEventArgs e)
        {
            // Создаем окно для выбора периода
            var periodWindow = new ProfitPeriodWindow(_statisticsManager);
            if (periodWindow.ShowDialog() == true)
            {
                var profit = _statisticsManager.GetTotalProfit(
                    periodWindow.StartDate,
                    periodWindow.EndDate
                );

                MessageBox.Show($"Прибыль за период с {periodWindow.StartDate:d} по {periodWindow.EndDate:d}:\n" +
                              $"{profit:C}", "Результат расчета");
            }
        }
    }
}