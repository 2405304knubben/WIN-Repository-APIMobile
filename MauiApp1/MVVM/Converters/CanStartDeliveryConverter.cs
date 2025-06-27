using System.Globalization;
using MauiApp1.ModelAPI;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MauiApp1.MVVM.Converters
{
    public class CanStartDeliveryConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IList<DeliveryState> states)
            {
                if (!states.Any())
                {
                    // Als er nog geen states zijn, kan de bezorging starten
                    return true;
                }                var lastState = states.OrderByDescending(s => s.DateTime).First();
                // Toestaan voor Pending en InTransit
                return lastState.State == DeliveryStateEnum.Pending || lastState.State == DeliveryStateEnum.InTransit;
            }
            // Als er geen states zijn, kan de bezorging starten
            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
