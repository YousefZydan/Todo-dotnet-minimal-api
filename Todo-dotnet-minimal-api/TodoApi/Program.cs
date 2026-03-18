using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var todos = new List<Todo>();
var nextId = 1;

app.MapGet("/todos", () => todos)
   .WithName("GetAllTodos");

app.MapGet("/todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
})
   .WithName("GetTodoById");

app.MapPost("/todos", ([FromBody] CreateTodoRequest request) =>
{
    var newTodo = new Todo
    {
        Id = nextId++,
        Title = request.Title!,
        IsCompleted = false
    };
    todos.Add(newTodo);
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
})
   .WithName("CreateTodo");

app.MapPut("/todos/{id}", (int id, [FromBody] UpdateTodoRequest request) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todo.Title = request.Title ?? todo.Title;
    todo.IsCompleted = request.IsCompleted ?? todo.IsCompleted;

    return Results.Ok(todo);
})
   .WithName("UpdateTodo");

app.MapDelete("/todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todos.Remove(todo);
    return Results.NoContent();
})
   .WithName("DeleteTodo");

app.Run();

class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }   // خليناها nullable
    public bool IsCompleted { get; set; }
}

class CreateTodoRequest
{
    public string? Title { get; set; }
}

class UpdateTodoRequest
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}