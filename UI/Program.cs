using Repository;
using UI.Components;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Components.Server.Circuits;
using UI;


var builder = WebApplication.CreateBuilder(args);


// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();


// Servicios propios
builder.Services.AddScoped<Repository.MateriaRepository>();
builder.Services.AddScoped<Services.MateriaService>();

builder.Services.AddSingleton<ProfesorRepository>();


// Detectar conexión/desconexión del navegador
builder.Services.AddSingleton<CircuitHandler, ClientCircuitHandler>();


var app = builder.Build();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();



app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Lifetime.ApplicationStarted.Register(() =>
{
    Task.Run(async () =>
    {
        await Task.Delay(1500);

        var server = app.Services.GetRequiredService<IServer>();

        var addresses = server
            .Features
            .Get<IServerAddressesFeature>()
            ?.Addresses;


        var url = addresses?
            .FirstOrDefault();


        if (!string.IsNullOrEmpty(url))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

    });
});


app.MapPost("/app-closed", () =>
{
    Task.Run(async () =>
    {
        await Task.Delay(1000);

        Environment.Exit(0);
    });

    return Results.Ok();
});
app.Run();