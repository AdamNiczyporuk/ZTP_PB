using System;
using System.Collections.Generic;
using System.Linq;

public class Task
{
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;

    // Konstruktor klasy Task, ustawiający nazwę oraz daty początku i końca zadania
    public Task(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    // Metoda oznaczająca zadanie jako wykonane; przyjmuje datę wykonania i sprawdza, czy zadanie wykonano na czas
    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    // Zwraca status zadania: "Completed", "Completed Late" lub "Pending"
    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }

    // Używana do wyświetlenia szczegółów zadania wraz ze statusem
    public override string ToString()
    {
        return $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}";
    }
}

public class Program
{
    public static void Main()
    {
        // Przykładowe zadania
        var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));
        var task3 = new Task("2A - Projektowanie schematu bazy danych", new DateTime(2024, 10, 28), new DateTime(2024, 11, 3));
        var task4 = new Task("2B - Tworzenie zapytań SQL", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

        // Oznaczanie przykładowych zadań jako wykonane (z różnymi datami ukończenia)
        task1.MarkAsCompleted(new DateTime(2024, 10, 25)); // Wykonane na czas
        task2.MarkAsCompleted(new DateTime(2024, 11, 1)); // Wykonane z opóźnieniem
        // task3 i task4 są jeszcze niewykonane

        // Lista zadań (przykładowa organizacja wyłącznie według nazw)
        var tasks = new List<Task> { task1, task2, task3, task4 };

        // Wyświetlanie listy zadań i ich statusów
        Console.WriteLine("Lista zadań:");
        foreach (var task in tasks)
        {
            Console.WriteLine(task);
        }

        // Zliczanie wykonanych, opóźnionych i oczekujących zadań
        int completedOnTime = tasks.Count(t => t.IsCompleted && !t.IsLate);
        int completedLate = tasks.Count(t => t.IsCompleted && t.IsLate);
        int pending = tasks.Count(t => !t.IsCompleted);
        int pendingLate = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        Console.WriteLine("\nPodsumowanie zadań:");
        Console.WriteLine($"Zadania wykonane na czas: {completedOnTime}");
        Console.WriteLine($"Zadania wykonane z opóźnieniem: {completedLate}");
        Console.WriteLine($"Zadania oczekujące: {pending}");
        Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {pendingLate}");
    }
}
