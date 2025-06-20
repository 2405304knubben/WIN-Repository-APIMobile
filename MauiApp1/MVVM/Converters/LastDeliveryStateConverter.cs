using System.Globalization;
using MauiApp1.ModelAPI;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MauiApp1.MVVM.Converters
{
    public class LastDeliveryStateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IList<DeliveryState> states && states.Any())
            {
                var lastState = states.OrderByDescending(s => s.DateTime).First();                return lastState.State switch
                {
                    DeliveryStateEnum.Pending => "Wachtend op bezorging",
                    DeliveryStateEnum.InTransit => "Onderweg",
                    DeliveryStateEnum.Delivered => "Bezorgd",
                    DeliveryStateEnum.Cancelled => "Geannuleerd",
                    _ => "Status onbekend"
                };
            }
            return "Nog niet gestart";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
