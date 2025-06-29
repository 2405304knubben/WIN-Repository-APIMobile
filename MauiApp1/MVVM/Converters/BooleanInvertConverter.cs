// Deze converter draait een ja/nee waarde om
// Bijvoorbeeld: als iets "waar" is, maakt deze er "niet waar" van
using System.Globalization;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper draait een boolean (ja/nee) waarde om
    public class BooleanInvertConverter : IValueConverter
    {
        // Deze functie draait de waarde om
        // Als we "true" krijgen, geeft het "false" terug en andersom
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Als we een boolean (ja/nee) waarde krijgen...
            if (value is bool boolValue)
            {
                // ...dan draaien we die om met het ! teken
                return !boolValue;
            }
            // Als het geen boolean was, geven we gewoon dezelfde waarde terug
            return value;
        }

        // Deze functie doet hetzelfde maar dan andersom
        // We hebben deze nodig als iemand een waarde in de interface aanpast
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Precies hetzelfde als hierboven
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
}
