using BlogApp1.Shared;
using Microsoft.JSInterop;
using Nextended.Core.Extensions;
using System.Net.Http.Json;

namespace BlogApp1.Client.Services
{
    public class CommentService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public CommentService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }
        public async Task<List<CommentDto>> GetCommentsByBlogIdAsync(int blogId)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<CommentDto>>($"api/comments/{blogId}");
                return response ?? new List<CommentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching comments: {ex.Message}");
                return new List<CommentDto>();
            }
        }
        public async Task<string> GetAuthorName(Guid? uid)
        {
            try
            {
                var response = await _http.GetStringAsync($"api/comments/user-info/{uid}");
                return response ?? string.Empty;


            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public async Task<string> GetTitleAsync(int id)
        {
            var response = await _http.GetStringAsync($"api/comments/gettitle/{id}");
            return response ?? string.Empty;
        }
        public async Task<string> GetAuthorUsingId(int BlogId)
        {
            try
            {
                var response = await _http.GetStringAsync($"api/comments/authorname/{BlogId}");
                Console.WriteLine(response);
                return response ?? string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<List<CommentDto>> GetRecentCommentsAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<CommentDto>>($"api/comments/recentcomments"); 
                return response ?? new List<CommentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching comments: {ex.Message}");
                return new List<CommentDto>();
            }
        }
        public async Task<Stats> GetTotalStatsAsync()
        {
            try
            {
                Stats response = await _http.GetFromJsonAsync<Stats>($"api/site/overallstats");
                Console.Write(response.TotalBlogs);
                return response ?? new Stats();
            }
            catch (Exception ex)
            {
                return new Stats();
            }
        }
        public class UserInfoDto
        {
            public string Name { get; set; }
        }
       
        public async Task<CommentDto> AddComment(
             int blogId,
             string content,
             Guid? parentCommentUid = null)
        {
            try
            {
                var userid = await _js.InvokeAsync<Guid>("sessionStorage.getItem", "uid");
                var authorname = await _js.InvokeAsync<string>("sessionStorage.getItem", "name");
                var dto = new CommentDto
                {
                    CommentUid=Guid.NewGuid(),
                    BlogId = blogId,
                    CommentUserId = userid,
                    ParentCommentUid = parentCommentUid,
                    IsParent = parentCommentUid == null,
                    Content = content,
                    AuthorName = authorname
                };

                var response = await _http.PostAsJsonAsync("api/comments/add", dto);
                if (response.IsSuccessStatusCode)
                {
                    var addedComment = await response.Content.ReadFromJsonAsync<CommentDto>();
                    Console.WriteLine(addedComment); // check if returned
                }
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the result (added comment returned from API)
                    var addedComment = await response.Content.ReadFromJsonAsync<CommentDto>();
                    return addedComment;
                }
                else
                {
                    // Optionally, you can log the response error here
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AddComment failed: {error}");
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        
    }
}
