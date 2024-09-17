
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using AudioTranscriptionAPI.Services;

namespace AudioTranscriptionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranscriptionController : ControllerBase
    {
        private readonly AudioService _audioService;

        public TranscriptionController(AudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpPost]
        public async Task<IActionResult> PostTranscription([FromBody] string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound("Arquivo de áudio não encontrado.");
            }

            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var transcription = await _audioService.TranscribeAudioFileAsync(fileStream, "opus");
            return Ok(new { text = transcription });
        }
    }
}
