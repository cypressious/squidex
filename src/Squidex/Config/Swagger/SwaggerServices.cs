﻿// ==========================================================================
//  SwaggerServices.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NodaTime;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.WebApi.Processors.Security;
using Squidex.Controllers.ContentApi.Generator;
using Squidex.Infrastructure;
using Squidex.Pipeline.Swagger;

namespace Squidex.Config.Swagger
{
    public static class SwaggerServices
    {
        public static void AddMySwaggerSettings(this IServiceCollection services)
        {
            services.AddSingleton(typeof(SwaggerOwinSettings), s =>
            {
                var urlOptions = s.GetService<IOptions<MyUrlsOptions>>().Value;

                var settings =
                    new SwaggerOwinSettings { Title = "Squidex API Specification", IsAspNetCore = false }
                        .ConfigurePaths(urlOptions)
                        .ConfigureSchemaSettings()
                        .ConfigureIdentity(urlOptions);

                return settings;
            });

            services.AddTransient<SchemasSwaggerGenerator>();
        }

        private static SwaggerOwinSettings ConfigureIdentity(this SwaggerOwinSettings settings, MyUrlsOptions urlOptions)
        {
            settings.DocumentProcessors.Add(
                new SecurityDefinitionAppender(Constants.SecurityDefinition, SwaggerHelper.CreateOAuthSchema(urlOptions)));

            settings.OperationProcessors.Add(new OperationSecurityScopeProcessor(Constants.SecurityDefinition));

            return settings;
        }

        private static SwaggerOwinSettings ConfigurePaths(this SwaggerOwinSettings settings, MyUrlsOptions urlOptions)
        {
            settings.SwaggerRoute = $"{Constants.ApiPrefix}/swagger/v1/swagger.json";

            settings.PostProcess = document =>
            {
                document.BasePath = Constants.ApiPrefix;
                document.Info.ExtensionData = new Dictionary<string, object>
                {
                    ["x-logo"] = new { url = urlOptions.BuildUrl("images/logo-white.png", false), backgroundColor = "#3f83df" }
                };
            };

            settings.MiddlewareBasePath = Constants.ApiPrefix;

            return settings;
        }

        private static SwaggerOwinSettings ConfigureSchemaSettings(this SwaggerOwinSettings settings)
        {
            settings.DefaultEnumHandling = EnumHandling.String;
            settings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;

            settings.TypeMappers = new List<ITypeMapper>
            {
                new PrimitiveTypeMapper(typeof(Instant), schema =>
                {
                    schema.Type = JsonObjectType.String;
                    schema.Format = JsonFormatStrings.DateTime;
                }),
                new PrimitiveTypeMapper(typeof(Language), s => s.Type = JsonObjectType.String),
                new PrimitiveTypeMapper(typeof(RefToken), s => s.Type = JsonObjectType.String)
            };

            settings.DocumentProcessors.Add(new XmlTagProcessor());

            settings.OperationProcessors.Add(new XmlTagProcessor());
            settings.OperationProcessors.Add(new XmlResponseTypesProcessor());

            return settings;
        }
    }
}
