using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using Vosk;

namespace AudioTranscriptionAPI.Services
{
    public class AudioService
    {
        private readonly Model _model;

        public AudioService(string modelPath)
        {
            _model = new Model(modelPath);
        }

        public async Task<string> TranscribeAudioFileAsync(Stream audioStream, string originalFormat)
        {
            Vosk.Vosk.SetLogLevel(0);

            await using var convertedStream = await ConvertTo16kHzMonoAsync(audioStream, originalFormat);

            return await TranscribeStreamAsync(convertedStream, _model);
        }

        private static async Task<Stream> ConvertTo16kHzMonoAsync(Stream audioStream, string originalFormat)
        {
            var outStream = new MemoryStream();
            var tempInputPath = Path.GetTempFileName();
            var tempOutputPath = Path.ChangeExtension(Path.GetTempFileName(), ".wav");

            
            await using (var tempInputStream = new FileStream(tempInputPath, FileMode.Create, FileAccess.Write))
            {
                await audioStream.CopyToAsync(tempInputStream);
            }

            var ffmpegArgs = $"-i \"{tempInputPath}\" -ar 16000 -ac 1 \"{tempOutputPath}\"";
            var ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = ffmpegArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffmpegProcess.Start();

            var outputTask = ffmpegProcess.StandardOutput.ReadToEndAsync();
            var errorTask = ffmpegProcess.StandardError.ReadToEndAsync();

            await ffmpegProcess.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            if (ffmpegProcess.ExitCode != 0)
            {
                throw new InvalidOperationException($"FFmpeg falhou ao converter o arquivo de áudio. Saída: {output}. Erro: {error}");
            }

            await using (var tempOutputStream = new FileStream(tempOutputPath, FileMode.Open, FileAccess.Read))
            {
                await tempOutputStream.CopyToAsync(outStream);
            }

          
            File.Delete(tempInputPath);
            File.Delete(tempOutputPath);

            outStream.Position = 0; 
            return outStream;
        }

        private static async Task<string> TranscribeStreamAsync(Stream audioStream, Model model)
        {
            if (audioStream == null || audioStream.Length == 0)
            {
                throw new ArgumentException("O stream de áudio está vazio ou nulo.");
            }

            using var recognizer = new VoskRecognizer(model, 16000.0f);
            using var waveStream = new WaveFileReader(audioStream);

            byte[] buffer = new byte[4096];
            string result = "";
            int bytesRead;
            while ((bytesRead = await waveStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                if (recognizer.AcceptWaveform(buffer, bytesRead))
                {
                    result += recognizer.Result();
                }
                else
                {
                    result += recognizer.PartialResult();
                }
            }

            result += recognizer.FinalResult();
            return result;
        }
    }
}
