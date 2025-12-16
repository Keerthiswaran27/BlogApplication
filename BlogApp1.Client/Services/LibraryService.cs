using BlogApp1.Shared;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace BlogApp1.Client.Services
{
    public class LibraryService
    {
        private readonly HttpClient _http;
        private readonly Task<Guid> _userId;
        private readonly IJSRuntime _js;
        public LibraryService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
            // Fetch userId from sessionStorage or localStorage
            _userId = GetUserIdFromSession();
        }

        private async Task<Guid> GetUserIdFromSession()
        {
            var id = await _js.InvokeAsync<Guid>("sessionStorage.getItem", "uid");
            return id;
        }

        // ------------------------
        // History APIs
        // ------------------------
        public async Task<List<HistoryDto>> GetHistoryAsync()
        {
            return await _http.GetFromJsonAsync<List<HistoryDto>>($"api/library/history/{_userId}") ?? new List<HistoryDto>();
        }

        public async Task AddToHistoryAsync(HistoryDto history)
        {
            await _http.PostAsJsonAsync("api/library/history/add", history);
        }

        // ------------------------
        // Collections APIs
        // ------------------------
        //public async Task<List<CollectionDto>> GetCollectionsAsync()
        //{
        //    return await _http.GetFromJsonAsync<List<CollectionDto>>($"api/library/collections/{_userId}") ?? new List<CollectionDto>();
        //}

        //public async Task AddToCollectionAsync(CollectionDto collection)
        //{
        //    await _http.PostAsJsonAsync("api/library/collections/add", collection);
        //}

        //public async Task RemoveFromCollectionAsync(Guid collectionId, int blogId)
        //{
        //    await _http.DeleteAsync($"api/library/collections/remove/{collectionId}/{blogId}");
        //}
        private async Task<Guid?> GetUserIdAsync()
        {
            var id = await _js.InvokeAsync<string>("sessionStorage.getItem", "uid");
            return Guid.TryParse(id, out var guid) ? guid : (Guid?)null;
        }

        public async Task<List<CollectionResponse>> GetCollectionsAsync()
        {
            var uid = await GetUserIdAsync();
            if (uid == null) return new List<CollectionResponse>();

            return await _http.GetFromJsonAsync<List<CollectionResponse>>($"api/collections/{uid}")
                   ?? new List<CollectionResponse>();
        }

        public async Task<CollectionDto?> CreateCollectionAsync(string name)
        {
            var uid = await GetUserIdAsync();
            if (uid == null) return null;

            var collection = new CollectionDto
            {
                UserId = uid.Value,
                CollectionName = name,
                BlogIds = Array.Empty<int>()
            };

            var res = await _http.PostAsJsonAsync("api/collections", collection);
            return await res.Content.ReadFromJsonAsync<CollectionDto>();
        }

        public async Task AddBlogToCollectionAsync(Guid collectionId, int blogId)
        {
            await _http.PostAsync($"api/collections/addBlog/{collectionId}/{blogId}", null);
        }

        public async Task RemoveBlogFromCollectionAsync(Guid collectionId, int blogId)
        {
            await _http.DeleteAsync($"api/collections/removeBlog/{collectionId}/{blogId}");
        }

        public async Task DeleteCollectionAsync(Guid collectionId)
        {
            await _http.DeleteAsync($"api/collections/{collectionId}");
        }
    }
}
