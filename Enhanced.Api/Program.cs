using Enhanced.Services.AmazonServices;
using Enhanced.Services.Appeagle;
using Enhanced.Services.BQ;
using Enhanced.Services.BraintreeService;
using Enhanced.Services.PayPal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAppeagleService, AppeagleService>();
builder.Services.AddScoped<IPayPalReportService, PayPalReportService>();

builder.Services.AddScoped<IAmazonReportManager, AmazonReportManager>();
builder.Services.AddScoped<IPayPalReportManager, PayPalReportManager>();
builder.Services.AddScoped<IBraintreeReportManager, BraintreeReportManager>();

builder.Services.AddScoped<IBQOrderManager, BQOrderManager>();
builder.Services.AddScoped<IBQService, BQService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
