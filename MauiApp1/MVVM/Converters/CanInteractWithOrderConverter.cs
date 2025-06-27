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

                // Een bestelling is interactief als de status 'Pending' of 'InTransit' is.
                bool isInteractive = lastStateEnum == DeliveryStateEnum.Pending || lastStateEnum == DeliveryStateEnum.InTransit || lastState == null;

                if (parameter is string param && param == "Opacity")
                {
                    return isInteractive ? 1.0 : 0.5;
                }

                if (parameter is string paramFlag && paramFlag == "InputTransparent")
                {
                    // InputTransparent: true betekent niet-klikbaar.
                    return !isInteractive;
                }

                return isInteractive;
            }

            // Standaardgedrag
            if (parameter is string p && p == "Opacity") return 1.0;
            if (parameter is string p2 && p2 == "InputTransparent") return false;

            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
