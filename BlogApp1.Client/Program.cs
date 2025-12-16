using ASService.Services;
using BlogApp1.Client;
using CAPService.Services;
using BSService.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using BlogApp1.Client.Services;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7244/") }); // Replace with your API base URL

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ImageApiService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<LibraryService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<EditorService>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<RAGHttpService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
await builder.Build().RunAsync();
