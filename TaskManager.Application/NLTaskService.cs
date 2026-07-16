using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;
using TaskManager.Application;
using TaskManager.Domain;

namespace TaskManager.Application;
public class NLTaskService : INLTaskService
{
    private readonly IConfiguration _configuration;
    private readonly ITaskService _taskService;
    private readonly ApplicationDbContext _context;
    public NLTaskService(IConfiguration configuration, ITaskService taskService, ApplicationDbContext context)
    {
        _configuration = configuration;
        _taskService = taskService;
        _context = context;
    }

    public async Task<TaskItem> NLToJson(string input, Guid userId)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var prompt = $@"You are a task extraction assistant. Today's date is {DateTime.UtcNow:yyyy-MM-dd}.

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

        var googleAI = new GoogleAI(apiKey);
        var model = googleAI.GenerativeModel(model : "gemini-3.1-flash-lite");
        var response = await model.GenerateContent(prompt);
        var responseText = response.Text;

        var doc = JsonDocument.Parse(responseText);
        var root = doc.RootElement;

        var title = root.GetProperty("title").GetString();
        var notes = root.GetProperty("notes").ValueKind == JsonValueKind.Null ? null : root.GetProperty("notes").GetString();
        var dueDate = root.GetProperty("dueDate").ValueKind == JsonValueKind.Null ? (DateOnly?)null : DateOnly.Parse(root.GetProperty("dueDate").GetString());
        var dueTime = root.GetProperty("dueTime").ValueKind == JsonValueKind.Null ? (TimeOnly?)null : TimeOnly.Parse(root.GetProperty("dueTime").GetString());
        var reminderAt = root.GetProperty("reminderAt").ValueKind == JsonValueKind.Null ? (DateTime?)null : DateTime.Parse(root.GetProperty("reminderAt").GetString());
        var projectName = root.GetProperty("projectName").ValueKind == JsonValueKind.Null ? null : root.GetProperty("projectName").GetString();

        Guid? projectId = null;
        if(projectName != null)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Name.ToLower() == projectName.ToLower() && p.UserId == userId);
            projectId = project?.Id;
        }

        return await _taskService.CreateTask(title, notes, userId, dueDate, dueTime, reminderAt, projectId);
    }
}