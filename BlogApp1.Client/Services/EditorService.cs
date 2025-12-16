using BlogApp1.Shared.EditorModels;
using Microsoft.JSInterop;
using Nextended.Core.Extensions;
using System.Net.Http.Json;

namespace BlogApp1.Client.Services
{
    public class EditorService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private Guid? _cachedEditorUid; // Cached UID for reuse

        public EditorService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        // 🧠 Helper: Get UID from sessionStorage
        private async Task<Guid?> GetEditorUidAsync()
        {
            try
            {
                if (_cachedEditorUid.HasValue)
                    return _cachedEditorUid;

                var uidString = await _js.InvokeAsync<string>("sessionStorage.getItem", "uid");

                if (!string.IsNullOrEmpty(uidString) && Guid.TryParse(uidString, out Guid uid))
                {
                    _cachedEditorUid = uid;
                    return uid;
                }

                Console.WriteLine("[EditorService] Editor UID not found in sessionStorage.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetEditorUidAsync] Error reading UID: {ex.Message}");
                return null;
            }
        }

        // ==============================
        // 1️⃣ Get Blogs by Status
        // ==============================
        public async Task<List<BlogSummaryModel>> GetBlogsAsync(string status = "pending")
        {
            try
            {
                var blogs = await _http.GetFromJsonAsync<List<BlogSummaryModel>>($"api/editor/blogs?status={status}");
                return blogs ?? new List<BlogSummaryModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetBlogsAsync] Error: {ex.Message}");
                return new List<BlogSummaryModel>();
            }
        }

        // ==============================
        // 2️⃣ Get Blog Details by ID
        // ==============================
        public async Task<BlogDetailModel?> GetBlogByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<BlogDetailModel>($"api/editor/blog/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetBlogByIdAsync] Error: {ex.Message}");
                return null;
            }
        }

        // ==============================
        // 3️⃣ Approve Blog (Auto Editor UID)
        // ==============================
        public async Task<bool> ApproveBlogAsync(int blogId, string reviewComments,BlogDetailModel blog)
        {
            try
            {
                var editorUid = await _js.InvokeAsync<string>("sessionStorage.getItem", "uid");


                blog.EditorUid = editorUid;
                    blog.ReviewComments = reviewComments;
                

                var response = await _http.PostAsJsonAsync($"api/editor/approve/{blogId}", blog);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving blog: {ex.Message}");
                return false;
            }
        }


        // ==============================
        // 4️⃣ Reject Blog (Auto Editor UID)
        // ==============================
        public async Task<bool> RejectBlogAsync(int id, string rejectionReason)
        {
            try
            {
                var uid = await _js.InvokeAsync<string>("sessionStorage.getItem", "uid");
                var payload = new
                {
                    EditorUid = uid,
                    RejectionReason = rejectionReason
                };

                var response = await _http.PostAsJsonAsync($"api/editor/reject/{id}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:RejectBlogAsync] Error: {ex.Message}");
                return false;
            }
        }


        // ==============================
        // 5️⃣ Send Back for Revision (Auto UID)
        // ==============================
        public async Task<bool> SendBackForRevisionAsync(int id, string authorUid, string message, string feedbackType = "general")
        {
            try
            {
                var uid = await GetEditorUidAsync();
                if (uid == null) throw new Exception("Editor UID not found.");

                var feedback = new FeedbackModel
                {
                    BlogId = id,
                    AuthorUid = authorUid,
                    EditorUid = uid.ToString(),
                    FeedbackType = feedbackType,
                    Message = message,
                    IsAuthorVisible = true
                };

                var response = await _http.PostAsJsonAsync($"api/editor/revise/{id}", feedback);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:SendBackForRevisionAsync] Error: {ex.Message}");
                return false;
            }
        }

        // ==============================
        // 6️⃣ Update Blog Content
        // ==============================
        public async Task<bool> UpdateContentAsync(int id, string newContent,BlogDetailModel blog)
        {
            try
            {
                var uid = await GetEditorUidAsync();
                if (uid == null) throw new Exception("Editor UID not found.");

                var revision = new RevisionModel
                {
                    BlogId = id,
                    Content = newContent,
                    EditorUid = uid.ToString(),
                    VersionNo = 0,
                    CreatedAt = DateTime.UtcNow,
                    IsCurrent = true
                };

                var response = await _http.PostAsJsonAsync($"api/editor/update-content/{id}", revision);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:UpdateContentAsync] Error: {ex.Message}");
                return false;
            }
        }

        // ==============================
        // 7️⃣ Get Revisions
        // ==============================
        public async Task<List<RevisionModel>> GetRevisionsAsync(int blogId)
        {
            try
            {
                var revisions = await _http.GetFromJsonAsync<List<RevisionModel>>($"api/editor/revisions/{blogId}");
                return revisions ?? new List<RevisionModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetRevisionsAsync] Error: {ex.Message}");
                return new List<RevisionModel>();
            }
        }

        // ==============================
        // 8️⃣ Restore Revision
        // ==============================
        public async Task<bool> RestoreRevisionAsync(int versionId)
        {
            try
            {
                var response = await _http.PostAsync($"api/editor/revisions/restore/{versionId}", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:RestoreRevisionAsync] Error: {ex.Message}");
                return false;
            }
        }

        // ==============================
        // 9️⃣ Get Feedback for Blog
        // ==============================
        public async Task<List<FeedbackModel>> GetFeedbackAsync(int blogId)
        {
            try
            {
                var feedback = await _http.GetFromJsonAsync<List<FeedbackModel>>($"api/editor/feedback/{blogId}");
                return feedback ?? new List<FeedbackModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetFeedbackAsync] Error: {ex.Message}");
                return new List<FeedbackModel>();
            }
        }

        // ==============================
        // 🔟 Get Editor Analytics (Auto UID)
        // ==============================
        public async Task<AnalyticsModel?> GetAnalyticsAsync()
        {
            try
            {
                var uid = await GetEditorUidAsync();
                if (uid == null) throw new Exception("Editor UID not found.");

                return await _http.GetFromJsonAsync<AnalyticsModel>($"api/editor/analytics/{uid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetAnalyticsAsync] Error: {ex.Message}");
                return null;
            }
        }
        public async Task<DashboardStatsModel?> GetDashboardStatsAsync()
        {
            try
            {
                var stats = await _http.GetFromJsonAsync<DashboardStatsModel>("api/editor/dashboardstats");
                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditorService:GetDashboardStatsAsync] Error: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> UpdateBlogAsync(BlogDetailModel blog)
        {
            try
            {
                if (blog == null || blog.Id <= 0)
                    return false;

                var response = await _http.PostAsJsonAsync($"api/editor/update/{blog.Id}", blog);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateBlogAsync] Error: {ex.Message}");
                return false;
            }
        }


    }
}
