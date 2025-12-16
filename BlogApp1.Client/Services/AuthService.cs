using BlogApp1.Shared;
using Microsoft.JSInterop;
using Supabase.Gotrue;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASService.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }
        public async Task<bool> RegisterAsync(SignupModel request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/signup", request);
            return response.IsSuccessStatusCode;
        }
        public async Task<string> LoginAsync(SignInModel request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/signin", request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var user_details =  JsonSerializer.Deserialize<UserDetails>(content, options);
                    var userInfoResponse = await _http.GetAsync($"api/auth/user-info?uid={user_details.Uid}");
                    Console.WriteLine("check 1 passes");
                    if (userInfoResponse.IsSuccessStatusCode)
                    {
                        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<AuthResponse>();
                        Console.WriteLine("check 2 passes");
                        if(userInfo.Role.Contains(request.Role))
                        {
                            await _js.InvokeVoidAsync("sessionStorage.setItem", "access_token", user_details.AccessToken); // Replace with actual token
                            await _js.InvokeVoidAsync("sessionStorage.setItem", "email", userInfo.Email);
                            await _js.InvokeVoidAsync("sessionStorage.setItem", "name", userInfo.FullName);
                            await _js.InvokeVoidAsync("sessionStorage.setItem", "userRole", request.Role);
                            await _js.InvokeVoidAsync("sessionStorage.setItem", "uid", user_details.Uid);
                            return request.Role;
                        }
                        else
                        {
                            return "wrong role";
                        }
                    }
                    else
                    {
                        return "no user found";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Sign-in failed: {errorContent}");
                }
                else
                {
                    throw new Exception($"Sign-in failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return "error";
            }
        }
    }
    public class AuthResponse
    {
        public string Email {  get; set; }
        public string FullName  { get; set; }
        public string[] Role { get; set; }
    }
    public class UserDetails
    {
        public string AccessToken { get; set; }
        public Guid Uid { get; set; }
    }
}