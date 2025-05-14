

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configuration
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// DbContext
builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseInMemoryDatabase("CurrencyDb"));

// HttpClient with retry and rate limiting handler
builder.Services.AddHttpClient<IExchangeRateClient, ExternalExchangeRateClient>()
    .AddPolicyHandler(PolicyExtensions.GetRetryPolicy())
    .AddHttpMessageHandler<ApiKeyRateLimitingHandler>();

// DI registrations
builder.Services.AddScoped<IExchangeRateClient, ExternalExchangeRateClient>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddHostedService<RealTimeRateBackgroundService>();

// Controllers, Swagger, Validation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Converter API", Version = "v1" });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
