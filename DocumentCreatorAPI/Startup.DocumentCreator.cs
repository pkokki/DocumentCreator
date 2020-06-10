using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Azure;
using DocumentCreator.Core.Repository;
using DocumentCreator.Core.Settings;
using DocumentCreator.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCreatorAPI
{
    partial class Startup
    {
        private const string REPO_FILE_SYSTEM = "FileSystem";
        private const string REPO_AZURITE = "Azurite";
        private const string REPO_AZURE_BLOB = "AzureBlob";

        private void ConfigureDocumentCreatorServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton(new GlobalSettings()
            {
                DocumentRepositoryType = REPO_FILE_SYSTEM,
                HtmlRepositoryType = REPO_FILE_SYSTEM
            });

            services.AddScoped(sp => BuildDocumentRepository(sp));
            services.AddScoped(sp => BuildHtmlRepository(sp));

            services.AddScoped<ITemplateProcessor, TemplateProcessor>();
            services.AddScoped<IMappingProcessor, MappingProcessor>();
            services.AddScoped<IDocumentProcessor, DocumentProcessor>();
            services.AddScoped<IMappingExpressionEvaluator, MappingExpressionEvaluator>();
        }

        private string _baseUrl;
        private string GetBaseUrl(IServiceProvider sp)
        {
            if (_baseUrl == null)
            {
                var context = sp.GetService<IHttpContextAccessor>();
                var request = context.HttpContext.Request;
                _baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            }
            return _baseUrl;
        }

        private IRepository BuildDocumentRepository(IServiceProvider sp)
        {
            var settings = sp.GetService<GlobalSettings>();
            return settings.DocumentRepositoryType switch
            {
                REPO_FILE_SYSTEM => new FileRepository(Env.ContentRootPath),
                REPO_AZURITE => new AzuriteRepository(),
                REPO_AZURE_BLOB => new AzureBlobRepository(Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")),
                _ => throw new InvalidOperationException("Unknown IRepository!"),
            };
        }

        private IHtmlRepository BuildHtmlRepository(IServiceProvider sp)
        {
            var settings = sp.GetService<GlobalSettings>();
            return settings.HtmlRepositoryType switch
            {
                REPO_FILE_SYSTEM => new HtmlFileRepository(Env.ContentRootPath, GetBaseUrl(sp)),
                REPO_AZURE_BLOB => new AzureBlobStaticHtmlRepository(
                    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"),
                    Environment.GetEnvironmentVariable("AZURE_STORAGE_STATIC_WEBSITE")),
                _ => throw new InvalidOperationException("Unknown IHtmlRepository!"),
            };
        }
    }
}
