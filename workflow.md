Great! Let's build something more practical - a **CLI Task Tracker** that'll teach you C# fundamentals while feeling DevOps-y. As a Java dev, you'll find C# familiar but with some nice syntactic sugar.

## 🛠️ Install .NET SDK on Ubuntu

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK (includes runtime and CLI)
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Verify installation
dotnet --version
```

## 🚀 Project: CLI Task Manager

### Step 1: Create Project Structure

```bash
# Create solution (think Maven parent POM)
mkdir TaskTracker && cd TaskTracker
dotnet new sln -n TaskTracker

# Create console app project
dotnet new console -n TaskTracker.CLI -f net8.0
dotnet sln add TaskTracker.CLI/TaskTracker.CLI.csproj

# Create class library (shared logic)
dotnet new classlib -n TaskTracker.Core -f net8.0
dotnet sln add TaskTracker.Core/TaskTracker.Core.csproj

# Add reference (like Maven dependency)
dotnet add TaskTracker.CLI reference TaskTracker.Core
```

### Step 2: Core Domain Model
**`TaskTracker.Core/Models/TaskItem.cs`**

```csharp
namespace TaskTracker.Core.Models;

public class TaskItem
{
    public int Id { get; init; }  // init = set only during initialization (Java final-ish)
    public string Title { get; set; } = string.Empty;  // Default value
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    // C# 12 primary constructor (similar to Java records)
    public TaskItem(int id, string title)
    {
        Id = id;
        Title = title;
    }
    
    // Expression-bodied member (lambda property)
    public string Status => IsComplete ? "✓" : "○";
    
    // String interpolation (way cleaner than Java)
    public override string ToString() => $"[{Status}] {Id}: {Title}";
}
```

### Step 3: Service Layer
**`TaskTracker.Core/Services/TaskService.cs`**

```csharp
using TaskTracker.Core.Models;
using System.Text.Json;

namespace TaskTracker.Core.Services;

public class TaskService
{
    private readonly string _dataFile;
    private List<TaskItem> _tasks;
    
    public TaskService(string dataFile = "tasks.json")
    {
        _dataFile = dataFile;
        _tasks = LoadTasks();
    }
    
    // Modern switch expression (pattern matching)
    public string ExecuteCommand(string command, string? argument = null) => command.ToLower() switch
    {
        "add" when !string.IsNullOrWhiteSpace(argument) => AddTask(argument),
        "list" => ListTasks(),
        "complete" when int.TryParse(argument, out var id) => CompleteTask(id),
        _ => "Unknown command. Try: add <title>, list, complete <id>"
    };
    
    private string AddTask(string title)
    {
        var task = new TaskItem(_tasks.Count + 1, title);
        _tasks.Add(task);
        SaveTasks();
        return $"Added: {task}";
    }
    
    private string ListTasks()
    {
        if (!_tasks.Any()) return "No tasks found!";  // LINQ method
        return string.Join('\n', _tasks.Select(t => t.ToString()));
    }
    
    private string CompleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);  // LINQ
        if (task is null) return $"Task {id} not found!";  // Pattern matching null check
        
        task.IsComplete = true;
        SaveTasks();
        return $"Completed: {task}";
    }
    
    private List<TaskItem> LoadTasks()
    {
        if (!File.Exists(_dataFile)) return new();  // Target-typed new
        
        var json = File.ReadAllText(_dataFile);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new();
    }
    
    private void SaveTasks()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_dataFile, JsonSerializer.Serialize(_tasks, options));
    }
}
```

### Step 4: CLI Application
**`TaskTracker.CLI/Program.cs`**

```csharp
using TaskTracker.Core.Services;

// Top-level program (no Main method needed - C# 9+)
var service = new TaskService();

Console.WriteLine("📝 Task Tracker CLI");
Console.WriteLine("Commands: add <title> | list | complete <id> | exit\n");

// Infinite loop with pattern matching
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrEmpty(input)) continue;
    
    // Split input and use array patterns
    var parts = input.Split(' ', 2);  // Max 2 parts
    var (command, argument) = parts.Length switch
    {
        1 => (parts[0], null),
        2 => (parts[0], parts[1]),
        _ => ("", null)
    };
    
    if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;
    
    Console.WriteLine(service.ExecuteCommand(command, argument));
    Console.WriteLine();
}
```

## 🔧 Build & Run

```bash
# Build entire solution
dotnet build

# Run the CLI app
dotnet run --project TaskTracker.CLI

# Or create a self-contained executable
dotnet publish TaskTracker.CLI -c Release -r linux-x64 --self-contained -o ./publish
./publish/TaskTracker.CLI
```

## 📦 Add a NuGet Package (like Maven dependencies)

Let's add colored console output:

```bash
# Add Spectre.Console for pretty CLI
dotnet add TaskTracker.CLI package Spectre.Console

# List packages
dotnet list package
```

## 🎯 Key C# Concepts for Java Devs

| Java | C# |
|------|-----|
| `final` | `readonly` (runtime) or `const` (compile-time) |
| `extends/implements` | `: BaseClass, IInterface` |
| `package` | `namespace` |
| Stream API | LINQ (`Where`, `Select`, `Any`) |
| `Optional<T>` | `T?` (nullable reference types) |
| `var` (Java 10+) | `var` (type inference) |
| Records | Records or `init` properties |

## 🚀 DevOps-style Commands

```bash
# Watch for changes and auto-rebuild
dotnet watch run --project TaskTracker.CLI

# Run tests (if you had them)
dotnet test

# Create a Docker image
cat > Dockerfile << EOF
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["./TaskTracker.CLI"]
EOF

docker build -t task-tracker .
```

This gives you a real CLI tool with file persistence, JSON serialization, and modern C# features - way more practical than "Hello World"! 🎉





