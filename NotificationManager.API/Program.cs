
using NotificationManager.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException("JWT Secret is not properly configured or too short!");
}
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

// Dependency Injection
builder.Services.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHttpClient<INotificationService, NotificationService>()
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check Endpoints
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
