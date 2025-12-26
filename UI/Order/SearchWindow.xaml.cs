using System.Windows;
using System.Windows.Controls;

namespace Sem3_kurs
{
    public partial class SearchWindow : Window
    {
        public string SearchText { get; private set; }

        public SearchWindow(string prompt)
        {
            InitializeComponent();
            TxtPrompt.Text = prompt;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchText = TxtSearch.Text.Trim();
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