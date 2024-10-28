using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;

// Klasa reprezentująca punkt
class Point
{
    // Współrzędne punktu
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

// Klasa reprezentująca figurę
class Figure
{
    // Punkty, które tworzą figurę
    public List<Point> Points { get; }

    public Figure(List<Point> points)
    {
        Points = points;
    }

    public override string ToString()
    {
        return $"Figura: {string.Join(", ", Points)}";
    }
}

// Klasa reprezentująca rysunek
class Drawing
{
    // Lista figur tworzących rysunek
    public List<Figure> Figures { get; }

    public Drawing(List<Figure> figures)
    {
        Figures = figures;
    }

    public override string ToString()
    {
        return $"Rysunek: \n{string.Join("\n", Figures)}";
    }
}

class DrawingBuilder
{
    private Figure currentFigure;
    private readonly List<Figure> figures = new List<Figure>();

    public DrawingBuilder MoveTo(int x, int y)
    {
        if (currentFigure != null)
        {
            figures.Add(currentFigure);
        }
        currentFigure = new Figure(new List<Point> { new Point(x, y) });
        return this;
    }
    public DrawingBuilder LineTo(int x, int y)
    {
        if(currentFigure ==null)
        {
            throw new InvalidOperationException("Musisz najpierw wywołać MoveTo, aby zdefiniować punkt początkowy figury.");
        }

        currentFigure.Points.Add(new Point(x, y));
        return this;
    }
    public DrawingBuilder Close()
    {
        if (currentFigure != null)
        {
            var firstPoint = currentFigure.Points[0];
            currentFigure.Points.Add(new Point(firstPoint.X,firstPoint.Y));
            figures.Add(currentFigure);
            currentFigure = null;

        }
        return this;
    }

    public Drawing Build()
    {
        if (currentFigure != null)
        {
            figures.Add(currentFigure);
        }
        
        return new Drawing(figures);
    }
}
class Director
{
    private readonly DrawingBuilder builder;
    public Director(DrawingBuilder builder)
    {
        this.builder = builder;
    }
    public void ConstructFromString(string instructions)
    {
        var commands = instructions.Split(' ',StringSplitOptions.RemoveEmptyEntries);

        for(int i =0; i< commands.Length;i++ )
        {
            string command = commands[i];
            switch (command)
            {
                case "M":
                    if(i+2 < commands.Length && int.TryParse(commands[i+1],out int x) && int.TryParse(commands[i+2], out int y))
                    {
                        builder.MoveTo(x,y);
                        i += 2;
                    }
                break;
                case "L":
                    if (i + 2 < commands.Length && int.TryParse(commands[i + 1], out int lineX) && int.TryParse(commands[i + 2], out int lineY))
                    {
                        builder.LineTo(lineX, lineY);
                        i += 2; // Przesuwamy indeks o dwa do przodu
                    }
                    break;

                case "Z":
                    builder.Close();
                    break;
            }   
        }
    }
}
class DrawingSaver
{
    public void SaveDrawing(Drawing drawing, string filePath)
    {
        // Ustawiamy rozmiar obrazu
        int width = 500;
        int height = 500;

        // Tworzymy obiekt Bitmap (obraz) o rozmiarze 500x500 pikseli
        using (Bitmap bitmap = new Bitmap(width, height))
        {
            // Tworzymy obiekt Graphics do rysowania na obrazie
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Wypełniamy tło na biało
                g.Clear(Color.White);

                // Ustawiamy czarne pióro o grubości 2 pikseli
                Pen blackPen = new Pen(Color.Black, 2);

                // Rysujemy wszystkie figury z rysunku
                foreach (var figure in drawing.Figures)
                {
                    if (figure.Points.Count > 0)
                    {
                        Point startPoint = figure.Points[0];
                        for (int i = 1; i < figure.Points.Count; i++)
                        {
                            Point endPoint = figure.Points[i];
                            g.DrawLine(blackPen, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                            startPoint = endPoint;
                        }
                        // Zamykanie figury, jeśli ma więcej niż 1 punkt
                        if (figure.Points.Count > 2)
                        {
                            g.DrawLine(blackPen, figure.Points[^1].X, figure.Points[^1].Y, figure.Points[0].X, figure.Points[0].Y);
                        }
                    }
                }
            }

            // Zapisujemy obraz jako plik PNG
            bitmap.Save(filePath, ImageFormat.Png);
            Console.WriteLine($"Rysunek został zapisany jako {filePath}");
        }
    }
}
class Program
{
    static void Main(string[] args)
    {

        var drawingBuilder = new DrawingBuilder();


        drawingBuilder
            .MoveTo(100, 400)
            .LineTo(200, 50)
            .LineTo(450, 300)
            .LineTo(250, 250)
            .Close();


        drawingBuilder
            .MoveTo(300, 350)
            .LineTo(350, 100)
            .LineTo(50, 200);


        //Drawing drawing = drawingBuilder.Build();


        //Console.WriteLine(drawing);


        //var drawingBuilder = new DrawingBuilder();

        //// Przykład użycia z Directorem
        //Director director = new Director(drawingBuilder);
        //director.ConstructFromString("M 100 400 L 200 50 L 450 300 L 250 250 Z M 300 350 L 350 100 L 50 200");

        //// Odbieramy gotowy rysunek
        //Drawing drawing = drawingBuilder.Build();

        //// Wypisujemy rysunek do konsoli
        //Console.WriteLine(drawing);

        //var drawingBuilder = new DrawingBuilder();

        //// Przykład użycia z Directorem
        //Director director = new Director(drawingBuilder);
        //director.ConstructFromString("M 100 400 L 200 50 L 450 300 L 250 250 Z M 300 350 L 350 100 L 50 200");

        // Odbieramy gotowy rysunek
        Drawing drawing = drawingBuilder.Build();

        // Wypisujemy rysunek do konsoli
        Console.WriteLine(drawing);

        // Tworzymy instancję DrawingSaver i zapisujemy rysunek
        DrawingSaver saver = new DrawingSaver();
        saver.SaveDrawing(drawing, "D:\\AAAAAAAANAUKA\\AAStudia\\SEMESTR5\\ZTP_PB\\PS4\\Builder\\Drawing.png");
    }

}
