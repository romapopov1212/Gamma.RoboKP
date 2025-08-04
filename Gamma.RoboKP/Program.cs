using Gamma.RoboKP.Application.Extensions;
using Gamma.RoboKP.Domain.Options;
using Gamma.RoboKP.Extensions;
using Gamma.RoboKP.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.PowerPoint;

//TODO: при регистрации для подтверждения почты отправлять код на почту 
//TODO: при попытке входа в аккаунт, у которого не подтвержденный аккаунт

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

builder.Services.AddControllers();

builder.Services.RegisterMapster();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});


builder
    .AddBearerAuthentication()
    .AddOptions()
    .AddData()
    .AddSwagger()
    .AddApplicationServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoboKpDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowSpecific");

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(token) && 
        !context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }

    await next();
});


app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();