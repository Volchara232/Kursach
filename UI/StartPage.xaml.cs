using System.Windows;
using System.Windows.Controls;

namespace Sem3_kurs
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string pageType = button.Tag as string;
                MessageBox.Show($"Переход на страницу: {pageType}", "Информация");
            }
        }
    }
}