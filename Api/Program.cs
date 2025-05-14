var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 2. Configuration
builder.Services.Configure<ExchangeApiOptions>(builder.Configuration.GetSection("ExchangeApi"));

// 3. EF Core
builder.Services.AddDbContext<ExchangeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. HttpClient + Polly retry
builder.Services.AddHttpClient<IExchangeRateClient, ExternalExchangeRateClient>()
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) }));

// 5. In‑memory rate limiting
builder.Services.AddSingleton<IApiKeyUsageTracker, InMemoryApiKeyUsageTracker>();

// 6. Controllers, versioning, Swagger
builder.Services.AddControllers();
builder.Services.AddApiVersioning(opts =>
{
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.DefaultApiVersion = new ApiVersion(1, 0);
    opts.ReportApiVersions = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CurrencyConverter API v1");
});

app.MapControllers();

app.Run();
