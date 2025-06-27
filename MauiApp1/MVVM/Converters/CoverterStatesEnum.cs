using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    public class LastDeliveryStateEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var states = value as IList<DeliveryState>;
            if (states == null)
            {
                Debug.WriteLine("[LastDeliveryStateEnumConverter] states is null");
                return "Onbekend";
            }
            if (states.Count == 0)
            {
                Debug.WriteLine("[LastDeliveryStateEnumConverter] states is leeg");
                return "Onbekend";
            }
            var last = states.OrderByDescending(s => s.DateTime).FirstOrDefault();
            Debug.WriteLine($"[LastDeliveryStateEnumConverter] Laatste state: {(last != null ? last.State.ToString() : "null")}");
            return last != null ? last.State.ToString() : "Onbekend";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
