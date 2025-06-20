
using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {


        string rtspUrl = "rtsp://admin:LABLRI123456@10.3.195.43:554/cam/realmonitor?channel=1&subtype=0";
        string rtmpTarget = "rtmp://192.168.0.10/live/camera1";

        string arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" -f null -";
        /*
        "-rtsp_transport tcp" : fala para o ffmpg usar conectar ao RTSP usando TCP 
        "-i \ Url\" : pega a stream da camera 
        "-c:v copy" : copia o video
        "-f flv" : Formato de saida stream
        \"{rtmpTarget}\" onde a imagem ira sair 
        */       
        RunFFmpeg("ffmpeg",arguments);
    }

        public static void RunFFmpeg(string ffmpegPath, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath, // o local onde esta o executavel do ffmpeg 
            Arguments = arguments, //o comando que sera exexuctado
            UseShellExecute = false, // se usara o shell da maquian 
            RedirectStandardError = true,  // FFmpeg logs vão para stderr
            RedirectStandardOutput = false, //os dados não estão indos diretamento para algum lugar então vão para o terminal, se tiver algum lugar pra ir coloque como true
            CreateNoWindow = false //Define se ira rodar no background ou no teminal mesmo false coloca no terminal true ele fica no background 
        };

        using (var process = new Process { StartInfo = startInfo }) // cria um processo que represanta um processo exteno , no caso o ffmpeg, startinfo dita como o processo sera iniciado 
        //o using garante que o processo sera descartado apos terminal 
        {
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine(e.Data);  // Printa FFmpeg logs 
            };

            process.Start(); //começa o processo 
            process.BeginErrorReadLine(); //le as mensagens assicronametne que são enviadas pelo ffmpeg
            Console.ReadKey(); //agurada o usuario clicar alguma tecla 

            process.Kill(); //acaba o processo 
        }
    }

}
