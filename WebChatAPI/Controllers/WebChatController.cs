using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebChatController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public WebChatController(IWebHostEnvironment webHost)
        {
            _env = webHost;
        }
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var responseMessage = await GetResponseFromJson(request.UserInput);
            return Ok(new { response = responseMessage });
        }

        private async Task<string> GetResponseFromJson(string userInput)
        {
            var jsonFilePath = Path.Combine(_env.WebRootPath, "responses.json");
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return "Error: Response data file not found.";
            }

            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var responses = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            // Check if there is a matching response
            foreach (var key in responses.Keys)
            {
                if (userInput.ToLower().Contains(key.ToLower()))
                {
                    return responses[key];
                }
            }

            return "Sorry, I don't have a response for that.";
        }
    }


    public class ChatRequest
    {
        public string UserInput { get; set; }
    }
}
   

