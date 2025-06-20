using Api.Controllers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// constrói app
var app = builder.Build();

// define uso de controllers
app.MapControllers();
   
// roda o servidor
app.Run();
