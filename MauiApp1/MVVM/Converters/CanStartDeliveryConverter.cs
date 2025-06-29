// Deze code helpt ons om te bepalen of een bezorging kan starten
// We hebben dit nodig om knoppen in de app aan of uit te zetten
using System.Globalization;
using MauiApp1.ModelAPI;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MauiApp1.MVVM.Converters
{
    // Dit is een helper die kijkt of we een bezorging mogen starten
    public class CanStartDeliveryConverter : IValueConverter
    {
        // Deze functie controleert of een bezorging mag starten
        // Het geeft 'true' (ja) of 'false' (nee) terug
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Kijk of we een lijst met bezorgstatussen hebben
            if (value is IList<DeliveryState> states)
            {
                // Als er nog geen statussen zijn (een lege lijst)
                if (!states.Any())
                {
                    // Dan mag de bezorging starten! (want het is een nieuwe bezorging)
                    return true;
                }
                
                // We zoeken de laatste status van de bezorging
                var lastState = states.OrderByDescending(s => s.DateTime).First();
                
                // We mogen alleen starten als de laatste status 'In Afwachting' of 'Onderweg' is
                return lastState.State == DeliveryStateEnum.Pending || lastState.State == DeliveryStateEnum.InTransit;
            }
            
            // Als we hier komen, was er geen lijst met statussen
            // Dan mag de bezorging ook starten! (want het is een nieuwe bezorging)
            return true;
        }

        // Deze functie hebben we niet nodig, maar moeten we wel hebben
        // Het is zoals een telefoon die ook een camera aan de voorkant heeft, maar die gebruik je bijna nooit
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
