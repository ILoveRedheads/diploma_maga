using System;
using System.Net.Http;
using Blazored.LocalStorage;
using CSService.Blazor;
using CSService.Blazor.Services;
using CSService.Blazor.Services.Impl;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddSingleton<ICustomNavigationManager, CustomNavigationManager>();
builder.Services.AddSingleton<ITransmitter, Transmitter>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IHttpRepository, HttpRepository>();
builder.Services.AddScoped<IAuthHttpRepository, AuthHttpRepository>();
builder.Services.AddScoped<ICustomLocalStorageService, CustomLocalStorageService>();
builder.Services.AddScoped<IExaminerService, ExaminerService>();
builder.Services.AddScoped<ISceneService, SceneService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IVrHeadsetService, VrHeadsetService>();

await builder.Build().RunAsync();
