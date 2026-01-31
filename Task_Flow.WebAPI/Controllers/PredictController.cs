using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Text.Json.Serialization;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictController : ControllerBase
    {  private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PredictController(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    [HttpGet("complete")]
    public async Task<IActionResult> GetSuggestion([FromQuery] string prompt)
    {
        try
        {
            var apiKey = _config["GeminiApiKey"];
             var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

            var requestBody = new
            {
                contents = new[] {
                new { parts = new[] { new { text = $"Complete this word: {prompt}. Return only the single completed word, no explanation." } } }
            }
            };

             var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-goog-api-key", apiKey); // Anahtarı buraya ekliyoruz
            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Gemini API Error: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            string prediction = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            return Ok(new { suggestion = prediction?.Trim() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
     public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
}
