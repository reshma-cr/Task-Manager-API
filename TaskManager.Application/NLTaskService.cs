using Microsoft.Extensions.Configuration;
using TaskManager.Application;
using TaskManager.Domain;

public class NLTaskService : INLTaskService
{
    private readonly IConfiguration _configuration;
    private readonly ITaskService _taskService;
    public NLTaskService(IConfiguration configuration, ITaskService taskService)
    {
        _configuration = configuration;
        _taskService = taskService;
    }

    public async Task<TaskItem> NLToJson(string input, Guid userId)
    {
        string prompt = $@"You are a task extraction assistant. Today's date is {DateTime.UtcNow:yyyy-MM-dd}.

        Extract task details from the following natural language input and return ONLY a valid JSON object with no explanation, no preamble, and no markdown formatting.

        Input: ""{input}""

        Return this exact JSON structure:
        {{
            ""title"": ""short task title"",
            ""notes"": ""additional details or null if none"",
            ""dueDate"": ""yyyy-MM-dd or null if no date mentioned"",
            ""dueTime"": ""HH:mm:ss or null if no time mentioned"",
            ""reminderAt"": ""yyyy-MM-ddTHH:mm:ss or null if no time mentioned"",
            ""projectName"": ""add to Work project or null if no time mentioned""
        }}

        Rules:
        - title is required, keep it concise
        - notes is null unless there are extra details beyond the title
        - dueDate must be in yyyy-MM-dd format, resolve relative dates like ""tomorrow"" using today's date, or null if no date mentioned
        - dueTime must be in HH:mm:ss 24-hour format, or null if no time mentioned
        - reminderAt must be in yyyy-MM-ddTHH:mm:ss format if the user mentions a reminder, or null if not mentioned
        - projectName is the project name string if the user mentions a project (e.g. ""add to Work project""), or null if not mentioned
        - Return ONLY the JSON object, nothing else";


    }
}