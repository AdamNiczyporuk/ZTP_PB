using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security;

public interface INewsService
{
    Response AddMessage(string title, string content);
    Response ReadMessage(int id);
    Response EditMessage(int id, string newContent);
    Response DeleteMessage(int id);
}

public class Response
{
    public string Status { get; set; }
    public string Message { get; set; }

    public Response(string status, string message)
    {
        Status = status;
        Message = message;
    }
}

public class User
{
    public string Name { get; set; }
    public UserRole Role { get; set; }

    public User(string name, UserRole role)
    {
        Name = name;
        Role = role;
    }
}

public enum UserRole
{
    Guest,
    User,
    Moderator,
    Admin
}
public class Message
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Message(int id, string title, string content)
    {
        Id = id;
        Title = title;
        Content = content;
    }
}

public class NewsService : INewsService
{
    private List<Message> _messages;
    private int _nextId;

    public NewsService()
    {    
        _messages = new List<Message>();
        _nextId = 1;
    }

    public Response AddMessage(string title, string content)
    {
        var message = new Message(_nextId++, title, content);
        _messages.Add(message);
        return new Response("Success", "Message added successfully.");
    }

    public Response ReadMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            return new Response("Success", $"{message.Title}: {message.Content}");
        }
        return new Response("Error", "Message not found.");
    }

    public Response EditMessage(int id, string newContent)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message == null)
        {
            return new Response("Error", "Message not found.");
        }

        message.Content = newContent;
        return new Response("Success", "Message edited successfully.");
    }

    public Response DeleteMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message == null)
        {
            return new Response("Error", "Message not found.");
        }

        _messages.Remove(message);
        return new Response("Success", "Message deleted successfully.");
    }
}
public class ProxyNewsService : INewsService
{
    private readonly INewsService _newsService;
    private readonly User _user;

    private readonly Dictionary<int,Response> _cache=  new  Dictionary<int,Response>();
    public ProxyNewsService(INewsService newsService, User user)
    {
        _newsService = newsService;
        _user = user;
    }


    public Response AddMessage(string title, string content)
    {
       if(_user.Role == UserRole.Admin || _user.Role == UserRole.User || _user.Role== UserRole.Moderator)
        {
            var respone = _newsService.AddMessage(title, content);
            ClearCache();
            return respone;
          
        }
       else
        {
            throw new UnauthorizedAccessException($"{_user.Role} doesnot have permission to delete documents.");
        }
    }

    public Response DeleteMessage(int id)
    {
        if (_user.Role == UserRole.Admin )
        {
            var response = _newsService.DeleteMessage(id);
            ClearCacheForId(id);
            return response;
        }
        else
        {
            throw new UnauthorizedAccessException($"{_user.Role} doesnot have permission to delete documents.");
        }
    }

    public Response EditMessage(int id, string newContent)
    {
        if(_user.Role == UserRole.Admin  || _user.Role== UserRole.Moderator)
        {
            var response = _newsService.EditMessage(id, newContent);
            ClearCacheForId(id);
            return response;
        }
        else
        {
            throw new UnauthorizedAccessException($"{_user.Role} doesnot have permission to delete documents.");
        }
    }

    public Response ReadMessage(int id)
    {
        if(_user.Role == UserRole.Admin|| _user.Role == UserRole.User || _user.Role == UserRole.Moderator ||_user.Role == UserRole.Guest)
        {
            if(_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            var response = _newsService.ReadMessage(id);
            _cache[id] = response;
            return response;
        }
        else
        {
            throw new UnauthorizedAccessException($"{_user.Role} doesnot have permission to delete documents.");
        }
    }
    private void ClearCacheForId(int id)
    {
        if (_cache.ContainsKey(id))
        {
            _cache.Remove(id);
        }
    }
    private void ClearCache()
    {
        _cache.Clear();
    }   
}

public class Program
{
    public static void Main()
    {
        //// Tworzymy instancję rzeczywistego serwisu
        //INewsService newsService = new NewsService();

        //// Dodawanie wiadomości
        //var addResponse1 = newsService.AddMessage("Breaking News", "New breakthrough in AI technology.");
        //Console.WriteLine($"{addResponse1.Status}: {addResponse1.Message}");

        //var addResponse2 = newsService.AddMessage("Market Update", "Stocks soar after positive earnings reports.");
        //Console.WriteLine($"{addResponse2.Status}: {addResponse2.Message}");

        //// Odczyt wiadomości
        //var readResponse1 = newsService.ReadMessage(1);
        //Console.WriteLine($"{readResponse1.Status}: {readResponse1.Message}");

        //var readResponse2 = newsService.ReadMessage(2);
        //Console.WriteLine($"{readResponse2.Status}: {readResponse2.Message}");

        //// Próbujemy odczytać wiadomość, która nie istnieje
        //var readResponse3 = newsService.ReadMessage(3);
        //Console.WriteLine($"{readResponse3.Status}: {readResponse3.Message}");

        //// Edycja wiadomości
        //var editResponse = newsService.EditMessage(1, "Updated content: AI technology is advancing rapidly.");
        //Console.WriteLine($"{editResponse.Status}: {editResponse.Message}");

        //// Odczyt wiadomości po edycji
        //var readResponseAfterEdit = newsService.ReadMessage(1);
        //Console.WriteLine($"{readResponseAfterEdit.Status}: {readResponseAfterEdit.Message}");

        //// Usunięcie wiadomości
        //var deleteResponse = newsService.DeleteMessage(2);
        //Console.WriteLine($"{deleteResponse.Status}: {deleteResponse.Message}");

        //// Próba odczytania usuniętej wiadomości
        //var readResponseAfterDelete = newsService.ReadMessage(2);
        //Console.WriteLine($"{readResponseAfterDelete.Status}: {readResponseAfterDelete.Message}");

        INewsService newsService = new NewsService();

        // Tworzymy użytkowników o różnych rolach
        var adminUser = new User("Admin User", UserRole.Admin);
        var moderatorUser = new User("Moderator User", UserRole.Moderator);
        var normalUser = new User("Normal User", UserRole.User);
        var guestUser = new User("Guest User", UserRole.Guest);

        // Tworzymy proxy dla admina
        var adminProxy = new ProxyNewsService(newsService, adminUser);

        // Przykład dodawania wiadomości przez admina
        var addResponse1 = adminProxy.AddMessage("Breaking News", "New breakthrough in AI technology.");
        Console.WriteLine($"{addResponse1.Status}: {addResponse1.Message}");

        // Tworzymy proxy dla moderatora
        var moderatorProxy = new ProxyNewsService(newsService, moderatorUser);

        // Moderator dodaje wiadomość
        var addResponse2 = moderatorProxy.AddMessage("Market Update", "Stocks soar after positive earnings reports.");
        Console.WriteLine($"{addResponse2.Status}: {addResponse2.Message}");

        // Czytanie wiadomości z cache (pierwsze odczyt z serwisu)
        var readResponse1 = adminProxy.ReadMessage(1);
        Console.WriteLine($"{readResponse1.Status}: {readResponse1.Message}");

        // Czytanie tej samej wiadomości (z cache)
        var readResponse2 = adminProxy.ReadMessage(1);
        Console.WriteLine($"{readResponse2.Status}: {readResponse2.Message}");

        // Moderator edytuje wiadomość
        var editResponse = moderatorProxy.EditMessage(1, "Updated content: AI technology is advancing rapidly.");
        Console.WriteLine($"{editResponse.Status}: {editResponse.Message}");

        // Czytanie wiadomości po edycji
        var readResponseAfterEdit = moderatorProxy.ReadMessage(1);
        Console.WriteLine($"{readResponseAfterEdit.Status}: {readResponseAfterEdit.Message}");

        // Gość próbuje odczytać wiadomość
        var guestProxy = new ProxyNewsService(newsService, guestUser);
        var guestReadResponse = guestProxy.ReadMessage(1);
        Console.WriteLine($"{guestReadResponse.Status}: {guestReadResponse.Message}");

        //var guestEditResponse = guestProxy.EditMessage(1, "Guest trying to edit message.");
        //Console.WriteLine($"{guestEditResponse.Status}: {guestEditResponse.Message}");

        // Admin usuwa wiadomość
        var deleteResponse = adminProxy.DeleteMessage(1);
        Console.WriteLine($"{deleteResponse.Status}: {deleteResponse.Message}");

        // Próba odczytania usuniętej wiadomości
        var readResponseAfterDelete = adminProxy.ReadMessage(1);
        Console.WriteLine($"{readResponseAfterDelete.Status}: {readResponseAfterDelete.Message}");
    }
}
