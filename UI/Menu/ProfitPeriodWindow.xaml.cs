using System;
using System.Windows;
using Sem3_kurs.Collections;

namespace Sem3_kurs
{
    public partial class ProfitPeriodWindow : Window
    {
        private StatisticsManager _statisticsManager;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public ProfitPeriodWindow(StatisticsManager statisticsManager)
        {
            InitializeComponent();
            _statisticsManager = statisticsManager;

            // Устанавливаем значения по умолчанию
            DpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            DpEndDate.SelectedDate = DateTime.Now;
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (!DpStartDate.SelectedDate.HasValue || !DpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите начальную и конечную даты", "Ошибка");
                return;
            }

            if (DpStartDate.SelectedDate.Value > DpEndDate.SelectedDate.Value)
            {
                MessageBox.Show("Начальная дата не может быть позже конечной", "Ошибка");
                return;
            }

            StartDate = DpStartDate.SelectedDate.Value;
            EndDate = DpEndDate.SelectedDate.Value;

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