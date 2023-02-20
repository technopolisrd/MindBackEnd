using Microsoft.EntityFrameworkCore;
using MindBackEnd.Authorization.Contracts;
using MindBackEnd.Authorization;
using MindBackEnd.Helpers;
using MindBackEnd.Middleware;
using MindBackEnd.Services.Contracts;
using MindBackEnd.Services.Services;
using System.Text.Json.Serialization;
using Mind.Context.Data.Security;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MindBackEnd;
using Mind.Repository.Mind.Contracts;
using Mind.Repository.Mind.Repository;
using Mind.Repository.Mind.Service.Contracts;
using Mind.Repository.Mind.Service;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<MindContext>();
    services.AddCors(options =>
    {
        options.AddPolicy("Todos", builder => builder.WithOrigins("*").WithHeaders("*").WithMethods("*"));
    });
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    services.AddEndpointsApiExplorer();
    services.AddAutoMapper(typeof(Program));
    services.AddSwaggerGen();

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                        new HeaderApiVersionReader("x-api-version"),
                                                        new MediaTypeApiVersionReader("x-api-version"));
    });

    services.AddVersionedApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });

    services.ConfigureOptions<ConfigureSwaggerOptions>();

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IEmailService, EmailService>();

    // Repositories
    services.AddScoped<iCustomerRepository, CustomerRepository>();
    services.AddScoped<iTeamRepository, TeamRepository>();
    services.AddScoped<iLogMovementRepository, LogMovementRepository>();

    // Services
    services.AddScoped<iCustomerService, CustomerService>();
    services.AddScoped<iTeamService, TeamService>();
    services.AddScoped<iLogMovementService, LogMovementService>();


}

var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<MindContext>();
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // generated swagger json and swagger ui middleware
    app.UseSwagger();
    //app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET 6 Account Challange API for Grupo Mind"));

    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });

    // global cors policy
    app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();