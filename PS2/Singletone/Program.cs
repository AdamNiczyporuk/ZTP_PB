using System;
using System.Linq;
using System.Runtime.CompilerServices;

interface IDatabaseConnection
{
    int AddRecord(string name, int age);

    void UpdateRecord(int id, string newName, int newAge);

    void DeleteRecord(int id);

    Record? GetRecord(int id);

    void ShowAllRecords();
}
interface IConnectionManager
{
    IDatabaseConnection GetConnection(string databaseName);
}

// Prosty rekord w bazie danych
class Record
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }

    public Record(int id, string name, int age)
    {
        Id = id;
        Name = name;
        Age = age;
    }

    public override string ToString()
    {
        return $"Record [ID={Id}, Name={Name}, Age={Age}]";
    }
}

// Prosta baza danych
class Database
{
    private static readonly Dictionary<string, Database> instances = new();

    private readonly List<Record> records; // Lista przechowująca rekordy
    private int nextId = 1; // Licznik do generowania unikalnych ID

    private Database()
    {
        records = new List<Record>();
    }

    public static Database GetInstance(string key)
    {
        if (!instances.ContainsKey(key))
        {
            instances[key] = new Database();
        }
        return instances[key];
    }

    // Zwraca implementację interfejsu DatabaseConnection
    public IDatabaseConnection CreateConnection()
    {
        return new DatabaseConnection(this);
    }

    // Prywatna klasa wewnętrzna implementująca interfejs DatabaseConnection
    // W Javie korzystamy z cech klas wewnętrznych, w C# ta klasa musiałaby posiadać
    // referencję na obiekt klasy Database
    private class DatabaseConnection : IDatabaseConnection
    {
        private readonly Database db;

        public DatabaseConnection(Database database)
        {
            db = database;
        }

        // Dodawanie nowego rekordu
        public int AddRecord(string name, int age)
        {
            Record newRecord = new(db.nextId++, name, age);
            db.records.Add(newRecord);
            Console.WriteLine($"Inserted: {newRecord}");
            return db.nextId - 1; // zwracamy id dodanego rekordu
        }

        // Pobieranie rekordu po ID
        public Record? GetRecord(int id)
        {
            return db.records.Where(rec => rec.Id == id).FirstOrDefault();
        }

        // Aktualizowanie rekordu po ID
        public void UpdateRecord(int id, string newName, int newAge)
        {
            Record? optionalRecord = GetRecord(id);

            if (optionalRecord != null)
            {
                Record record = optionalRecord;
                record.Name = newName;
                record.Age = newAge;
                Console.WriteLine($"Updated: {record}");
            }
            else
            {
                Console.WriteLine($"Record with ID {id} not found.");
            }
        }

        // Usuwanie rekordu po ID
        public void DeleteRecord(int id)
        {
            Record? optionalRecord = GetRecord(id);

            if (optionalRecord != null)
            {
                db.records.Remove(optionalRecord);
                Console.WriteLine($"Deleted record with ID {id}");
            }
            else
            {
                Console.WriteLine($"Record with ID {id} not found.");
            }
        }

        // Wyświetlanie wszystkich rekordów
        public void ShowAllRecords()
        {
            if (db.records.Any())
            {
                Console.WriteLine("All records:");
                db.records.ForEach(r => Console.WriteLine(r));
            }
            else
            {
                Console.WriteLine("No records in the database.");
            }
        }
    }
   

}
class ConnectionManager : IConnectionManager
{
    private static ConnectionManager? instance;
    private const int MaxConnections = 3;
    private readonly Dictionary<string, int> connectionCounter;
    private readonly Dictionary<string, Queue<IDatabaseConnection>> connectionPools;


    private ConnectionManager() 
    { 
        connectionCounter = new Dictionary<string, int>();
        connectionPools = new Dictionary<string, Queue<IDatabaseConnection>>();
    }

    public static ConnectionManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ConnectionManager();
        }
        return instance;
    }
    public IDatabaseConnection GetConnection(string databaseName)
    {
        Database database = Database.GetInstance(databaseName);

        if(!connectionPools.ContainsKey(databaseName))
        {
            connectionPools[databaseName] = new Queue<IDatabaseConnection>();
            connectionCounter[databaseName] = 0;
        }

        if (connectionPools[databaseName].Count< MaxConnections)
        {
            var newConnection = database.CreateConnection();
            connectionPools[databaseName].Enqueue(newConnection);
            connectionCounter[databaseName]++;
            return newConnection;
        }
        var connection = connectionPools[databaseName].Dequeue();
        connectionPools[databaseName].Enqueue(connection);
        return connection;


       
    }

}

public class Ztp01
{
    public static void Main(string[] args)
    {
        // Pobranie instancji ConnectionManager
        ConnectionManager connectionManager = ConnectionManager.GetInstance();

        // Test dla pierwszej bazy danych - pobranie 4 połączeń
        IDatabaseConnection connection1 = connectionManager.GetConnection("Database1");
        IDatabaseConnection connection2 = connectionManager.GetConnection("Database1");
        IDatabaseConnection connection3 = connectionManager.GetConnection("Database1");
        IDatabaseConnection connection4 = connectionManager.GetConnection("Database1");  

        // Dodanie rekordów i wyświetlanie wyników z connection1 i connection2
        connection1.AddRecord("Franek", 19);
        connection1.ShowAllRecords();
        connection2.AddRecord("Krzysztof", 30);
        connection2.ShowAllRecords();
        connection3.AddRecord("Marek", 20);
        connection3.ShowAllRecords();
        connection4.AddRecord("Anna", 25);
        connection4.ShowAllRecords();

        if (ReferenceEquals(connection1, connection4))
        {
            Console.WriteLine("\n");
            Console.WriteLine("connection1 i connection4 to ten sam obiekt");
            Console.WriteLine("\n");
        }
        else
        {
            Console.WriteLine("connection1 i connection4 to różne obiekty");
        }
            //connection1.ShowAllRecords();
            //connection2.ShowAllRecords();

        //// Sprawdzenie, czy connection3 i connection4 również widzą te same dane (ponieważ korzystają z tego samego obiektu bazy)
        //connection3.ShowAllRecords();
        //connection4.ShowAllRecords();  // Powinno pokazać to samo, co connection1
            
        //Console.WriteLine("=== Testowanie drugiej bazy danych ===");

        //// Test dla drugiej bazy danych - inne połączenia
        //IDatabaseConnection connection5 = connectionManager.GetConnection("Database2");
        //IDatabaseConnection connection6 = connectionManager.GetConnection("Database2");

        //// Dodanie rekordów i wyświetlenie wyników dla drugiej bazy
        //connection5.AddRecord("Anna", 22);
        //connection6.AddRecord("Marek", 35);
        //connection5.ShowAllRecords();
        //connection6.ShowAllRecords();  



    }
}