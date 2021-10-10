using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

[assembly: ApiController]

namespace WildRiftWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Authenticate

            AuthenticationSettings authenticationSettings = new();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });

            #endregion Authenticate

            services.AddControllers().AddFluentValidation();

            #region DbContex registration

            services.AddDbContext<WildRiftDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LolWildDbConnection")));

            #endregion DbContex registration

            services.AddAutoMapper(GetType().Assembly);

            #region Services

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IChampionService, ChampionService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IRuneService, RuneService>();

            #endregion Services

            #region Register Polly Policies

            services.ConfigurePollyPolicies(PollyPolicies.GetPolicies());

            #endregion Register Polly Policies

            #region Middlewares

            services.AddScoped<ErrorHandlingMiddleware>();

            #endregion Middlewares

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            #region Validators

            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<UpdateChampion>, UpdateChampionValidator>();
            services.AddScoped<IValidator<ChampionQuery>, ChampionQueryValidator>();
            services.AddScoped<IValidator<ItemQuery>, ItemQueryValidator>();
            services.AddScoped<IValidator<RuneQuery>, RuneQueryValidator>();

            #endregion Validators

            #region Context

            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();

            #endregion Context

            #region Swagger

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wild Rift API", Version = "v1" });
                });

            #endregion Swagger

            #region Corse

            services.AddCors(option =>
            {
                option.AddPolicy("FrontEndClient", builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader().WithOrigins(Configuration["AllowedOrigins"]);
                });
            });

            #endregion Corse
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseCors("FrontEndClient");

            #region Middlewares

            app.UseMiddleware<ErrorHandlingMiddleware>();

            #endregion Middlewares

            app.UseAuthentication();
            app.UseHttpsRedirection();

            #region Swagger

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wild Rift API v1"); });
            }

            #endregion Swagger

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}