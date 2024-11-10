using Microsoft.OpenApi.Models;
using VFX.Application.Common;

namespace VFX.WebAPI.Extensions
{
    // Extension methods for setting up Swagger in the WebAPI project.
    // This class configures the OpenAPI documentation settings for the API.
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerOpenAPI(this IServiceCollection services, AppSettings appSettings)
        {
            // Add Swagger services to the service collection
            services.AddSwaggerGen(options =>
            {
                // Define the Swagger document (API specification)
                options.SwaggerDoc("OpenAPISpecification", new OpenApiInfo
                {
                    Title = appSettings.ApplicationDetail.ApplicationName,

                    Version = "v1",

                    Description = appSettings.ApplicationDetail.Description,

                    Contact = new OpenApiContact
                    {
                        Email = "amin.marghzari@gmail.com",
                        Name = "Amin Marghzari",
                        Url = new Uri(appSettings.ApplicationDetail.ContactWebsite),
                    },

                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
            });

            return services;
        }
    }
}
