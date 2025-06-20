
using System.Diagnostics;
using System.Net.WebSockets;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Habilita WebSockets
app.UseWebSockets();

//Configura o roteamento
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // suporte a controllers
    endpoints.Map("/stream", async context => // Rota para o WebSocket
    {
        if (context.WebSockets.IsWebSocketRequest) // 
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync(); // Aceita a conexão WebSocket
            var ffmpegPath = "ffmpeg";
            var rtspUrl = "rtsp://admin:LABLRI123456@10.3.195.43:554/cam/realmonitor?channel=1&subtype=0"; 
            var arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" -f mpegts -codec:v mpeg1video -";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo); // Inicia o processo ffmpeg
            using var output = process.StandardOutput.BaseStream; // captura a saída do ffmpeg
            var buffer = new byte[4096]; // o buffer para leitura dos dados

            while (!context.RequestAborted.IsCancellationRequested) // Enquanto a conexão não for cancelada
            {
                int bytesRead = await output.ReadAsync(buffer, 0, buffer.Length, context.RequestAborted); // o ffmpeg lê os dados
                if (bytesRead > 0) //se houver dados lidos
                {
                    await webSocket.SendAsync( // Envia os dados lidos para o WebSocket
                        new ArraySegment<byte>(buffer, 0, bytesRead),
                        WebSocketMessageType.Binary,
                        true,
                        context.RequestAborted
                    );
                }
                else break;
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stream ended", context.RequestAborted); // Fecha o WebSocket
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    });
});

app.Run();

