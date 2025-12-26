using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Sem3_kurs.Enums;

namespace Sem3_kurs.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                switch (status)
                {
                    case OrderStatus.New: return Brushes.Green;
                    case OrderStatus.InProgress: return Brushes.Orange;
                    case OrderStatus.OffersPrepared: return Brushes.Blue;
                    case OrderStatus.Completed: return Brushes.DarkGreen;
                    case OrderStatus.Rejected: return Brushes.Red;
                    case OrderStatus.Archived: return Brushes.Gray;
                    default: return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}