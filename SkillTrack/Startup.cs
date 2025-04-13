using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using StackExchange.Redis;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors;
using SkillTrack.Core.Services;
using SkillTrack.Core.Services.Redis; 

namespace SkillTrack
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public bool UseRedis { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            // Определяем, использовать ли Redis
            string redisConnectionString = Configuration.GetConnectionString("Redis");
            UseRedis = !string.IsNullOrEmpty(redisConnectionString);
        }   

        public void ConfigureServices(IServiceCollection services)
        {
            // Добавляем контроллеры с настройками JSON
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Игнорировать циклические ссылки
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    
                    // Игнорировать null-значения
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    
                    // CamelCase для имен свойств
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    
                    // Нечувствительность к регистру при десериализации
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            
            // Настраиваем Redis только если он доступен
            if (UseRedis)
            {
                Console.WriteLine("Настройка Redis...");
                try 
                {
                    var redisConnectionString = Configuration.GetConnectionString("Redis");
                    services.AddSingleton<IConnectionMultiplexer>(sp =>
                    {
                        var options = ConfigurationOptions.Parse(redisConnectionString);
                        options.ConnectTimeout = 10000; 
                        options.SyncTimeout = 10000;
                        options.AbortOnConnectFail = false; 
                        return ConnectionMultiplexer.Connect(options);
                    });
                    services.AddScoped<IRedisService, RedisService>();
                    services.AddScoped<RedisService>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка настройки Redis: {ex.Message}");
                    Console.WriteLine("Продолжаем без Redis...");
                    UseRedis = false;
                    services.AddSingleton<IRedisService, InMemoryRedisService>();
                }
            }
            else
            {
                Console.WriteLine("Redis не настроен. Используем резервную службу в памяти.");
                services.AddSingleton<IRedisService, InMemoryRedisService>();
            }
                
            // services.AddSingleton<SupabaseClient>(serviceProvider =>
            // {
            //     var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            //     var supabaseConfig = configuration.GetSection("Supabase");

            //     var supabaseClient = new SupabaseClient(); 

            //     return supabaseClient;
            // });
            
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? "defaultkeyforunittests"))
                };
            });
            
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
            
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddMemoryCache();
            
                // NSwag/OpenAPI configuration
                services.AddOpenApiDocument(document =>
                {
                    document.Title = "MedExpo Auth Service API";
                    document.Version = "v1";
                    document.Description = "Authentication and authorization service for MedExpo platform";
                    
                    // Configure base path and API info
                    document.PostProcess = d =>
                    {
                        d.Servers.Add(new OpenApiServer { Url = "/" });
                    };
                    
                    // Ensure versioned paths
                    document.OperationProcessors.Add(new OperationProcessor(context =>
                    {
                        var path = context.OperationDescription.Path;
                        if (!path.Contains("/v1/"))
                        {
                            context.OperationDescription.Path = path.Replace("/api/", "/api/v1/");
                        }
                        return true;
                    }));
                });
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            }); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            // Настройка CORS - используйте именованную политику
            app.UseCors("AllowAll");
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            // Добавляем аутентификацию и авторизацию
            app.UseAuthentication();
            app.UseAuthorization();
            
            // NSwag middleware - must come after routing but before endpoints
            app.UseOpenApi();
            app.UseSwaggerUi(c => 
            {
                c.DocumentPath = "/swagger/v1/swagger.json";
                c.Path = "/swagger";
            });
            
            app.UseResponseCompression();
            
            // Ensure static files are served
            app.UseStaticFiles();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Index}/{id?}");
                
                // Явно добавляем маппинг для API контроллеров
                endpoints.MapControllers();
                
                // Редирект с корня на Swagger
                endpoints.MapGet("/", context => {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            });
        }
    }
}