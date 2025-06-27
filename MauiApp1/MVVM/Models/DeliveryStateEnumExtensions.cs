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
