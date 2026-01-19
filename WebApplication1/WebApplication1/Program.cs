using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using WebApplication1.Data;
using WebApplication1.Helper;
using WebApplication1.Middlewares;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Mapea la sección "UserAuthSettings" a la clase UserAuthSettings
builder.Services.Configure<UserAuthSettings>(
    builder.Configuration.GetSection("UserAuthSettings"));


// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddControllers();

//database connection memory
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseInMemoryDatabase("CursoApisDb");
// });
//SQL Server connection
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
//     options.UseSqlServer(connectionString);


//database connection postgres
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
    options.UseNpgsql(connectionString);
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger generation
builder.Services.AddSwaggerGen(
    options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xamlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xamlPath);


    options.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Usuario y contraseña con *Basic Auth*."
    });

    options.AddSecurityRequirement(document =>new OpenApiSecurityRequirement
    {
       [new OpenApiSecuritySchemeReference("BasicAuth",document)] = []
    });


});


// enable CORS
var MyAllowOrigins = "MyAllowOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowOrigins,
        policy =>
        {
            // policy.AllowAnyOrigin()// allow all origins
            //       .AllowAnyMethod()
            //       .AllowAnyHeader();
            policy.WithOrigins("http://localhost:7133") // URL de tu frontend
                        .AllowAnyMethod()
                        .AllowAnyHeader();
        });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();


var app = builder.Build();

// Esta línea es clave:
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using CORS
app.UseCors(MyAllowOrigins);

app.UseHttpsRedirection();

app.UseBasicAuth();

app.UseAuthorization();

//Custom middleware could go here example
app.UseRequestLogging();
// app.Use(async(context,next) =>
// {
//     var path = context.Request.Path;
//     Console.WriteLine($"Request path: {path}");

//     await next.Invoke();

//     Console.WriteLine($"Response status code: {context.Response.StatusCode}");

// });

app.MapControllers();

app.Run();
