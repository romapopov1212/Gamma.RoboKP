using System.Text;
using Gamma.RoboKP.Application.Extensions;
using Gamma.RoboKP.Application.Services;
using Gamma.RoboKP.Domain.Abstractions.Auth;
using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Enums;
//using Gamma.RoboKP.Domain.Models;
using Gamma.RoboKP.Domain.Options;
using Gamma.RoboKP.Infrastructure.Context;
using Gamma.RoboKP.Infrastructure.Models;
using Gamma.RoboKP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Gamma.RoboKP.Extensions;

public static class ServiceCollectionsExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Rutok Download Video",
                Version = "v1",
            });
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return builder;
    }
    
    public static WebApplicationBuilder AddData(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<RoboKpDbContext>(option =>
        {
            option.UseNpgsql(builder.Configuration.GetConnectionString("RoboKpConnection"));
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITokenRepository, TokenRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
        builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();
        
        
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddTransient<IMailService, MailService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
        builder.Services.AddScoped<IDiscountService, DiscountService>();
        builder.Services.AddScoped<ICartService, CartService>();
        
        return builder;
    }

    public static WebApplicationBuilder AddBearerAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x => // cтандартный метод
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // аутентификация 
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // авторизация
            })
            .AddJwtBearer(x => // добавляем эту схему
            
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = // для валидации токена
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(builder.Configuration["Authentication:TokenPrivateKey"]!)),
                    
                    ValidIssuer = "test",
                    ValidAudience = "test",
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var tokenFromCookie = context.HttpContext.Request.Cookies["access_token"];
                        
                        if (!string.IsNullOrEmpty(tokenFromCookie))
                        {
                            context.Token = tokenFromCookie;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddAuthorization(options => // роли
        {
            options.AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin.ToString()));
            options.AddPolicy("ManagerGamma", policy => policy.RequireRole(UserRole.Manager.ToString()));
            options.AddPolicy("ManagerPartner", policy => policy.RequireRole(UserRole.ManagerPartner.ToString()));
        });
       // builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<RoboKpDbContext>()
            .AddUserManager<UserManager<AppUser>>()
            .AddUserStore<UserStore<AppUser, IdentityRoleEntity, RoboKpDbContext, long>>();
        
        return builder;
    }

    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));
        
        return builder;
    }

}