using System;
using Windows.UI.Xaml.Data;

namespace ApplicationMetro.Common
{
    /// <resumo>
    /// Conversor de valor que converte true para false e vice-versa.
    /// </resumo>
    public sealed class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }
    }
}
