using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Options;
using STSY.Identity.Abstraction.Service;
using STSY.Identity.API.EndPoints;
using STSY.Identity.Example.ContractImplementation;
using STSY.Identity.JWT.Generators.Access;
using STSY.Identity.JWT.Generators.AccessTokens;
using STSY.Identity.JWT.Options;
using STSY.Microsoft.Identity.EndPoints;
using STSY.Microsoft.Identity.Extension;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.RequireHttpsMetadata = false;
    var key = builder.Configuration["AccessJWT:0:SecretKey"];
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(key)),
        ValidIssuer = "test",
        ValidAudience = "test"
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSTSYMicrosoftIdentity(identity =>
{
    identity.User.RequireUniqueEmail = true;
    identity.Password.RequiredUniqueChars = 3;
    identity.Password.RequiredLength = 8;
    identity.Password.RequireUppercase = true;
    identity.Password.RequireNonAlphanumeric = true;
    identity.Password.RequireDigit = true;
    identity.Password.RequireLowercase = true;

}, context =>
{
    context.UseMySQL(builder.Configuration.GetConnectionString("Identity"), c => c.MigrationsAssembly(Assembly.GetExecutingAssembly()));
});
builder.Services.AddScoped<ISendChallengeTokens, SendChallengeTokens>();
builder.Services.AddScoped<ISessionManager, StanderSessionCreator>();
builder.Services.AddSingleton(s => new RandomRefreshTokenOption { ExpireHours = 200, RefreshTokenSize = 64 });
builder.Services.AddScoped<IRefreshTokenGenerator, RandomTokenGenerator>();
builder.Services.AddSingleton<List<JWTAccessKeyOption>>(builder.Configuration.GetSection("AccessJWT").Get<List<JWTAccessKeyOption>>());
builder.Services.AddSingleton<List<JWTMFTokenOption>>(builder.Configuration.GetSection("MFJWT").Get<List<JWTMFTokenOption>>());
builder.Services.AddScoped<IMFTokenGenerator, JWTMFAccessTokenGenerator>();
builder.Services.AddScoped<IAccessTokenGenerator, JWTAccessTokenGenerator>();
builder.Services.AddScoped<IGetUserClaims, GetUserClaim>();
builder.Services.AddScoped<AuthenticatorFactory>();
builder.Services.AddScoped<STSYLogin>();

builder.Services.AddScoped<IGetCurrentAuthorizedUser, GetCurrentAuthorizedUser>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "STSY API v1");
        options.RoutePrefix = string.Empty; // Swagger at root URL
    });
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapSTSYAccountEndPoint("identity");
app.MapSTSYLoginEndPoint("identity");
app.MapSTSYMFASeupApis("identity");
app.MapSTSYPassKeyApis("identity");
app.Run();
