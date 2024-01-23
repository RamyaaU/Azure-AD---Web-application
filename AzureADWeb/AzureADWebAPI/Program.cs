using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//adding authentication and scheme is used is jwt 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        //this is the issuer that we get from json parser from meta doc
        options.Authority = "https://login.microsoftonline.com/3145f25f-fecb-4ea2-b291-7a035ce927d2/v2.0";
        //this is the application id uri of the ad
        options.Audience = "https://azureeADLearning.onmicrosoft.com/6cceb20d-ff90-4d3a-b719-d74fb3d1476b";
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
