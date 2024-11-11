using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

public interface ITaskComponent
{
    bool IsCompleted { get; }
     DateTime StartDate { get; }
     DateTime EndDate { get; }

    void MarkAsCompleted (DateTime completionDate);
    void Display(int depth = 0);

}

public class Task : ITaskComponent
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
    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}- {Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
    }

}
public class TaskGroup : ITaskComponent
{
    public string Name { get; }
    protected List<ITaskComponent> _tasks = new List<ITaskComponent>();

    public TaskGroup(string name)
    {
        Name = name;
    }

    public void Add(ITaskComponent task)
    {
        _tasks.Add(task);
    }

    public void Remove(ITaskComponent task)
    {
        _tasks.Remove(task);
    }
    public virtual bool IsCompleted
    {
        get
        {
            return _tasks.All(t => t.IsCompleted);
        }
    }
    public DateTime StartDate
    {
        get
        {
            return _tasks.Min(t => t.StartDate);
        }
    }
    public DateTime EndDate
    {
        get
        {
            return _tasks.Max(t => t.EndDate);
        }
    }
    public virtual void MarkAsCompleted(DateTime completionDate)
    {
        foreach (var task in _tasks)
        {
            if (!task.IsCompleted)
            {
                task.MarkAsCompleted(completionDate);
            }
            
        }
    }
    public void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}+ {Name}");
        foreach (var task in _tasks)
        {
            task.Display(depth + 1);
        }
    }



}
public class OptionalTaskGroup: TaskGroup
{
    public OptionalTaskGroup(string name) : base(name) { }

    public override  bool IsCompleted
    {
        get
        {
            return _tasks.Any(t=> t.IsCompleted);
        }
    }
    public override void MarkAsCompleted(DateTime completionDate)
    {
        if(_tasks.Any(t=> !t.IsCompleted))
        {
            var taskComplete = _tasks.First(t => !t.IsCompleted);
            taskComplete.MarkAsCompleted(completionDate);
        }
    }
}
public class Program
{
    public static void Main()
    {
        int consoleWidth = Console.WindowWidth;
        string line = new string('-', consoleWidth - 1);
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
        Console.WriteLine(line);
        // Tworzenie grupy zadań
        var group1 = new TaskGroup("Grupa 1");
        group1.Add(task1);
        group1.Add(task2);

        var group2 = new TaskGroup("Grupa 2");
        group2.Add(task3);
        group2.Add(task4);

        // Tworzenie głównej grupy zadań
        var mainGroup = new TaskGroup("Projekt");
        mainGroup.Add(group1);
        mainGroup.Add(group2);

        // Wyświetlanie struktury hierarchicznej zadań
        Console.WriteLine("\nStruktura grupy zadań:");
        mainGroup.Display();
        
        // Zliczanie zadań w grupie
        int groupCompletedOnTime = tasks.Count(t => t.IsCompleted && !t.IsLate);
        int groupCompletedLate = tasks.Count(t => t.IsCompleted && t.IsLate);
        int groupPending = tasks.Count(t => !t.IsCompleted);
        int groupPendingLate = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        Console.WriteLine("\nPodsumowanie zadań w grupach:");
        Console.WriteLine($"Zadania wykonane na czas: {groupCompletedOnTime}");
        Console.WriteLine($"Zadania wykonane z opóźnieniem: {groupCompletedLate}");
        Console.WriteLine($"Zadania oczekujące: {groupPending}");
        Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {groupPendingLate}");
        Console.WriteLine(line);
        // Oznaczanie grupy jako wykonanej (rekursywne oznaczanie zadań w grupach)
        mainGroup.MarkAsCompleted(DateTime.Now);


       
        mainGroup.Display();
        int groupCompletedOnTime1 = tasks.Count(t => t.IsCompleted && !t.IsLate);
        int groupCompletedLate1 = tasks.Count(t => t.IsCompleted && t.IsLate);
        int groupPending1 = tasks.Count(t => !t.IsCompleted);
        int groupPendingLate1 = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        Console.WriteLine("\nPodsumowanie zadań w grupach:");
        Console.WriteLine($"Zadania wykonane na czas: {groupCompletedOnTime1}");
        Console.WriteLine($"Zadania wykonane z opóźnieniem: {groupCompletedLate1}");
        Console.WriteLine($"Zadania oczekujące: {groupPending1}");
        Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {groupPendingLate1}");

        
        Console.WriteLine(line);
    }
}
