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