using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Reflection;
using System.Text;
using WildRiftWebAPI;

var builder = WebApplication.CreateBuilder(args);

// NLog
builder.Host.UseNLog();

#region Configure Services

// Authenticate

AuthenticationSettings authenticationSettings = new();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option =>
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

// Controllers and Versioning

builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Default;
});

// DbContex registration

builder.Services.AddDbContext<WildRiftDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("LolWildDbConnection")));

// AutoMapper

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Services

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IRuneService, RuneService>();

// Register Polly Policies

builder.Services.ConfigurePollyPolicies(PollyPolicies.GetPolicies(), PollyPolicies.GetAsyncPolicies());

// Middlewares

builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Password Hasher

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Validators

builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateChampion>, UpdateChampionValidator>();
builder.Services.AddScoped<IValidator<ChampionQuery>, ChampionQueryValidator>();
builder.Services.AddScoped<IValidator<ItemQuery>, ItemQueryValidator>();
builder.Services.AddScoped<IValidator<RuneQuery>, RuneQueryValidator>();

// Context

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

// Swager

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wild Rift API", Version = "v1" });
});

// Corse

builder.Services.AddCors(option =>
{
    option.AddPolicy("FrontEndClient", policyBuilder =>
    {
        policyBuilder.AllowAnyMethod().AllowAnyHeader().WithOrigins(builder.Configuration["AllowedOrigins"]);
    });
});

#endregion

var app = builder.Build();

#region Configure

app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontEndClient");

// Middlewares

app.UseMiddleware<ErrorHandlingMiddleware>();

// Authentication and Redirection
app.UseAuthentication();
app.UseHttpsRedirection();

// Swagger

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wild Rift API v1"); });
}

// Routing, Authorization and Endpoints

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

#endregion

app.Run();