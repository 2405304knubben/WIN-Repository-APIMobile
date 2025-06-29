/// <summary>
/// Dit helpt ons om leuke berichtjes te laten zien over waar je pakketje is!
/// </summary>
/// <remarks>
/// Dit is een speciaal hulpje dat moeilijke computer-woorden omzet naar leuke Nederlandse zinnetjes,
/// zodat iedereen kan begrijpen wat er met hun pakketje gebeurt!
/// </remarks>
/// <param name="state">Dit vertelt ons waar het pakketje nu is</param>
/// <returns>
/// Een lief Nederlands berichtje dat vertelt wat er met het pakketje gebeurt:
/// - Als het wacht: "Wachtend op bezorging"
/// - Als het onderweg is: "Order onderweg"
/// - Als het aangekomen is: "Order bezorgd"
/// - Als het niet meer komt: "Order geannuleerd"
/// - Als we het niet weten: "Onbekend"
/// </returns>
namespace MauiApp1.ModelAPI
{
    public static class DeliveryStateEnumExtensions
    {
        public static string GetDisplayText(this DeliveryStateEnum state)
        {
            return state switch
            {
                DeliveryStateEnum.Pending => "Wachtend op bezorging",
                DeliveryStateEnum.InTransit => "Order onderweg",
                DeliveryStateEnum.Delivered => "Order bezorgd",
                DeliveryStateEnum.Cancelled => "Order geannuleerd",
                _ => "Onbekend"
            };
        }
    }
}
