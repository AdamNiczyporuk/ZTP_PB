using System;
using System.Collections.Generic;



public interface IOrderState
{
    void AddProduct(Order order, string product);
    void SubmitOrder(Order order);
    void ConfirmPayment(Order order);
    void PackProduct(Order order, string product);
    void ShipOrder(Order order);
    void CancelOrder(Order order);
    void ShowOrderDetails(Order order);
}
public class Order
{
    private IOrderState state;
    public Dictionary<string, bool> Products { get; } = new(); // Produkty: nazwa -> czy spakowany
    private bool isPaid = false; // Czy zamówienie zostało opłacone?

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
