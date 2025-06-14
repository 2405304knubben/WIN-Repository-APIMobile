using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.NewFolder
{
    // Models/Customer.cs
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public List<Order> Orders { get; set; }
    }

    // Models/Order.cs
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<Product> Products { get; set; }
        public List<DeliveryState> DeliveryStates { get; set; }
    }

    // Models/Product.cs
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<Order> Orders { get; set; }
        public List<Part> Parts { get; set; }
    }

    // Models/DeliveryState.cs
    public class DeliveryState
    {
        public int Id { get; set; }
        public DeliveryStateEnum State { get; set; }
        public DateTime DateTime { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int DeliveryServiceId { get; set; }
        public DeliveryService DeliveryService { get; set; }
    }

    public enum DeliveryStateEnum
    {
        // Vul deze in met de juiste enumwaarden van de API
    }

    // Models/DeliveryService.cs
    public class DeliveryService
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Models/Part.cs
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
    }

}
