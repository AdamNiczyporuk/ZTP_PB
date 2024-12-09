using State;
using System;
using System.Collections.Generic;


public class Order
{
    
    public Dictionary<string, bool> Products { get; } = new(); // Produkty: nazwa -> czy spakowany
    public bool isPaid = false; // Czy zamówienie zostało opłacone?
    private IOrderState state;

    public Order(IOrderState state)
    {
        this.state = state;
    }

    // Dodaje produkt do zamówienia
    public void AddProduct(string product)
    {
        if (isPaid)
        {
            Console.WriteLine("Nie można dodawać produktów do opłaconego zamówienia.");
            return;
        }
        Products.Add(product, false);
        Console.WriteLine($"Dodano produkt: {product}");
    }

    // Zatwierdza zamówienie
    public void SubmitOrder()
    {
        Console.WriteLine("Zamówienie zostało złożone i oczekuje na opłatę.");
    }

    // Oznacza zamówienie jako opłacone
    public void ConfirmPayment()
    {
        isPaid = true;
        Console.WriteLine("Płatność została potwierdzona.");
    }

    // Oznacza dany produkt jako spakowany
    public void PackProduct(string product)
    {
        Products[product] = true;
        Console.WriteLine($"Produkt {product} został spakowany.");
    }

    // Wysyła zamówienie
    public void ShipOrder()
    {
        Console.WriteLine("Zamówienie zostało wysłane.");
    }

    // Anuluje zamówienie
    public void CancelOrder()
    {
        if (isPaid)
        {
            Console.WriteLine("Środki zostały zwrócone klientowi.");
        }

        Console.WriteLine("Zamówienie zostało anulowane.");
        Products.Clear();
        isPaid = false;
    }

    // Wyświetla szczegóły zamówienia
    public void ShowOrderDetails()
    {
        Console.WriteLine($"Zamówienie [{(isPaid ? "Opłacone" : "Nieopłacone")}]:");
        foreach (var product in Products)
        {
            Console.WriteLine($" - {product.Key}: {(product.Value ? "Spakowany" : "Nie spakowany")}");
        }
    }

    public void SetState(IOrderState state)
    {
        this.state = state;
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Order order = new Order();

        // Dodawanie produktów
        order.AddProduct("Laptop");
        order.AddProduct("Myszka");
        order.ShowOrderDetails();

        // Złożenie zamówienia
        order.SubmitOrder();

        // Potwierdzenie płatności
        order.ConfirmPayment();

        // Spakowanie produktów
        order.PackProduct("Laptop");
        order.PackProduct("Myszka");
        order.ShowOrderDetails();

        // Wysłanie zamówienia
        order.ShipOrder();
    }
}
