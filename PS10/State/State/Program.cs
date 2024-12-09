using State;
using System;
using System.Collections.Generic;


public class Order
{
    
    public Dictionary<string, bool> Products { get; } = new(); // Produkty: nazwa -> czy spakowany
    public  bool isPaid = false; // Czy zamówienie zostało opłacone?
    private IOrderState state;

    public Order(IOrderState state)
    {
        this.state = state;
    }

    // Dodaje produkt do zamówienia
    public void AddProduct(string product)
    {
       state.AddProduct(this, product);
    }

    // Zatwierdza zamówienie
    public void SubmitOrder()
    {
         state.SubmitOrder(this);
    }

    // Oznacza zamówienie jako opłacone
    public void ConfirmPayment()
    {
       state.ConfirmPayment(this);
    }

    // Oznacza dany produkt jako spakowany
    public void PackProduct(string product)
    {
       state.PackProduct(this, product);
    }

    // Wysyła zamówienie
    public void ShipOrder()
    {
        state.ShipOrder(this);
    }

    // Anuluje zamówienie
    public void CancelOrder()
    {
        state.CancelOrder(this);
    }

    // Wyświetla szczegóły zamówienia
    public void ShowOrderDetails()
    {
       state.ShowOrderDetails(this);
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
        Console.WriteLine("=== Testowanie aplikacji zamówień ===");

        // Utworzenie zamówienia w stanie CreatedState
        Order order = new Order(new CreatedState());

        Console.WriteLine("\n--- Testowanie CreatedState ---");
        order.AddProduct("Laptop");
        order.AddProduct("Myszka");
        order.AddProduct("Laptop"); // Próba dodania tego samego produktu
        order.SubmitOrder();
        order.ShowOrderDetails();

        Console.WriteLine("\n--- Testowanie SubmittedState ---");
        order.AddProduct("Klawiatura"); // Próba dodania produktu po złożeniu zamówienia
        order.SubmitOrder();
        order.ConfirmPayment();
        order.ShowOrderDetails();

        Console.WriteLine("\n--- Testowanie PaidState ---");
        order.AddProduct("Monitor"); // Próba dodania produktu po opłaceniu zamówienia
        order.PackProduct("Laptop");
        order.PackProduct("Myszka");
        order.PackProduct("Klawiatura"); // Próba pakowania produktu, który nie jest w zamówieniu
        order.ShowOrderDetails();

        Console.WriteLine("\n--- Testowanie ShippedState ---");
        order.ShipOrder();
        order.AddProduct("Kabel USB"); // Próba dodania produktu po wysyłce
        order.CancelOrder(); // Próba anulowania zamówienia po wysyłce
        order.ShowOrderDetails();
    }

 }
