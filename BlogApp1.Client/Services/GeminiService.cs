using BlogApp1.Shared; // optional if you store models in Shared
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlogApp1.Client.Services
{
    public class GeminiService
    {
        private readonly HttpClient _http;

        private readonly IJSRuntime _js;

        public GeminiService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<GeminiBlogOutput?> GenerateBlogAsync(GeminiRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/gemini/generate", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GeminiBlogOutput>();
            }

            // Return null if something fails
            return null;
        }
        public async Task<(bool Success, string Message)> SaveAIBlogAsync(AIBlogDto blog)
        {
            try
            {
                // 🧠 Get author info from session storage
                var authorName = await _js.InvokeAsync<string>("sessionStorage.getItem", "name");
                var authorUid = await _js.InvokeAsync<string>("sessionStorage.getItem", "uid");

                // Fallbacks (if sessionStorage is empty)
                blog.AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Guest Author" : authorName;
                blog.AuthorUid = string.IsNullOrWhiteSpace(authorUid) ? Guid.NewGuid().ToString() : authorUid;

                // 📨 Send to backend API
                var response = await _http.PostAsJsonAsync("api/blogs/saveaiblog", blog);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var msg = doc.RootElement.TryGetProperty("message", out var msgProp)
                        ? msgProp.GetString()
                        : "Blog saved successfully.";
                    return (true, msg ?? "Saved");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, $"Save failed: {error}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
    
    // same models as backend (can be shared)
    public class GeminiRequest
    {
        public string Title { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Tone { get; set; } = "";
        public string Length { get; set; } = "";
        public string CustomPrompt { get; set; } = ""; // ✅ replaced keywords
        public bool IncludeImage { get; set; }
    }
    

    public class GeminiBlogOutput
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Meta_Description { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public string Content { get; set; }
        public string Image_Url { get; set; }
        public string Estimated_Read_Time { get; set; }
    }
}
