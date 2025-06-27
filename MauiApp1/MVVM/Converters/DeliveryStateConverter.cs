using System.Globalization;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    public class DeliveryStateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DeliveryStateEnum state)
            {
                return state switch
                {                    DeliveryStateEnum.Pending => "Wachtend op bezorging",
                    DeliveryStateEnum.InTransit => "Onderweg",
                    DeliveryStateEnum.Delivered => "Bezorgd",
                    DeliveryStateEnum.Cancelled => "Geannuleerd",
                    _ => "Onbekend"
                };
            }
            return "Onbekend";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}