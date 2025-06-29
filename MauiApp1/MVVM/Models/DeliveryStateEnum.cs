// Dit bestand beschrijft alle mogelijke statussen die een bezorging kan hebben
// Het is als een lijstje met stappen die een pakketje doorloopt
namespace MauiApp1.ModelAPI
{
    // Dit zijn alle mogelijke bezorgstatussen
    public enum DeliveryStateEnum
    {
        // Het pakketje wacht om bezorgd te worden
        Pending = 1,
        
        // Het pakketje is onderweg naar de klant
        InTransit = 2,
        
        // Het pakketje is aangekomen bij de klant
        Delivered = 3,
        
        // De bezorging is gestopt (geannuleerd)
        Cancelled = 4
    }
}
