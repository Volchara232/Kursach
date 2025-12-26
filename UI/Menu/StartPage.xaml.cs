using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;

namespace Sem3_kurs
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Получаем доступ к главному окну и данным
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null && mainWindow.Agency != null)
            {
                var agency = mainWindow.Agency;
                ClientsCount.Text = agency.ClientsList.Clients.Count.ToString();

                // Считаем активные заявки
                int activeOrders = 0;
                foreach (var client in agency.ClientsList.Clients)
                {
                    activeOrders += client.GetActiveOrders().Count();
                }
                ActiveOrdersCount.Text = activeOrders.ToString();
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string pageType = button.Tag.ToString();
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    switch (pageType)
                    {
                        case "Clients":
                            mainWindow.BtnClients_Click(sender, e);
                            break;
                        case "Properties":
                            mainWindow.BtnProperties_Click(sender, e);
                            break;
                        case "Statistics":
                            mainWindow.BtnStatistics_Click(sender, e);
                            break;
                    }
                }
            }
        }
    }
}