using BlogApp1.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlogApp1.Client.Services
{
    public class AdminService
    {
        private readonly HttpClient _http;

        public AdminService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<BlogUserDto>> GetAllUsersAsync()
        {
            var result = await _http.GetFromJsonAsync<List<BlogUserDto>>("api/admin");
            Console.WriteLine("Yes it is entering the service");
            if(result == null)
            {
                Console.WriteLine("success");
            }
            return result ?? new List<BlogUserDto>();
        }

        public async Task<BlogUserDto?> GetUserByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<BlogUserDto>($"api/admin/getuser/{id}");
        }

        public async Task<bool> UpdateUserAsync(BlogUserDto user)
        {

            var response = await _http.PutAsJsonAsync($"api/admin/updateuser", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/admin/deleteuser/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<string>> GetFollowersAsync(List<string> followerIds)
        {
            if (followerIds == null || !followerIds.Any())
                return new List<string>();

            // Convert list to comma-separated string
            var idsQuery = string.Join(",", followerIds);

            // Call backend API
            var response = await _http.GetAsync($"api/admin/getfollowers?ids={idsQuery}");
            if (!response.IsSuccessStatusCode)
            {
                // Optionally, handle 404 or errors here
                return new List<string>();
            }

            var fullNames = await response.Content.ReadFromJsonAsync<List<string>>();
            return fullNames ?? new List<string>();
        }
        public async Task<List<string>> GetFollowingAsync(List<string> followingIds)
        {
            if (followingIds == null || !followingIds.Any())
                return new List<string>();

            var idsQuery = string.Join(",", followingIds);

            var response = await _http.GetAsync($"api/admin/getfollowing?ids={idsQuery}");
            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var fullNames = await response.Content.ReadFromJsonAsync<List<string>>();
            return fullNames ?? new List<string>();
        }
        public async Task<BlogDto> GetBlogById(int id)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<BlogDto>($"api/admin/blogid/{id}");
                Console.WriteLine("✅ Entered GetBlogById service");

                if (result == null)
                {
                    Console.WriteLine("⚠️ Blog not found or null result");
                    return new BlogDto();
                }

                Console.WriteLine("✅ Blog fetched successfully");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching blog: {ex.Message}");
                return new BlogDto();
            }
        }
        public async Task<bool> UpdateBlogAsync(BlogDto blog)
        {
            if (blog == null) return false;

            var response = await _http.PutAsJsonAsync("api/admin/update-blog", blog);

            return response.IsSuccessStatusCode;
        }
        public async Task<List<CommentDto>> GetAllCommentsAsync()
        {
            var response = await _http.GetFromJsonAsync<List<CommentDto>>("api/admin/getallcomments");
            return response ?? new List<CommentDto>();
        }

        // ✅ 2. Get one comment by CommentUid
        public async Task<CommentDto?> GetCommentByUidAsync(Guid commentUid)
        {
            var response = await _http.GetFromJsonAsync<CommentDto>($"api/admin/getcomment/uid/{commentUid}");
            return response;
        }

        // ✅ 3. Update a comment
        public async Task<bool> UpdateCommentAsync(CommentDto updatedComment)
        {
            var response = await _http.PutAsJsonAsync("api/admin/updatecomment", updatedComment);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteCommentAsync(Guid commentUid)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/admin/delete-comment/{commentUid}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Comment {commentUid} deleted successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"⚠️ Failed to delete comment: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception deleting comment: {ex.Message}");
                return false;
            }
        }
        public async Task<List<CommentDto>> GetRepliesAsync(Guid parentCommentUid)
        {
            try
            {
                var replies = await _http.GetFromJsonAsync<List<CommentDto>>(
                    $"api/admin/replies/{parentCommentUid}");

                return replies ?? new List<CommentDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP Error while fetching replies: {ex.Message}");
                return new List<CommentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error while fetching replies: {ex.Message}");
                return new List<CommentDto>();
            }
        }


    }
}
