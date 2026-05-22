using AutoMapper;
using Microsoft.OpenApi;
using RestaurantManagement.API.Middlewares;
using RestaurantManagement.Application;
using RestaurantManagement.Application.Mappings;
using RestaurantManagement.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(AutoMapperProfile).Assembly);
});



builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();


//Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Resturant Management API",
        Version = "v1",
        Description = "API for managing restaurant orders, menu, reservations, etc."
    });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token. Example: Bearer asda212..."
    });

});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API V1");
    });
}

app.Run();
