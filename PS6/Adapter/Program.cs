using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

public interface ITableDataSource
{
    int GetRowCount(); // Liczba wierszy w tabeli
    int GetColumnCount(); // Liczba kolumn w tabeli
    string GetColumnName(int columnIndex); // Nazwa kolumny (np. "Name", "Age")
    string GetCellData(int rowIndex, int columnIndex); // Dane w komórce (wiersz, kolumna)
}

public class TableService
{
    public void DisplayTable(ITableDataSource dataSource)
    {
        // Wyświetlanie nagłówków kolumn
        for (int col = 0; col < dataSource.GetColumnCount(); col++)
        {
            Console.Write(dataSource.GetColumnName(col).PadRight(15));
        }
        Console.WriteLine();

        // Linie oddzielające nagłówki od danych
        Console.WriteLine(new string('-', dataSource.GetColumnCount() * 16));


        // Wyświetlanie wierszy danych
        for (int row = 0; row < dataSource.GetRowCount(); row++)
        {
            for (int col = 0; col < dataSource.GetColumnCount(); col++)
            {
                Console.Write(dataSource.GetCellData(row, col).PadRight(15));
            }
            Console.WriteLine();
        }
    }
}

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Status { get; set; }

    public User(string name, int age, string status)
    {
        Name = name;
        Age = age;
        Status = status;
    }
}

public class ArrayAdapter : ITableDataSource
{
    private readonly int[] _array;

    public ArrayAdapter(int[] array)
    {
        _array = array;
    }
    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (columnIndex == 0)
        {
            return rowIndex.ToString();
        }
        else if (columnIndex == 1)
        {
            return _array[rowIndex].ToString();
        }
        return "Brak";
    }

    public int GetColumnCount()
    {
        return 2;
    }

    public string GetColumnName(int columnIndex)
    {
        if (columnIndex == 0)
        {
            return "Index";
        }
        else if (columnIndex == 1)
        {
            return "Value";
        }
        return columnIndex.ToString();



    }

    public int GetRowCount()
    {
        return _array.Length;
    }
}
public class GenericArrayAdapter<T> : ITableDataSource
{
    private readonly T[] _array;

    public GenericArrayAdapter(T[] array)
    {
        _array = array;
    }

    public int GetRowCount()
    {
        return _array.Length;
    }

    public int GetColumnCount()
    {
        return 2;
    }

    public string GetColumnName(int columnIndex)
    {
        return columnIndex == 0 ? "Index" : "Value";
    }

    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (columnIndex == 0)
        {
            return rowIndex.ToString();
        }
        else if (columnIndex == 1)
        {
            return _array[rowIndex]?.ToString() ?? "null";
        }
        return "Invalid Column";
    }
}
public class DictionaryAdapter : ITableDataSource
{
    public readonly Dictionary<string, int> _dictionary;
    private List<string> _keys;
    public DictionaryAdapter(Dictionary<string, int> dictionary)
    {
        _dictionary = dictionary;
        _keys = new List<string>(_dictionary.Keys);
    }

    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _keys.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Invalid row index.");
        }

        var key = _keys[rowIndex];

        if (columnIndex == 0)
        {
            return key;
        }
        else
        {
            return _dictionary[key].ToString();
        }
    }

    public int GetColumnCount()
    {
        return 2;
    }

        public string GetColumnName(int columnIndex)
        {
            if (columnIndex == 0)
            {
                return "Key";
            }
            else if (columnIndex == 1)
            {
                return "Value";
            }
            throw new Exception("Invalid Colum name");
            
        
        }

    public int GetRowCount()
    {
        return _dictionary.Count;
    }
}
public class GenericDictionaryAdapter<TKey, TValue> : ITableDataSource
{
    private readonly Dictionary<TKey, TValue> _dictionary;
    private readonly List<TKey> _keys;

    public GenericDictionaryAdapter(Dictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
        _keys = new List<TKey>(_dictionary.Keys);
    }

    public int GetRowCount()
    {
        return _dictionary.Count;
    }

    public int GetColumnCount()
    {
        return 2;
    }

    public string GetColumnName(int columnIndex)
    {
        return columnIndex == 0 ? "Key" : "Value";
    }

    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _keys.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Invalid row index.");
        }

        TKey key = _keys[rowIndex];

        if (columnIndex == 0)
        {
            return key?.ToString() ?? "null";
        }
        else if (columnIndex == 1)
        {
            return _dictionary[key]?.ToString() ?? "null";
        }
        return "Invalid Column";
    }
}

public class GenericListAdapter<T> : ITableDataSource
{
    private readonly List<T> _list;
    private readonly PropertyInfo[] _properties;

    public GenericListAdapter(List<T> list)
    {
        _list = list;
        _properties = typeof(T).GetProperties();
    }

    public int GetRowCount()
    {
        return _list.Count;
    }

    public int GetColumnCount()
    {
        return _properties.Length;
    }

    public string GetColumnName(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= _properties.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), "Invalid column index.");
        }

        return _properties[columnIndex].Name;
    }

    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Invalid row index.");
        }

        T item = _list[rowIndex];
        object value = _properties[columnIndex].GetValue(item);

        return value?.ToString() ?? "null";
    }
}
public class UserListAdapter : ITableDataSource
{
    private readonly List<User> _users;

    public UserListAdapter(List<User> users)
    {
        _users = users;
    }

    public string GetCellData(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _users.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Invalid row index.");
        }

        var user = _users[rowIndex];

        if (columnIndex == 0)
        {
            return user.Name;
        }
        else if (columnIndex == 1)
        {
            return user.Age.ToString();
        }
        else if (columnIndex == 2)
        {
            return user.Status;
        }
        throw new Exception("Invalid Data");
    }

    public int GetColumnCount()
    {
        return 3;
    }

    public string GetColumnName(int columnIndex)
    {
        if (columnIndex == 0)
        {
            return "Name";
        }
        else if (columnIndex == 1)
        {
            return "Age";
        }
        else if (columnIndex == 2)
        {
            return "Status";
        }
        throw new Exception("Invalid Colum name");
    }

    public int GetRowCount()
    {
        return _users.Count;
    }
}

public class Program
{
    public static void Main()
    {
        TableService tableService = new TableService();

        // Test adaptera dla tablicy liczb całkowitych
        int[] numbersArray = { 10, 20, 30, 40, 50 };
        ITableDataSource arrayAdapter = new GenericArrayAdapter<int>(numbersArray);
        Console.WriteLine("Generic Array Table:");
        tableService.DisplayTable(arrayAdapter);

        // Test adaptera dla słownika
        Dictionary<string, int> dictionary = new Dictionary<string, int>
        {
            { "One", 1 },
            { "Two", 2 },
            { "Three", 3 }
        };
        ITableDataSource dictionaryAdapter = new GenericDictionaryAdapter<string, int>(dictionary);
        Console.WriteLine("\nGeneric Dictionary Table:");
        tableService.DisplayTable(dictionaryAdapter);

        // Test adaptera dla listy użytkowników
        List<User> users = new List<User>
        {
            new User("Alice", 25, "Active"),
            new User("Bob", 30, "Inactive"),
            new User("Charlie", 35, "Active")
        };
        ITableDataSource userListAdapter = new GenericListAdapter<User>(users);
        Console.WriteLine("\nGeneric User List Table:");
        tableService.DisplayTable(userListAdapter);
    }
}

