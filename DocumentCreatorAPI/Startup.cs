using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DocumentCreatorAPI
{
    partial class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureDocumentCreatorServices(services);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader() // at least .WithExposedHeaders("Content-Disposition")
                        ;
                });
            });

            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "DocumentCreator API", 
                    Version = "v1",
                    Description = "An opinionated API for creating documents from JSON objects (e.g. other API payloads) based on Word templates and Excel formula expressions.",
                    TermsOfService = null,
                    Contact = new OpenApiContact()
                    {
                        Name = "Panos",
                        Email = null,
                        Url = new Uri("https://github.com/pkokki/DocumentCreator")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Use under MIT licence",
                        Url = new Uri("https://github.com/pkokki/DocumentCreator/blob/master/LICENSE.TXT")
                    }
                });

                // Swashbuckle.AspNetCore.Annotations
                c.EnableAnnotations();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(DocumentCreator.Core.ITemplateProcessor).Assembly.GetName().Name}.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(JsonExcelExpressions.IExpressionEvaluator).Assembly.GetName().Name}.xml"));
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.UseCamelCasing(true);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAny");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "dcfs", "files", "html")),
                RequestPath = "/html",
                EnableDirectoryBrowsing = false
            });

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new List<string> { "index.html", "default.html" }
            }); ;
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocumentCreator API v1");
                // To serve the Swagger UI at the app's root
                //c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
