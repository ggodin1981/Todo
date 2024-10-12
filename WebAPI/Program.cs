//using Microsoft.EntityFrameworkCore;
//using TodoApi.Data;
//using TodoApi.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddSwaggerGen();
//// Register the In-Memory database using Entity Framework Core
//builder.Services.AddScoped<ITodoService, TodoService>();
//builder.Services.AddDbContext<TodoContext>(opt =>
//    opt.UseInMemoryDatabase("TodoList"));

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllers();
//app.Run();



using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register the In-Memory database
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
// Enable CORS to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
