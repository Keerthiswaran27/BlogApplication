using BlogApp1.Shared;
using System.Net.Http.Json;

namespace BlogApp1.Client.Services
{
    public class RAGHttpService
    {
        private readonly HttpClient _http;
        public RAGHttpService(HttpClient http) => _http = http;

        // ✅ FIXED: Full async - NO .Result!
        public async Task<ChatResponse> ChatAsync(string question)
        {
            var request = new ChatRequest { Question = question };
            var response = await _http.PostAsJsonAsync("api/rag/chat", request);
            response.EnsureSuccessStatusCode();  // Throws if not 200
            return await response.Content.ReadFromJsonAsync<ChatResponse>()!;
        }   
    }
}
