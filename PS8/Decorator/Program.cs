using System;
using System.Collections.Generic;
using System.Numerics;


public  interface IMessage
{
    int Id { get; set; }
    string Title { get; set; }
    string Content { get; set; }


}
 public class Message : IMessage
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Message(string title, string content)
    {
        Title = title;
        Content = content;
    }
}


public interface IMessageBox
{
    void AddMessage(Message message);
    IMessage GetMessageById(int id);
    void DisplayAllMessageTitles();
   
}
public class MessageBox : IMessageBox
{
    private List<Message> messages = new List<Message>();
    private int nextId = 1;

    public void AddMessage(Message message)
    {
        message.Id = nextId++;
        messages.Add(message);
    }

    public IMessage GetMessageById(int id)
    {
        return messages.Find(m => m.Id == id);
    }

    public void DisplayAllMessageTitles()
    {
        Console.WriteLine("\nLista wiadomości:");
        if (messages.Count == 0)
        {
            Console.WriteLine("Brak wiadomości w skrzynce.");
        }
        else
        {
            foreach (var message in messages)
            {
                Console.WriteLine($"ID: {message.Id} - {message.Title}");
            }
        }
    }
}
public abstract class MessageBoxDecorator : IMessageBox
{
    protected readonly IMessageBox decoratedMessageBox;

    
    protected MessageBoxDecorator(IMessageBox messageBox)
    {
        decoratedMessageBox = messageBox;
    }
    
    public virtual void AddMessage(Message message)
    {
        
        decoratedMessageBox.AddMessage(message);
    }

 
    public virtual IMessage GetMessageById(int id)
    {
        return decoratedMessageBox.GetMessageById(id);
    }

    public virtual void DisplayAllMessageTitles()
    {
        decoratedMessageBox.DisplayAllMessageTitles();
    }

}
public class BlockingAddMessageBoxDecorator : MessageBoxDecorator
{
    private readonly string bannedWord;

    public BlockingAddMessageBoxDecorator(IMessageBox messageBox, string bannedWord) : base(messageBox)
    {
        this.bannedWord = bannedWord;
    }

    public override void AddMessage(Message message)
    {
        if (message.Content.Contains(bannedWord, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Wiadomość zawiera zakazane słowo: \"{bannedWord}\" i nie zostanie dodana.");
        }
        else
        {
            base.AddMessage(message);
        }   
       
    }
}
public class HiddenMessage : IMessage
{
    public int Id { get; set; } = -1; // Specjalny ID dla ukrytych wiadomości
    public string Title { get; set; } = "Ukryta wiadomość";
    public string Content { get; set; } = "Ta wiadomość zawiera treści zakazane.";
}


public class BlockingGetMessageBoxDecorator: MessageBoxDecorator
{
    private readonly string bannedWord;
    private readonly HiddenMessage hiddenMessage = new HiddenMessage();

    public BlockingGetMessageBoxDecorator(IMessageBox messageBox, string bannedWord) : base(messageBox)
    {
        this.bannedWord = bannedWord;
    }

    public override IMessage GetMessageById(int id)
    {
        var message = base.GetMessageById(id);

        // Jeśli wiadomość zawiera zakazane słowo, zwracaj `HiddenMessage`
        if (message != null && message.Content.Contains(bannedWord))
        {
            return hiddenMessage;
        }

        return message;
    }




}
public class DateMessageDecorator : IMessage
{
    private readonly IMessage originalMessage;
    private readonly DateTime sentDate;

    public DateMessageDecorator(IMessage message)
    {
        originalMessage = message;
        sentDate = DateTime.Now;
    }

    public int Id
    {
        get => originalMessage.Id;
        set => originalMessage.Id = value;
    }

    public string Title
    {
        get => originalMessage.Title;
        set => originalMessage.Title = value;
    }

    public string Content
    {
        get => $"{originalMessage.Content}\n(Wysłano: {sentDate})";
        set => originalMessage.Content = value;
    }
}

public class AddingDateMessageBoxDecorator : MessageBoxDecorator
{
    private DateTime currentTestDate;

    public AddingDateMessageBoxDecorator(IMessageBox messageBox, DateTime startDate) : base(messageBox)
    {
        currentTestDate = startDate;
    }

    public override void AddMessage(Message message)
    {
        message.Content += $"\n(Wysłano: {currentTestDate:yyyy-MM-dd})";
        currentTestDate = currentTestDate.AddDays(1); // Zwiększenie daty o jeden dzień
        base.AddMessage(message);
    }
}
class Program
{
    static void Main(string[] args)
    {
        IMessageBox baseMessageBox = new MessageBox();

        // Data początkowa: 7 dni wstecz
        DateTime startDate = DateTime.Now.AddDays(-7);

        // Dekorator dodający daty
        IMessageBox dateAddingMessageBox = new AddingDateMessageBoxDecorator(baseMessageBox, startDate);

        // Dekorator blokujący dostęp do wiadomości zawierających zakazane słowo
        IMessageBox blockingMessageBox = new BlockingGetMessageBoxDecorator(dateAddingMessageBox, "zasady");

        // Dodanie przykładowych wiadomości
        blockingMessageBox.AddMessage(new Message("Powiadomienie o spotkaniu", "Spotkanie zespołu odbędzie się w piątek o godzinie 10:00."));
        blockingMessageBox.AddMessage(new Message("Nowe zasady pracy zdalnej", "Od przyszłego miesiąca obowiązują nowe zasady pracy zdalnej."));
        blockingMessageBox.AddMessage(new Message("Wyniki kwartalne", "Wyniki finansowe za ostatni kwartał pokazują wzrost o 15%."));

        bool running = true;
        while (running)
        {
            // Wyświetlanie wszystkich tematów wiadomości
            blockingMessageBox.DisplayAllMessageTitles();

            Console.WriteLine("\nWybierz ID wiadomości do wyświetlenia (lub 0, aby zakończyć): ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (id == 0)
                {
                    running = false;
                    Console.WriteLine("Koniec programu.");
                }
                else
                {
                    var message = blockingMessageBox.GetMessageById(id);
                    if (message != null)
                    {
                        Console.WriteLine($"\nTytuł: {message.Title}");
                        Console.WriteLine($"Treść: {message.Content}");
                    }
                    else
                    {
                        Console.WriteLine("Nie znaleziono wiadomości o podanym ID lub zawiera zakazane słowo.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór.");
            }
        }


    }
}