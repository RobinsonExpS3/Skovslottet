using Microsoft.EntityFrameworkCore;
using Slottet.API.Controllers;
using Slottet.API.Middlewares;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure;
using Slottet.Infrastructure.Auditing;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ShiftboardController>();
builder.Services.AddScoped<ISpecialResponsibilityDTOService, SpecialResponsibilityDTOService>();
builder.Services.AddScoped<IResidentDTOService, ResidentDTOService>();
builder.Services.AddScoped<IStaffDTOService, StaffDTOService>();
builder.Services.AddScoped<IMedicineDTOService, MedicineDTOService>();
builder.Services.AddScoped<ISwapPhoneDTOService, SwapPhoneDTOService>();
builder.Services.AddScoped<IShiftBoardDTOService, ShiftBoardDTOService>();
builder.Services.AddScoped<IGroceryDayDTOService, GroceryDayDTOService>();
builder.Services.AddScoped<IPaymentMethodDTOService, PaymentMethodDTOService>();
builder.Services.AddScoped<IPhoneDTOService, PhoneDTOService>();
builder.Services.AddScoped<IAuditScope, AuditScope>();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<IAuditLogDTOService, AuditLogDTOService>();
builder.Services.AddScoped<IStaffPNDTOService, StaffPNDTOService>();

// Builder for EF Core
builder.Services.AddDbContext<SlottetDBContext>((ai, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(ai.GetRequiredService<AuditInterceptor>()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("blazorApp", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:5001");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });

    options.AddPolicy("blazorApp2", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:7201");
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

    await DBSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();

app.UseCors("blazorApp");
app.UseCors("blazorApp2");

app.UseMiddleware<AuditScopeMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
