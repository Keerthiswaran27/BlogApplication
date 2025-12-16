using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlogApp1.Client.Services
{
    public class ImageApiService
    {
        private readonly HttpClient _http;

        public ImageApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> UploadImageAsync(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "Image", file.Name);
            content.Add(new StringContent(file.Name), "Name");

            var response = await _http.PostAsync("api/image", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
            return result?.Url ?? string.Empty;
        }

        private class UploadResponse
        {
            public string Url { get; set; }
        }
    }
}
