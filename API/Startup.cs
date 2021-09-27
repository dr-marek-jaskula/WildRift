using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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

            #endregion

            services.AddControllers().AddFluentValidation();

            #region DbContex registration
            services.AddDbContext<WildRiftDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LolWildDbConnection")));
            #endregion

            services.AddAutoMapper(GetType().Assembly); 
            
            #region Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IChampionService, ChampionService>();
            services.AddScoped<IItemService, ItemService>();
            #endregion

            #region Middlewares
            services.AddScoped<ErrorHandlingMiddleware>();
            #endregion

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            #region Validators
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<UpdateChampion>, UpdateChampionValidator>();
            services.AddScoped<IValidator<ChampionQuery>, ChampionQueryValidator>();
            #endregion

            #region Context
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            #endregion

            #region Swagger
            services.AddSwaggerGen();
            #endregion

            #region Corse
            services.AddCors(option =>
            {
                option.AddPolicy("FrontEndClient", builder => 
                {
                    builder.AllowAnyMethod().AllowAnyHeader().WithOrigins(Configuration["AllowedOrigins"]); 
                });
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseCors("FrontEndClient");

            #region Middlewares
            app.UseMiddleware<ErrorHandlingMiddleware>();
            #endregion

            app.UseAuthentication();
            app.UseHttpsRedirection();

            #region Swagger
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wild Rift API"); });
            }
            #endregion

            app.UseRouting(); 
            app.UseAuthorization(); 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
