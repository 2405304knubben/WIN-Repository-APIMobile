// Deze converter kijkt naar de laatste status van een bezorging
// Het is als een pakketvolgsysteem dat laat zien waar je pakje nu is
using System.Globalization;
using MauiApp1.ModelAPI;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper laat de meest recente status van een bezorging zien
    public class LastDeliveryStateConverter : IValueConverter
    {
        // Deze functie zoekt de laatste status en maakt er een mooi Nederlands berichtje van
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Als we een lijst met statussen krijgen en er zitten statussen in...
            if (value is IList<DeliveryState> states && states.Any())
            {
                // We zoeken de nieuwste status (met de laatste datum/tijd)
                var lastState = states.OrderByDescending(s => s.DateTime).First();
                
                // Dan kijken we welke status het is en maken we er een mooi berichtje van
                return lastState.State switch
                {
                    DeliveryStateEnum.Pending => "Wachtend op bezorging",
                    DeliveryStateEnum.InTransit => "Onderweg",
                    DeliveryStateEnum.Delivered => "Bezorgd",
                    DeliveryStateEnum.Cancelled => "Geannuleerd",
                    _ => "Status onbekend"
                };
            }
            // Als er nog geen statussen zijn, dan is de bezorging nog niet gestart
            return "Nog niet gestart";
        }

        // Deze functie hebben we niet nodig
        // Het is als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
