using System;
using System.Collections.Generic;

public class Message
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

public class MessageBox
{
    private List<Message> messages = new List<Message>();
    private int nextId = 1;

    public void AddMessage(Message message)
    {
        message.Id = nextId++;
        messages.Add(message);
    }

    public Message GetMessageById(int id)
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