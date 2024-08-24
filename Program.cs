using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var web = app.RunAsync("http://127.0.0.1:0");
var dir = Directory.CreateTempSubdirectory();
using var proc = Process.Start(new ProcessStartInfo
{
    FileName = new[] // https://github.com/zserge/lorca/blob/master/locate.go
    {
        @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
        @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
    }.First(File.Exists),
    Arguments = $"--app={app.Urls.First()} --window-size=480,640 --user-data-dir={dir}",// --enable-logging=stderr",
}) ?? throw new("Web app execution failure");

try { await Task.WhenAny(web, proc.WaitForExitAsync()); }
finally
{
    proc.CloseMainWindow();
    dir.Delete(true);
}