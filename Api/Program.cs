using System;


var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona os serviços dos controladores (os atendentes da API)
builder.Services.AddControllers();

var app = builder.Build();
// 7. Usa roteamento de controladores
app.MapControllers();

// 8. Inicia o servidor
app.Run();