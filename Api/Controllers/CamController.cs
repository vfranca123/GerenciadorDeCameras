using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class CamController : ControllerBase
    {
        [HttpGet("stream")]
        public async Task Stream() // é colocado como assincrono pois recebe dadso o tempo todo 
        {
            string rtspUrl = "rtsp://admin:LABLRI123456@10.3.195.43:554/cam/realmonitor?channel=1&subtype=0";
            string arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" -f mpegts -codec:v mpeg1video -codec:a mp2 -";

            Response.ContentType = "video/mp2t";

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
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

                using var output = process.StandardOutput.BaseStream;
                await output.CopyToAsync(Response.Body);

                await process.WaitForExitAsync();
            }
        }
    }
}
