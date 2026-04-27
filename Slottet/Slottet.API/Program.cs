using Microsoft.EntityFrameworkCore;
using Slottet.API.Controllers;
using Slottet.API.Middlewares;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure;
using Slottet.Infrastructure.Auditing;
using Slottet.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ShiftboardController>();

builder.Services.AddScoped<IAuditScope, AuditScope>();
builder.Services.AddScoped<AuditInterceptor>();

// Builder for EF Core
builder.Services.AddDbContext<SlottetDBContext>((ai, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(ai.GetRequiredService<AuditInterceptor>()));

builder.Services.AddCors(options => {
    options.AddPolicy("blazorApp", policyBuilder => {
        policyBuilder.WithOrigins("https://localhost:5001");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SlottetDBContext>();

    await context.Database.MigrateAsync();
    await DBSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();

app.UseMiddleware<AuditScopeMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.UseCors("blazorApp");

app.Run();
