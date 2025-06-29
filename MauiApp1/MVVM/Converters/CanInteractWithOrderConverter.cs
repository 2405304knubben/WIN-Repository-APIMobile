// Deze converter bepaalt of we iets met een bestelling kunnen doen
// Het is als een stoplicht: groen betekent dat we er iets mee kunnen doen, rood betekent dat we moeten wachten
using System.Globalization;
using MauiApp1.ModelAPI;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper kijkt of een bestelling actief is (of we er iets mee kunnen doen)
    public class CanInteractWithOrderConverter : IValueConverter
    {
        // Deze functie controleert of we met een bestelling kunnen werken
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Als we een bestelling krijgen...
            if (value is Order order)
            {
                // We kijken naar de laatste status van de bestelling
                var lastState = order.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault();
                var lastStateEnum = lastState?.State;

                // Een bestelling is actief als:
                // - De status 'In Afwachting' is, of
                // - De status 'Onderweg' is, of
                // - Er nog geen status is (nieuwe bestelling)
                bool isInteractive = lastStateEnum == DeliveryStateEnum.Pending || 
                                   lastStateEnum == DeliveryStateEnum.InTransit || 
                                   lastState == null;

                // Als we de doorzichtigheid willen weten...
                if (parameter is string param && param == "Opacity")
                {
                    // Actieve bestellingen zijn helemaal zichtbaar (1.0)
                    // Niet-actieve bestellingen zijn half doorzichtig (0.5)
                    return isInteractive ? 1.0 : 0.5;
                }

                // Als we willen weten of je erop kunt klikken...
                if (parameter is string paramFlag && paramFlag == "InputTransparent")
                {
                    // InputTransparent: true betekent dat je er NIET op kunt klikken
                    // We draaien isInteractive om omdat InputTransparent het tegenovergestelde betekent
                    return !isInteractive;
                }

                // In alle andere gevallen geven we gewoon terug of de bestelling actief is
                return isInteractive;
            }

            // Als we geen bestelling krijgen, gebruiken we standaardwaardes:
            // - Opacity (doorzichtigheid) = 1.0 (helemaal zichtbaar)
            // - InputTransparent = false (je kunt erop klikken)
            // - Anders = true (je kunt ermee werken)
            if (parameter is string p && p == "Opacity") return 1.0;
            if (parameter is string p2 && p2 == "InputTransparent") return false;
            return true;
        }

        // Deze functie hebben we niet nodig
        // Het is als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
