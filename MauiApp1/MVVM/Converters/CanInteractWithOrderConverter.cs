using System.Globalization;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    public class CanInteractWithOrderConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Order order)
            {
                var lastState = order.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault();
                var lastStateEnum = lastState?.State;

                if (parameter is bool invertLogic && invertLogic)
                {
                    // Voor InputTransparent: true betekent niet-klikbaar
                    // Alleen niet-klikbaar maken als het DELIVERED is
                    return lastStateEnum == DeliveryStateEnum.Delivered;
                }
                
                // Voor opacity: 1.0 voor actief, 0.5 voor delivered
                return lastStateEnum == DeliveryStateEnum.Delivered ? 0.5 : 1.0;
            }
            
            return parameter is bool inv && inv ? false : 1.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
