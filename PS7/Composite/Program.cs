using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Collections.Generic;

public interface ITaskComponent
{
    bool IsCompleted { get; }
     DateTime StartDate { get; }
     DateTime EndDate { get; }

    void MarkAsCompleted (DateTime completionDate);
    void Display(int depth = 0);
    string GenerateGanttChart(DateTime projectStart, DateTime projectEnd);
   

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
    public string GetStatusGant(DateTime date )
    {
        if(IsCompleted && !IsLate && date >= StartDate && date <=EndDate)
        {
            return "X";
        }
        if(IsCompleted && IsLate && date >= StartDate && date <=EndDate)
        {
            return "!";
        }
        if(date >= StartDate && date <=EndDate)
        {
            return "#";
        }
        return ".";
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

    public string GenerateGanttChart(DateTime projectStart, DateTime projectEnd)
    {
        // Obliczenie liczby dni całkowitego projektu
        int totalProjectDays = (projectEnd - projectStart).Days + 1;

        // Obliczenie liczby dni trwania zadania
        int taskDuration = (EndDate - StartDate).Days + 1;

        // Tworzymy pasek z wykresem Gantta
        string taskBar = $"{Name.Substring(0,3),-25} "; // Nazwa zadania z odpowiednim formatowaniem

        // Przechodzimy po dniach w zakresie projektu
        for (int i = 0; i < totalProjectDays; i++)
        {
            DateTime currentDate = projectStart.AddDays(i);

            // Jeżeli dzień jest w zakresie zadania, generujemy odpowiedni symbol
            if (currentDate >= StartDate && currentDate <= EndDate)
            {
                taskBar += GetStatusGant(currentDate); // X, !, # lub .
            }
            else
            {
                taskBar += "."; // Jeżeli dzień poza zakresem zadania, wyświetlamy kropkę
            }
        }

        return taskBar;
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

    public string GenerateGanttChart(DateTime projectStart, DateTime projectEnd)
    {
        List<String> chart = new List<string>();

        foreach(var task in _tasks)
        {
            chart.Add(task.GenerateGanttChart(projectStart, projectEnd));
        }
        return string .Join("\n", chart);
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


public class RecurringTask
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int IntervalInDays { get; set; } // Okres powtarzania w dniach
    public List<TaskInstance> Instances { get; set; } = new List<TaskInstance>();

    // Konstruktor
    public RecurringTask(string name, DateTime startDate, DateTime endDate, int intervalInDays)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        IntervalInDays = intervalInDays;
        GenerateTaskInstances();
    }

    // Generowanie instancji zadania na podstawie daty rozpoczęcia i zakończenia
    private void GenerateTaskInstances()
    {
        DateTime currentDate = StartDate;

        // Generowanie instancji zadania cyklicznego
        while (currentDate <= EndDate)
        {
            Instances.Add(new TaskInstance
            {
                Date = currentDate,
                Status = TaskStatus.NotCompleted
            });
            currentDate = currentDate.AddDays(IntervalInDays);
        }
    }

    // Metoda do oznaczania zadania jako wykonanego
    public void MarkCompleted(DateTime completedDate)
    {
        foreach (var instance in Instances)
        {
            if (instance.Date == completedDate)  // Sprawdzamy, czy data powtórzenia jest równa dacie wykonania
            {
                instance.Status = TaskStatus.CompletedOnTime;
            }
            else if (instance.Date < completedDate && instance.Status == TaskStatus.NotCompleted)
            {
                instance.Status = TaskStatus.CompletedLate;
            }
        }
    }

    // Metoda do wyświetlania wykresu Gantta
    public string GetGanttChart()
    {
        char[] ganttRepresentation = new char[Instances.Count];

        for (int i = 0; i < Instances.Count; i++)
        {
            ganttRepresentation[i] = Instances[i].Status switch
            {
                TaskStatus.CompletedOnTime => 'X',
                TaskStatus.CompletedLate => '!',
                TaskStatus.NotCompleted => '.',
                _ => '.'
            };
        }

        return new string(ganttRepresentation);
    }
}

// Statusy zadania
public enum TaskStatus
{
    NotCompleted,
    CompletedOnTime,
    CompletedLate
}

// Instancja zadania cyklicznego
public class TaskInstance
{
    public DateTime Date { get; set; }
    public TaskStatus Status { get; set; }
}

public class Program
{
    public static void Main()
    {
        //int consoleWidth = Console.WindowWidth;
        //string line = new string('-', consoleWidth - 1);
        //// Przykładowe zadania
        //var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        //var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));
        //var task3 = new Task("2A - Projektowanie schematu bazy danych", new DateTime(2024, 10, 28), new DateTime(2024, 11, 3));
        //var task4 = new Task("2B - Tworzenie zapytań SQL", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

        //// Oznaczanie przykładowych zadań jako wykonane (z różnymi datami ukończenia)
        //task1.MarkAsCompleted(new DateTime(2024, 10, 25)); // Wykonane na czas
        //task2.MarkAsCompleted(new DateTime(2024, 11, 1)); // Wykonane z opóźnieniem
        //// task3 i task4 są jeszcze niewykonane

        //// Lista zadań (przykładowa organizacja wyłącznie według nazw)
        //var tasks = new List<Task> { task1, task2, task3, task4 };

        //// Wyświetlanie listy zadań i ich statusów
        //Console.WriteLine("Lista zadań:");
        //foreach (var task in tasks)
        //{
        //    Console.WriteLine(task);
        //}

        //// Zliczanie wykonanych, opóźnionych i oczekujących zadań
        //int completedOnTime = tasks.Count(t => t.IsCompleted && !t.IsLate);
        //int completedLate = tasks.Count(t => t.IsCompleted && t.IsLate);
        //int pending = tasks.Count(t => !t.IsCompleted);
        //int pendingLate = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        //Console.WriteLine("\nPodsumowanie zadań:");
        //Console.WriteLine($"Zadania wykonane na czas: {completedOnTime}");
        //Console.WriteLine($"Zadania wykonane z opóźnieniem: {completedLate}");
        //Console.WriteLine($"Zadania oczekujące: {pending}");
        //Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {pendingLate}");
        //Console.WriteLine(line);
        //// Tworzenie grupy zadań
        //var group1 = new TaskGroup("Grupa 1");
        //group1.Add(task1);
        //group1.Add(task2);

        //var group2 = new TaskGroup("Grupa 2");
        //group2.Add(task3);
        //group2.Add(task4);

        //// Tworzenie głównej grupy zadań
        //var mainGroup = new TaskGroup("Projekt");
        //mainGroup.Add(group1);
        //mainGroup.Add(group2);

        //// Wyświetlanie struktury hierarchicznej zadań
        //Console.WriteLine("\nStruktura grupy zadań:");
        //mainGroup.Display();

        //// Zliczanie zadań w grupie
        //int groupCompletedOnTime = tasks.Count(t => t.IsCompleted && !t.IsLate);
        //int groupCompletedLate = tasks.Count(t => t.IsCompleted && t.IsLate);
        //int groupPending = tasks.Count(t => !t.IsCompleted);
        //int groupPendingLate = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        //Console.WriteLine("\nPodsumowanie zadań w grupach:");
        //Console.WriteLine($"Zadania wykonane na czas: {groupCompletedOnTime}");
        //Console.WriteLine($"Zadania wykonane z opóźnieniem: {groupCompletedLate}");
        //Console.WriteLine($"Zadania oczekujące: {groupPending}");
        //Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {groupPendingLate}");
        //Console.WriteLine(line);
        //// Oznaczanie grupy jako wykonanej (rekursywne oznaczanie zadań w grupach)
        //mainGroup.MarkAsCompleted(DateTime.Now);



        //mainGroup.Display();
        //int groupCompletedOnTime1 = tasks.Count(t => t.IsCompleted && !t.IsLate);
        //int groupCompletedLate1 = tasks.Count(t => t.IsCompleted && t.IsLate);
        //int groupPending1 = tasks.Count(t => !t.IsCompleted);
        //int groupPendingLate1 = tasks.Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        //Console.WriteLine("\nPodsumowanie zadań w grupach:");
        //Console.WriteLine($"Zadania wykonane na czas: {groupCompletedOnTime1}");
        //Console.WriteLine($"Zadania wykonane z opóźnieniem: {groupCompletedLate1}");
        //Console.WriteLine($"Zadania oczekujące: {groupPending1}");
        //Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {groupPendingLate1}");


        //Console.WriteLine(line);




        // Option 2 



        // Ustalanie szerokości konsoli i linii
        //int consoleWidth = Console.WindowWidth;
        //string line = new string('-', consoleWidth - 1);

        //// Tworzymy zadania
        //var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        //var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));
        //var task3 = new Task("2A - Projektowanie schematu bazy danych", new DateTime(2024, 10, 28), new DateTime(2024, 11, 3));
        //var task4 = new Task("2B - Tworzenie zapytań SQL", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

        //// Oznaczanie przykładowych zadań jako wykonane (z różnymi datami ukończenia)
        //task1.MarkAsCompleted(new DateTime(2024, 10, 25)); // Wykonane na czas
        //task2.MarkAsCompleted(new DateTime(2024, 11, 1)); // Wykonane z opóźnieniem
        //// task3 i task4 są jeszcze niewykonane

        //// Lista zadań (przykładowa organizacja wyłącznie według nazw)
        //var tasks = new List<Task> { task1, task2, task3, task4 };

        //// Wyświetlanie listy zadań i ich statusów
        //Console.WriteLine("Lista zadań:");
        //foreach (var task in tasks)
        //{
        //    Console.WriteLine(task);
        //}

        //// Tworzenie grupy zadań (zwykła i opcjonalna)
        //var group1 = new TaskGroup("Grupa Obowiązkowa");
        //group1.Add(task1);
        //group1.Add(task2);

        //var group2 = new OptionalTaskGroup("Grupa Opcjonalna");
        //group2.Add(task3);
        //group2.Add(task4);

        //// Wyświetlanie grup
        //Console.WriteLine("\nStruktura grupy zadań:");
        //group1.Display();
        //group2.Display();
        //Console.WriteLine(line);

        //// Oznaczanie grupy jako wykonanej (rekursywne oznaczanie zadań w grupach)

        //group1.MarkAsCompleted(DateTime.Now);
        //group2.MarkAsCompleted(DateTime.Now);

        //// Ponowne wyświetlanie statusów grup
        //group1.Display();
        //group2.Display();


        //Option 3

        //// Tworzymy zadania
        //var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        //var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));
        //var task3 = new Task("2A - Projektowanie schematu bazy danych", new DateTime(2024, 10, 28), new DateTime(2024, 11, 3));
        //var task4 = new Task("2B - Tworzenie zapytań SQL", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

        //// Oznaczamy przykładowe zadania jako wykonane
        //task1.MarkAsCompleted(new DateTime(2024, 10, 25)); // Wykonane na czas
        //task2.MarkAsCompleted(new DateTime(2024, 11, 1)); // Wykonane z opóźnieniem

        //// Tworzymy grupę zadań
        //var group = new TaskGroup("Grupa 1");
        //group.Add(task1);
        //group.Add(task2);
        //group.Add(task3);
        //group.Add(task4);

        //// Określamy zakres dat
        //DateTime projectStart = new DateTime(2024, 10, 21);
        //DateTime projectEnd = new DateTime(2024, 11, 30);

        //// Generowanie wykresu Gantta
        //Console.WriteLine(group.GenerateGanttChart(projectStart, projectEnd));

        //Option 4
        var recurringTask = new RecurringTask(
    "Zadanie 4",
    new DateTime(2024, 11, 1),
    new DateTime(2024, 11, 30),
    5 // Zadanie powtarza się co 5 dni
);

        // Oznaczanie niektórych powtórzeń jako wykonanych
        recurringTask.MarkCompleted(new DateTime(2024, 11, 5));
        recurringTask.MarkCompleted(new DateTime(2024, 11, 10));

        // Wyświetlanie wykresu Gantta
        Console.WriteLine($"Wykres Gantta dla zadania '{recurringTask.Name}':");
        Console.WriteLine(recurringTask.GetGanttChart());

    }
}
