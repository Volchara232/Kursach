using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class SelectPropertyWindow : Window
    {
        public Property SelectedProperty { get; private set; }

        public SelectPropertyWindow(IEnumerable<Property> properties)
        {
            InitializeComponent();

            var propertyList = properties.ToList();
            PropertiesListBox.ItemsSource = propertyList;

            if (propertyList.Any())
            {
                PropertiesListBox.SelectedIndex = 0;
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedProperty = PropertiesListBox.SelectedItem as Property;

            if (SelectedProperty == null)
            {
                MessageBox.Show("Выберите объект недвижимости", "Ошибка");
                return;
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