using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;

namespace CAPService.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;

        public CustomAuthStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            var role = await _sessionStorage.GetItemAsync<string>("userRole");
            var userId = await _sessionStorage.GetItemAsync<string>("access_token");

            if (!string.IsNullOrWhiteSpace(role))
            {
                identity = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, userId),
                new Claim(ClaimTypes.Role, role)
            }, "apiauth_type");
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthenticationChanged() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
