using TaskTracker.Core.Services;

// Top-level program (no Main method needed - C#9+)
var service = new TaskService();

Console.WriteLine("Task Tracker CLI");
Console.WriteLine("Commands: add <title> | list | complete <id> | exit\n");

while (true)
{
	Console.Write("> ");
	var input = Console.ReadLine()?.Trim();

	if (string.IsNullOrEmpty(input)) continue;

	var parts = input.Split(' ', 2);
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
