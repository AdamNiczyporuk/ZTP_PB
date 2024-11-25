using System;
using System.Collections.Generic;
using System.Numerics;

public interface IMessage
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
    void AddMessage(IMessage message); // Accept IMessage, not Message
    IMessage GetMessageById(int id);
    void DisplayAllMessageTitles();
}

public class MessageBox : IMessageBox
{
    private List<IMessage> messages = new List<IMessage>();
    private int nextId = 1;

    public void AddMessage(IMessage message) // Accept IMessage
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

    public virtual void AddMessage(IMessage message) // Accept IMessage
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

    public override void AddMessage(IMessage message) // Accept IMessage
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

public class BlockingGetMessageBoxDecorator : MessageBoxDecorator
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

    public override void AddMessage(IMessage message) // Accept IMessage
    {
        message.Content += $"\n(Wysłano: {currentTestDate:yyyy-MM-dd})";
        currentTestDate = currentTestDate.AddDays(1); // Zwiększenie daty o jeden dzień
        base.AddMessage(message);
    }
}

public class ReadStatusMessageDecorator : IMessage
{
    private readonly IMessage originalMessage;
    private bool isRead;

    public ReadStatusMessageDecorator(IMessage message)
    {
        originalMessage = message;
        isRead = false; // Domyślnie wiadomość jest oznaczona jako "Nowa"
    }

    public int Id
    {
        get => originalMessage.Id;
        set => originalMessage.Id = value;
    }

    public string Title
    {
        get => $"{(isRead ? "[Odczytana]" : "[Nowa]")} {originalMessage.Title}";
        set => originalMessage.Title = value;
    }

    public string Content
    {
        get
        {
            MarkAsRead();
            return originalMessage.Content;
        }
        set => originalMessage.Content = value;
    }

    private void MarkAsRead()
    {
        if (!isRead)
        {
            isRead = true; // Oznaczenie wiadomości jako odczytanej
        }
    }
}

public class AddingReadStatusMessageBoxDecorator : MessageBoxDecorator
{
    public AddingReadStatusMessageBoxDecorator(IMessageBox messageBox) : base(messageBox)
    {
    }

    public override void AddMessage(IMessage message) // Accept IMessage
    {
        // Wrap the message with ReadStatusMessageDecorator
        var decoratedMessage = new ReadStatusMessageDecorator(message);

        // Now pass the decoratedMessage, which is of type IMessage, to base.AddMessage
        base.AddMessage(decoratedMessage);  // No need to cast to Message
    }

    public override IMessage GetMessageById(int id)
    {
        var message = base.GetMessageById(id);
        if (message is ReadStatusMessageDecorator readMessage)
        {
            // Access the content (this triggers the read status)
            readMessage.Content = readMessage.Content; // Use getter to trigger marking as read
        }
        return message;
    }
}


class Program
{
    static void Main(string[] args)
    {
        
        IMessageBox baseMessageBox = new MessageBox();

        
        DateTime startDate = DateTime.Now.AddDays(-7);

        IMessageBox readStatusMessageBox = new AddingReadStatusMessageBoxDecorator(baseMessageBox);


        IMessageBox dateAddingMessageBox = new AddingDateMessageBoxDecorator(readStatusMessageBox, startDate);

    
        IMessageBox blockingMessageBox = new BlockingGetMessageBoxDecorator(dateAddingMessageBox, "zasady");


        

        IMessageBox blockingAddMessageBox = new BlockingAddMessageBoxDecorator(blockingMessageBox, "zasady");


        blockingAddMessageBox.AddMessage(new Message("Powiadomienie o Kawie", "Spotkanie kawowe  odbędzie się w piątek o godzinie 10:00."));
        blockingAddMessageBox.AddMessage(new Message("Powiadomienie o spotkaniu", "Spotkanie zespołu odbędzie się w piątek o godzinie 10:00."));
        blockingAddMessageBox.AddMessage(new Message("Nowe zasady pracy zdalnej", "Od przyszłego miesiąca obowiązują nowe zasady pracy zdalnej."));
        blockingAddMessageBox.AddMessage(new Message("Wyniki kwartalne", "Wyniki finansowe za ostatni kwartał pokazują wzrost o 15%."));

        bool running = true;
        while (running)
        {
            // Wyświetlanie wszystkich tematów wiadomości
            blockingAddMessageBox.DisplayAllMessageTitles();

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
                    var message = blockingAddMessageBox.GetMessageById(id);
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
                Console.WriteLine("Wprowadź poprawny numer.");
            }
        }
    }
    
}
