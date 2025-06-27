using DiningVsCodeNew;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "EnableCORS",
        builder =>
        {
            builder.WithOrigins("http://localhost",
                "https://cafeteria.evercare.ng",
                "https://cafeteriaapi.evercare.ng",
                "https://cafeteriaapi.evercare.ng:2020",
                "https://cafeteriaapi.evercare.ng:2020/Onlinepayment",
                "https://cafeteria.evercare.ng:3030",
                "https://localhost:4200",
                "http://localhost:4200",
                "http://localhost:5057",
                "https://localhost:7230",
                "http://cafeteria.evercare.ng",
                "http://cafeteriaapi.evercare.ng",
                "http://10.20.20.101:5050",
                "http://10.20.20.101:4040",
                "http://10.20.20.104:2020",
                "http://localhost:90")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7146",
            ValidAudience = "https://localhost:7146",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
        };
    });

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Services.Configure<Setting>(builder.Configuration.GetSection("Setting"));
var setting = builder.Configuration.GetSection("Setting").Get<Setting>();
builder.Services.AddSingleton<Setting>(setting);
Setting.initializeRepoDb();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<MenuRepository>();
builder.Services.AddSingleton<VoucherRepository>();
builder.Services.AddSingleton<CustomerTypeRepository>();
builder.Services.AddSingleton<PaymentMainRepository>();
builder.Services.AddSingleton<PaymentDetailsRepository>();
builder.Services.AddSingleton<MenuRepository>();
builder.Services.AddSingleton<PaymentModeRepository>();
builder.Services.AddSingleton<OrderedMealRepository>();
builder.Services.AddSingleton<OnlinePaymentRepository>();
builder.Services.AddSingleton<ServedRepository>();
builder.Services.AddSingleton<CustomerRouteRepository>();
builder.Services.AddSingleton<MealtariffRepository>();
builder.Services.AddSingleton<SieveProcessor>();
builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
builder.Services.AddSingleton<SieveProcessor>();
builder.Services.AddSingleton<AvailableMealRepository>();
builder.Services.AddSingleton<MealActivityRepository>();
builder.Services.AddSingleton<TransferRepository>();
builder.Services.AddSingleton<FeedbackRepository>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("EnableCORS");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
