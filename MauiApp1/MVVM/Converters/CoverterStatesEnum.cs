// Deze converter geeft de technische naam van een bezorgstatus
// Het is als een tolk die de computertaal onvertaald doorgeeft
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper geeft de ruwe statuscode van een bezorging
    public class LastDeliveryStateEnumConverter : IValueConverter
    {
        // Deze functie zoekt de laatste status en geeft de technische naam
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // We proberen een lijst met statussen te krijgen
            var states = value as IList<DeliveryState>;
            
            // Als er geen lijst is...
            if (states == null)
            {
                // Dan schrijven we dat in het logboek en zeggen we "Onbekend"
                Debug.WriteLine("[LastDeliveryStateEnumConverter] states is null");
                return "Onbekend";
            }
            // Als de lijst leeg is...
            if (states.Count == 0)
            {
                // Dan schrijven we dat in het logboek en zeggen we "Onbekend"
                Debug.WriteLine("[LastDeliveryStateEnumConverter] states is leeg");
                return "Onbekend";
            }
            
            // We zoeken de laatste status
            var last = states.OrderByDescending(s => s.DateTime).FirstOrDefault();
            // We schrijven in het logboek welke status we hebben gevonden
            Debug.WriteLine($"[LastDeliveryStateEnumConverter] Laatste state: {(last != null ? last.State.ToString() : "null")}");
            
            // We geven de technische naam terug (bijvoorbeeld "InTransit" of "Delivered")
            return last != null ? last.State.ToString() : "Onbekend";
        }

        // Deze functie hebben we niet nodig
        // Het is als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
