﻿using System.Diagnostics;
using System.Security.Cryptography;
using Auth.Infrastructure.CrossCutting.Extensions.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Auth.API;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.AddInfrastructureServices(Configuration);
        
        services.AddControllers(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());
        
        services.AddApplicationServices();
        
        var publicKey = Configuration["Jwt:PublicKeyXml"];
        var issuer = Configuration["Jwt:Issuer"];

        var publicRsa = RSA.Create();
        publicRsa.FromXmlString(publicKey!);
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = "all",
                    IssuerSigningKey = new RsaSecurityKey(publicRsa)
                };
                
                options.RequireHttpsMetadata = !Debugger.IsAttached;
            });

        services.AddAuthorization();
        
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
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hyzen Auth API v2"));

        app.UseCors(e => e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}