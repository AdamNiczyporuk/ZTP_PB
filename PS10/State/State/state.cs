using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace State
{
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

    public class CreatedState : IOrderState
    {

        public void AddProduct(Order order, string product)
        {
            if (order.Products.ContainsKey(product))
            {
                Console.WriteLine($"Produkt {product} jest już w zamówieniu.");
                return;
            }
            if (order.isPaid)
            {
                Console.WriteLine("Nie można dodawać produktów do opłaconego zamówienia.");
                return;
            }
            order.Products.Add(product, false);
            Console.WriteLine($"Dodano produkt: {product}");
        }
        public void SubmitOrder(Order order)
        {
            if (order.Products.Count == 0)
            {
                Console.WriteLine("Nie można złożyć pustego zamówienia.");
                return;
            }
            Console.WriteLine("Zamówienie zostało złożone i oczekuje na opłatę.");
            order.SetState(new SubmittedState());
        }
        public void ConfirmPayment(Order order)
        {
            Console.WriteLine("Zamówienie nie zostało jeszcze złożone.");
        }
        public void PackProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie nie zostało jeszcze złożone.");
        }
        public void ShipOrder(Order order)
        {
            Console.WriteLine("Zamówienie nie zostało jeszcze złożone.");
        }
        public void CancelOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
            order.SetState(new CanceledState());
        }
        public void ShowOrderDetails(Order order)
        {
            Console.WriteLine("Zamówienie jest w trakcie składania.");
        }
    }

    public class SubmittedState : IOrderState
    {
        public void AddProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie zostało już złożone.");
        }
        public void SubmitOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało już złożone.");
        }
        public void ConfirmPayment(Order order)
        {
            order.isPaid = true;
            Console.WriteLine("Płatność została potwierdzona.");
            order.SetState(new PaidState());
        }
        public void PackProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie nie zostało jeszcze opłacone.");
        }
        public void ShipOrder(Order order)
        {
            Console.WriteLine("Zamówienie nie zostało jeszcze opłacone.");
        }
        public void CancelOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
            order.Products.Clear();
            order.SetState(new CanceledState());
        }
        public void ShowOrderDetails(Order order)
        {
            Console.WriteLine("Zamówienie złożone oczekuje na opłatę.");
        }
    }
    public class PaidState : IOrderState
    {
        public void AddProduct(Order order,string product)
        {
            Console.WriteLine("Zamówienie zostało opłacone nie można dodać nowego produktu");
        }
        public void SubmitOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało opłacone");
        }
        public void ConfirmPayment(Order order)
        {
            Console.WriteLine("Zamówienie zostało opłacone");
        }
        public void PackProduct(Order order, string product)
        {
            if (!order.Products.ContainsKey(product))
            {
                Console.WriteLine($"Produkt {product} nie znajduje się w zamówieniu.");
                return;
            }
            if (order.Products[product])
            {
                Console.WriteLine($"Produkt {product} jest już spakowany.");
                return;
            }
            order.Products[product] = true;
            Console.WriteLine($"Produkt {product} został spakowany.");
        }
        public void ShipOrder(Order order)
        {
            if (order.Products.Values.Any(packed => !packed))
            {
                Console.WriteLine("Nie wszystkie produkty zostały spakowane. Nie można wysłać zamówienia.");
                return;
            }
            Console.WriteLine("Zamówienie zostało wysłane.");
            order.SetState(new ShippedState());
        }
        public void CancelOrder(Order order)
        {
            if (order.isPaid)
            {
                Console.WriteLine("Środki zostały zwrócone klientowi.");
            }
            Console.WriteLine("Zamówienie zostało anulowane.");
            order.Products.Clear();
            order.isPaid = false;
            order.SetState(new CreatedState());
        }
        public void ShowOrderDetails(Order order)
        {
            Console.WriteLine("Zamówienie opłacone oczekuje na spakowanie.");
        }
    }
    public class ShippedState : IOrderState
    {
        public void AddProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie zostało już wysłane.Nie można dodać nowego!");
        }
        public void SubmitOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało wysłane.");
        }
        public void ConfirmPayment(Order order)
        {
            Console.WriteLine("Zamówienie zostało wysłane.");
        }
        public void PackProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie zostało wysłane.");
        }
        public void ShipOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało wysłane.");
        }
        public void CancelOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało wysłane.NIe można go anulować");
        }
        public void ShowOrderDetails(Order order)
        {
            Console.WriteLine("Zamówienie zostało wysłane.");
        }


    }

    public class CanceledState :IOrderState
    {
        public void AddProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }   
        public void SubmitOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }
        public void ConfirmPayment(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }
        public void PackProduct(Order order, string product)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }
        public void ShipOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }
        public void CancelOrder(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }
            public void ShowOrderDetails(Order order)
        {
            Console.WriteLine("Zamówienie zostało anulowane.");
        }

    }



}
