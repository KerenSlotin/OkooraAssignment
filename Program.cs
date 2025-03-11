var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRateFetcherProvider, RateFetcherService>();
builder.Services.AddHostedService<RateFetcherService>();
builder.Services.AddSingleton<IDataStoreHandler>(provider => new FileHandler("exchangeRates.json"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseRouting(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();

