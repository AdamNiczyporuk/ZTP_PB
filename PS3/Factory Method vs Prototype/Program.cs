using System;
using System.Collections.Generic;
using System.Text;

// Klasa reprezentująca komórkę tabeli

abstract class Cell
{
    protected object value;

    public abstract Cell Clone();
    public abstract void SetValue(object value);

    public override string ToString()
    {
        return value.ToString().PadRight(15);
    }
}
class TextCell : Cell
{
    public TextCell(string value = "")
    {
        this.value = value;
    }
    public override Cell Clone()
    {
        return new TextCell();
    }
    public override void SetValue(object value)
    {
        this.value = value.ToString();
    }
}
class NumberCell : Cell
{
    public NumberCell(int value = 0)
    {
        this.value = value;
    }
    public override Cell Clone()
    {
        return new  NumberCell();
    }
    public override void SetValue(object value)
    {
        this.value = Convert.ToInt32(value);
    }
}
class BooleanCell : Cell
{
    public BooleanCell(bool value = false)
    {
        this.value = value;
    }
    public override Cell Clone()
    {
        return new BooleanCell();
    }
    public override void SetValue(object value)
    {
        this.value = Convert.ToBoolean(value);
    }

}

// Klasa reprezentująca nagłówek kolumny w tabeli
//class Header
//{
//    public string Name { get; }

//    public Header(string name)
//    {
//        Name = name;
//    }
//}

class Header
{
    public string Name { get; }
    private readonly Cell prototypeCell;

    public Header(string name, Cell prototypeCell)
    {
        Name = name;
        this.prototypeCell = prototypeCell;
    }

    public Cell CreateCell(object value)
    {
        Cell newCell = prototypeCell.Clone(); 
        newCell.SetValue(value); 
        return newCell;
    }

    public Cell CreateDefaultCell()
    {
        return prototypeCell.Clone();
    }
}
//class TextHeader : Header
//{
//    public TextHeader(string name) : base(name) { }

//    public override Cell CreateCell(object value)
//    {
//        return new TextCell(value.ToString());
//    }

//    public override Cell CreateDefaultCell()
//    {
//        return new TextCell("");
//    }
//}
//class NumberHeader : Header
//{
//    public NumberHeader(string name) : base(name) { }

//    public override Cell CreateCell(object value)
//    {
//        if (int.TryParse(value.ToString(), out int number))
//        {
//            return new NumberCell(number);
//        }
//        throw new ArgumentException("Invalid number format.");
//    }

//    public override Cell CreateDefaultCell()
//    {
//        return new NumberCell(0);
//    }
//}

//class BooleanHeader : Header
//{
//    public BooleanHeader(string name) : base(name) { }
//    public override Cell CreateCell(object value)
//    {
//        if(bool.TryParse(value.ToString(), out bool boolean))
//        {
//            return new BooleanCell(boolean);
//        }
//        throw new ArgumentException("Invalid boolean format.");
//    }
//    public override Cell CreateDefaultCell()
//    {
//        return new BooleanCell(false);
//    }
//}


// Klasa reprezentująca tabelę
class Table
{
    private readonly List<Header> headers;  // Lista nagłówków kolumn
    private readonly List<List<Cell>> rows; // Lista wierszy (każdy wiersz to lista komórek)

    public Table()
    {
        headers = new List<Header>();
        rows = new List<List<Cell>>();
    }

    public void AddColumn(Header header)
    {
        headers.Add(header);

        // Dodajemy domyślne komórki do każdego z istniejących wierszy
        foreach (var row in rows)
        {
            row.Add(header.CreateDefaultCell());
        }
    }

    public void AddRow(params object[] cellValues)
    {
        if (cellValues.Length != headers.Count)
        {
            throw new ArgumentException("Liczba wartości nie zgadza się z liczbą kolumn.");
        }

        var newRow = new List<Cell>();
        for (int i = 0; i < cellValues.Length; i++)
        {
            newRow.Add(headers[i].CreateCell(cellValues[i]));
        }

        rows.Add(newRow);

    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        // Dodajemy nagłówki
        foreach (var header in headers)
        {
            sb.Append(header.Name.PadRight(15));
        }
        sb.AppendLine();

        // Dodajemy separator
        sb.AppendLine(new string('-', headers.Count * 15));

        // Dodajemy wiersze
        foreach (var row in rows)
        {
            foreach (var cell in row)
            {
                sb.Append(cell.ToString());
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Tworzymy nową tabelę
        Table table = new Table();

        // Dodajemy kolumny
        table.AddColumn(new Header("Name", new TextCell()));
        table.AddColumn(new Header("Age", new NumberCell()));
        table.AddColumn(new Header("Is Student", new BooleanCell()));

        // Dodajemy wiersze
        table.AddRow("Alice", 30, false);
        table.AddRow("Bob", 25, true);
        table.AddRow("Charlie", 35, false);

        // Wyświetlamy tabelę
        Console.WriteLine(table.ToString());

        Console.ReadKey();
    }
}
