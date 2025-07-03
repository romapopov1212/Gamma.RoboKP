using Gamma.RoboKP.Application.Extensions;
using Gamma.RoboKP.Domain.Options;
using Gamma.RoboKP.Extensions;
using Gamma.RoboKP.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

//TODO: при регистрации для подтверждения почты отправлять код на почту 
//TODO: при попытке входа в аккаунт, у которого не подтвержденный аккаунт

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

builder.Services.AddControllers();

builder.Services.RegisterMapster();

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


app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();