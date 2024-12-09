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
        Order order = new Order(new CreatedState());


        order.AddProduct("Laptop");
        order.AddProduct("Myszka");
        order.ShowOrderDetails();


        order.SubmitOrder();
        order.ShowOrderDetails();


        order.ConfirmPayment();
        order.ShowOrderDetails();


        order.PackProduct("Laptop");
        order.PackProduct("Myszka");
        order.ShowOrderDetails();

        order.ShipOrder();
        order.ShowOrderDetails();

        order.CancelOrder();
        order.ShowOrderDetails();
    }
}
