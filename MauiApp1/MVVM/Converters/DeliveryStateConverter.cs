// Dit bestand zorgt ervoor dat we bezorgstatussen in mooie Nederlandse tekst kunnen laten zien
// Het vertaalt bijvoorbeeld "InTransit" naar "Onderweg"
using System.Globalization;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper maakt mooie Nederlandse teksten van bezorgstatussen
    public class DeliveryStateConverter : IValueConverter
    {
        // Deze functie vertaalt een bezorgstatus naar een mooi Nederlands berichtje
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Als we een bezorgstatus krijgen
            if (value is DeliveryStateEnum state)
            {
                // Dan kijken we welke status het is en geven we de juiste tekst terug
                return state switch
                {
                    // Als het pakketje wacht om bezorgd te worden
                    DeliveryStateEnum.Pending => "Wachtend op bezorging",
                    // Als het pakketje onderweg is
                    DeliveryStateEnum.InTransit => "Onderweg",
                    // Als het pakketje is aangekomen
                    DeliveryStateEnum.Delivered => "Bezorgd",
                    // Als de bezorging is gestopt
                    DeliveryStateEnum.Cancelled => "Geannuleerd",
                    // Als we de status niet kennen
                    _ => "Onbekend"
                };
            }
            // Als we geen status krijgen, zeggen we dat het onbekend is
            return "Onbekend";
        }

        // Deze functie hebben we niet nodig, maar moeten we wel hebben
        // Het is net als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}