using AibolitAPI.Auth;
using AibolitAPI.Data;
using AibolitAPI.Interfaces;
using AibolitAPI.Mappers;
using AibolitAPI.Middleware;
using AibolitAPI.Repositories;
using AibolitAPI.SearchProviders;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка logger-а запроса к базе данных
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("test-sql-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


// Настройка контекста данных для подключения к базе данных
builder.Services.AddDbContext<AibolitDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies(false)
        .LogTo(message => Log.Logger.Information(message), LogLevel.Information)
        .EnableSensitiveDataLogging());


// Настройка AutoMapper
builder.Services.AddAutoMapper(config => { config.AddProfile<MapperProfile>(); });


// Регистрация сервисов и репозиториев
builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<AdministratorService>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentService>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<DoctorService>();

builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<HospitalService>();

builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<MedicalRecordService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<PatientService>();

builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<PrescriptionService>();

builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<RecommendationService>();

builder.Services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
builder.Services.AddScoped<WorkScheduleService>();

builder.Services.AddScoped<IRoleValidator, RoleValidator>();

builder.Services.AddScoped<KeycloakService>();

builder.Services.AddHttpClient<ExternalApiSearchProvider>();
builder.Services.AddScoped<IDiseaseSearchProvider, ExternalApiSearchProvider>();
builder.Services.AddHttpClient<OpenAIDiseaseSearchProvider>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["OpenAI:ApiKey"]}");
    client.DefaultRequestHeaders.Add("Content-Type", "application/json");
});


builder.Services.AddScoped<DiseaseSearchService>();

// Настройка аутентификации Keycloak
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8081/realms/aibolit-api";
        options.Audience = "aibolit-api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8081/realms/aibolit-api",
            ValidateAudience = true,
            ValidAudience = "aibolit-api",
            ValidateLifetime = true,
            NameClaimType = "sub",
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseMiddleware<KeycloakMiddleware>();
app.UseAuthorization();
app.MapControllers();


app.MapControllers();

app.Run();