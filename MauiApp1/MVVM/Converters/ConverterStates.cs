// Deze converter maakt van een bezorgstatus een mooi Nederlands berichtje
// Het is als een vertaler die computertaal omzet naar mensentaal
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper maakt mooie Nederlandse statusberichten van bezorgstatussen
    public class LastDeliveryStateToStatusConverter : IValueConverter
    {
        // Deze functie zoekt de laatste status en maakt er een duidelijk berichtje van
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // We proberen een lijst met statussen te krijgen
            var states = value as IList<DeliveryState>;
            
            // Als er geen lijst is...
            if (states == null)
            {
                // Dan schrijven we dat in het logboek en zeggen we "Onbekend"
                Debug.WriteLine("[LastDeliveryStateToStatusConverter] states is null");
                return "Onbekend";
            }
            // Als de lijst leeg is...
            if (states.Count == 0)
            {
                // Dan schrijven we dat in het logboek en zeggen we "Onbekend"
                Debug.WriteLine("[LastDeliveryStateToStatusConverter] states is leeg");
                return "Onbekend";
            }
            
            // We zoeken de laatste status
            var last = states.OrderByDescending(s => s.DateTime).FirstOrDefault();
            // We schrijven in het logboek welke status we hebben gevonden
            Debug.WriteLine($"[LastDeliveryStateToStatusConverter] Laatste state: {(last != null ? last.State.ToString() : "null")}");
            
            // We maken er een mooi Nederlands berichtje van
            return last?.State switch
            {
                DeliveryStateEnum.Pending => "Besteld",
                DeliveryStateEnum.InTransit => "Onderweg",
                DeliveryStateEnum.Delivered => "Bezorgd",
                _ => "Onbekend"
            };
        }

        // Deze functie hebben we niet nodig
        // Het is als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
