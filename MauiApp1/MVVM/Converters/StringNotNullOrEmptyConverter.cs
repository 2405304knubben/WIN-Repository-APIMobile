// Deze converter kijkt of er tekst is ingevuld
// Het is als een formulier waar je controleert of iemand iets heeft ingevuld
using System.Globalization;

namespace MauiApp1.MVVM.Converters
{
    // Deze helper controleert of er tekst is ingevuld of niet
    public class StringNotNullOrEmptyConverter : IValueConverter
    {
        // Deze functie controleert of er tekst is
        // Het geeft "true" (ja) als er tekst is, en "false" (nee) als het leeg is
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Als we tekst krijgen...
            if (value is string str)
            {
                // Dan controleren we of er iets is ingevuld (niet leeg of null)
                return !string.IsNullOrEmpty(str);
            }
            // Als we geen tekst krijgen, dan is het antwoord "nee" (false)
            return false;
        }

        // Deze functie hebben we niet nodig
        // Het is als een knop die niets doet als je erop drukt
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
