namespace MauiApp1.ModelAPI
{
    // Dit is een onderdeel, zoals een wiel van een auto of een knop van een machine
    public class Part
    {
        // Elk onderdeel heeft een nummer, zodat we weten welk onderdeel het is
        public int Id { get; set; }
        
        // De naam van het onderdeel, zoals "Wiel" of "Motor"
        public string Name { get; set; }
        
        // Een uitleg over wat dit onderdeel doet, zoals "Dit wiel draait rond"
        public string Description { get; set; }
        
        // Een lijst van alle dingen die je kunt maken met dit onderdeel
        public List<Product> Products { get; set; }
    }
}
