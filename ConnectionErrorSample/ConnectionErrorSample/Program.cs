using ConnectionErrorSample.Client.Pages;
using ConnectionErrorSample.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = System.IO.Path.Join(path, "blogging.db");

builder.Services.AddDbContext<BloggingContext>(options =>
      options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddHttpClient("WebAPI", client => client.BaseAddress = new Uri("http://localhost:5278"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/blogs", async (BloggingContext context, CancellationToken cancellationToken) =>
{
    await Task.Delay(200);
    var blogs = await context.Blogs.Select(blog => new BlogViewModel { BlogId = blog.BlogId, Url = blog.Url }).ToListAsync(cancellationToken);
    return blogs;
});

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ConnectionErrorSample.Client._Imports).Assembly);

app.Run();
