using BarHand.API.Inventory.Domain.Repositories;
using BarHand.API.Inventory.Domain.Services;
using BarHand.API.Inventory.Persistence.Repositories;
using BarHand.API.Inventory.Services;
using BarHand.API.Security.Authorization.Handlers.Implementations;
using BarHand.API.Security.Authorization.Handlers.Interfaces;
using BarHand.API.Security.Authorization.Middleware;
using BarHand.API.Security.Authorization.Settings;
using BarHand.API.Security.Domain.Repositories;
using BarHand.API.Security.Domain.Services;
using BarHand.API.Security.Persistence;
using BarHand.API.Security.Services;
using BarHand.API.Notifications.Domain.Repositories;
using BarHand.API.Notifications.Domain.Services;
using BarHand.API.Notifications.Persistence.Repositories;
using BarHand.API.Notifications.Services;
using BarHand.API.Shared.Domain.Repositories;
using BarHand.API.Shared.Persistence.Contexts;
using BarHand.API.Shared.Persistence.Repositories;
using BarHand.API.Stores.Domain.Repositories;
using BarHand.API.Stores.Domain.Services;
using BarHand.API.Stores.Persistence.Repositories;
using BarHand.API.Stores.Services;
using BarHand.API.Suppliers.Domain.Repositories;
using BarHand.API.Suppliers.Domain.Services;
using BarHand.API.Suppliers.Persistence.Repositories;
using BarHand.API.Suppliers.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    // Add API Documentation Information

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "BarHand API",
        Description = "BarHand RESTful API",
        TermsOfService = new Uri("https://barhand.github.io/Landing-Page-BarHand/"),
        Contact = new OpenApiContact
        {
            Name = "BarHand",
            Url = new Uri("https://barhand.github.io/Landing-Page-BarHand/")
        },
        License = new OpenApiLicense
        {
            Name = "BarHand Resources License",
            Url = new Uri("https://barhand.github.io/Landing-Page-BarHand/")
        }
    });
    options.EnableAnnotations();
    options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth"}
            },
            Array.Empty<string>()
        }
    });
}
);

//Add Data Base Connection

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(
    options=>options.UseMySQL(connectionString)
        .LogTo(Console.WriteLine,LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

//LowerCase URLs configuration
builder.Services.AddRouting(options => options.LowercaseUrls = true);

//Dependency Injection Configuration

// Shared Bounded Context Injection Configuration

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
 
//Inventory Bounded Context Injection Configuration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
//Suppliers
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
//Stores
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();
// Security Injection Configuration
builder.Services.AddScoped<IJwtHandler, JwtHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

//Auto Mapper Configuration
builder.Services.AddAutoMapper(
    typeof(BarHand.API.Mapping.ModelToResourceProfile),
    typeof(BarHand.API.Mapping.ResourceToModelProfile),
    typeof(BarHand.API.Security.Mapping.ModelToResourceProfile),
    typeof(BarHand.API.Security.Mapping.ResourceToModelProfile));

//AppSettingConfiguration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

//Application built
//Notifications
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

//Validation for ensuring Database Objects area created

using (var scope=app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<AppDbContext>())
{
    context.Database.EnsureCreated();
}


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        //options.RoutePrefix = string.Empty;
    });

//}
// Configure CORS

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure Error Handler Middleware

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure JWT Handling Middleware

app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();