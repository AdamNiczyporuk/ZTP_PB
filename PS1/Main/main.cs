using System;
using System.Collections.Generic;

class Circle
{
    private double radius;

    public Circle(double radius)
    {
        this.radius = radius;
    }

    public double CalculatePerimeter()
    {
        return 2 * Math.PI * radius;
    }

    public double CalculateArea()
    {
        return Math.PI * radius * radius;
    }
}

class Rectangle
{
    private double width, height;

    public Rectangle(double width, double height)
    {
        this.width = width;
        this.height = height;
    }

    public double Perimeter()
    {
        return 2 * (width + height);
    }

    public double Area()
    {
        return width * height;
    }
}

class Triangle
{
    private double a, b, c;

    public Triangle(double a, double b, double c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public double GetPerimeter()
    {
        return a + b + c;
    }

    public double GetArea()
    {
        double s = GetPerimeter() / 2;
        return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
    }
}

partial class Program
{
    static void Main()
    {
        List<object> figures = new List<object>();
        int choice;
        do
        {
            Console.WriteLine("Choose a figure to add (1 - Circle, 2 - Rectangle, 3 - Triangle, 0 - Exit): ");
            choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 1)
            {
                double radius = Convert.ToDouble(Console.ReadLine());
                figures.Add(new Circle(radius));
            }
            else if (choice == 2)
            {
                double width = Convert.ToDouble(Console.ReadLine());
                double height = Convert.ToDouble(Console.ReadLine());
                figures.Add(new Rectangle(width, height));
            }
            else if (choice == 3)
            {
                double a = Convert.ToDouble(Console.ReadLine());
                double b = Convert.ToDouble(Console.ReadLine());
                double c = Convert.ToDouble(Console.ReadLine());
                figures.Add(new Triangle(a, b, c));
            }
        } while (choice != 0);

        Console.WriteLine("\nFigures in the collection:");
        foreach (object figure in figures)
        {
            if (figure is Circle circle)
                Console.WriteLine("Circle " +"Perimeter: " + circle?.CalculatePerimeter() + ", Area: " + circle?.CalculateArea());
            else if (figure is Rectangle rectangle)
                Console.WriteLine("Rectangle " + "Perimeter: " + rectangle?.Perimeter() + ", Area: " + rectangle?.Area());
            else if (figure is Triangle triangle)
                Console.WriteLine("Triangle " + "Perimeter: " + triangle?.GetPerimeter() + ", Area: " + triangle?.GetArea());
        }
    }
}
