using _4VGymAPIEF.Data;
using _4VGymAPIEF.Dtos;
using _4VGymAPIEF.Repositories;   
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ActivityTypeRepository>();
builder.Services.AddScoped<InstructorRepository>();
builder.Services.AddScoped<ActivityRepository>();

// Añadir los controladores (igual que en 4VGymAPI)
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errorList = context.ModelState
                .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                {
                    // Intentamos separar "1001|Mensaje de error"
                    var parts = error.ErrorMessage.Split('|');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int parsedCode))
                    {
                        return new ErrorResponse(parsedCode, parts[1]);
                    }
                    // Si no tiene delimitador o falla el parseo
                    return new ErrorResponse(9999, error.ErrorMessage);
                }))
                .ToList();

            return new BadRequestObjectResult(errorList);
        };
    });

// OpenAPI. Así en Swagger los DTO se llaman igual que en 4VGymAPI (ActivityTypeDto -> ActivityType)
builder.Services.AddOpenApi(options =>
{
    options.CreateSchemaReferenceId = jsonTypeInfo =>
    {
        string? id = OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
        return id != null && id.EndsWith("Dto") ? id[..^3] : id;
    };
});

// Entity Framework: el DbContext con la cadena de conexión de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlServer(connectionString));

// Repositorios: Scoped (uno por petición HTTP), igual que el DbContext. Descoméntalos cuando los crees
//builder.Services.AddScoped<ActivityTypeRepository>();
//builder.Services.AddScoped<InstructorRepository>();
//builder.Services.AddScoped<ActivityRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI (Swashbuckle), igual que en 4VGymAPI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RestAPI for 4VGym v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();