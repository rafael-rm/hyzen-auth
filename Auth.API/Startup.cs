using Auth.Infrastructure.CrossCutting.Extensions.IoC;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;

namespace Auth.API;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.AddControllers(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());
        
        services.AddAuthDbContext(Configuration);
        services.AddDomainServices();
        services.AddApplicationServices();
        services.AddInfrastructureServices(Configuration);
        
        // TODO: Avaliar filters
        /*services.AddControllers(options =>
        {
            options.Filters.Add(new CustomActionFilter());
            options.Filters.Add(new CustomExceptionFilterAttribute());
        });*/
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hyzen Auth API", Version = "v1" });
            
            c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });
				
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Authorization",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hyzen Auth API v1"));

        app.UseCors(e => e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}