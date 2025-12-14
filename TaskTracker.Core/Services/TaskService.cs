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

	// modern switch expression (pattern matching)
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
		if (!_tasks.Any()) return "no tasks found!"; // LINQ method
		return string.Join('\n', _tasks.Select(t => t.ToString()));
	}

	private string CompleteTask(int id)
	{
		var task = _tasks.FirstOrDefault(t => t.Id == id);
		if (task is null) return $"Task {id} not found!";

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
