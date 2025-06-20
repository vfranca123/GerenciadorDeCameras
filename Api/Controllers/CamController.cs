using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Api.Controllers // <--- namespace ajustado
{
    [ApiController]
    [Route("api/controller")] // <--- rota corrigida
    public class CamController : ControllerBase
    {



        [HttpGet]
        public IActionResult Get()
        {

            //exemplo de camera para testes 
            string rtspUrl = "rtsp://admin:LABLRI123456@10.3.195.43:554/cam/realmonitor?channel=1&subtype=0";
            string rtmpTarget = "rtmp://192.168.0.10/live/camera1";
            string arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" -f null -";
            this.RunFFmpeg("ffmpeg", arguments);

            return Ok("Servidor back feito");
        }

        public void RunFFmpeg(string ffmpegPath, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Console.WriteLine(e.Data);
                };

                process.Start();
                process.BeginErrorReadLine();
                Console.ReadKey();
                process.Kill();
            }
        }
    }
}
