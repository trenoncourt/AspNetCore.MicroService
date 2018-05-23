using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MicroService.Routing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using BodyParameter = Swashbuckle.AspNetCore.Swagger.BodyParameter;
using IParameter = Swashbuckle.AspNetCore.Swagger.IParameter;

namespace AspNetCore.MicroService.Swagger
{
    public class MicroServiceSwaggerGenerator : ISwaggerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private ISchemaRegistry _schemaRegistry;

        public MicroServiceSwaggerGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            var metadatas = _serviceProvider.GetService<MicroServiceMetadatas>();
            
            SchemaRegistryFactory schemaRegistryFactory = new SchemaRegistryFactory(new JsonSerializerSettings(), new SchemaRegistrySettings());
            _schemaRegistry = schemaRegistryFactory.Create();
            IDictionary<string, PathItem> paths = metadatas.RouteActionMetadatas
                .GroupBy(m => m.RelativePath)
                .ToDictionary(m => "/" + m.First().RelativePath, m =>
                    new PathItem
                    {
                        Get = CreateOperation(m, HttpMethods.Get),
                        Post = CreateOperation(m, HttpMethods.Post),
                        Put = CreateOperation(m, HttpMethods.Put),
                        Delete = CreateOperation(m, HttpMethods.Delete)
                        
                    });
            var swaggerDoc = new SwaggerDocument
            {
                Info = new Info(),
                Host = host,
                BasePath = basePath,
                Schemes = schemes,
                Paths = paths,
                Definitions = _schemaRegistry.Definitions,
                SecurityDefinitions = null,
                Security = null
            };
            
            return swaggerDoc;
        }

        private Operation CreateOperation(IEnumerable<RouteActionMetadata> metadatas, string httpMethod)
        {
            RouteActionMetadata metadata = metadatas.FirstOrDefault(m => m.HttpMethod == httpMethod);
            if (metadata == null) return null;

            string operationId = metadata.RelativePath.Capitalize();
            var operation = new Operation
            {
                OperationId = operationId,
                Tags = new List<string> {operationId},
                Responses = metadata.Output != null ? SuccessResponses(metadata.Output.Type) : SuccessResponses(),
                Produces = metadata.ContentTypes,
                Parameters = GetParameters(metadata)
            };
            
            return operation;
        }

        private Dictionary<string, Response> SuccessResponses()
        {
            return new Dictionary<string, Response>
            {
                {
                    "200",
                    new Response
                    {
                        Description = "Success"
                    }
                }
            };
        }

        private Dictionary<string, Response> SuccessResponses(Type type)
        {
            return new Dictionary<string, Response>
            {
                {
                    "200",
                    new Response
                    {
                        Description = "Success",
                        Schema = _schemaRegistry.GetOrRegister(type)
                    }
                }
            };
        }

        private Dictionary<string, Response> SuccessResponses<T>()
        {
            return new Dictionary<string, Response>
            {
                {
                    "200",
                    new Response
                    {
                        Description = "Success",
                        Schema = _schemaRegistry.GetOrRegister(typeof(T))
                    }
                }
            };
        }

        private List<IParameter> GetParameters(RouteActionMetadata metadata)
        {
            var parameters = new List<IParameter>();
            foreach (PathParameter pathParameter in metadata.Input.PathParameters)
            {
                Schema schema = _schemaRegistry.GetOrRegister(pathParameter.Type);
                parameters.Add(new NonBodyParameter
                {
                    Name = pathParameter.Name,
                    In = "path",
                    Required = pathParameter.Required,
                    Type = schema.Type,
                    Format = schema.Format,
                    Description = schema.Description,
                    Default = pathParameter.Default,
                    Minimum = pathParameter.Minimum,
                    Maximum = pathParameter.Maximum
                });
            }
            foreach (QueryParameter pathParameter in metadata.Input.QueryParameters)
            {
                Schema schema = _schemaRegistry.GetOrRegister(pathParameter.Type);
                parameters.Add(new NonBodyParameter
                {
                    Name = pathParameter.Name,
                    In = "query",
                    Required = pathParameter.Required,
                    Type = schema.Type,
                    Format = schema.Format,
                    Description = schema.Description,
                    Default = pathParameter.Default,
                    Minimum = pathParameter.Minimum,
                    Maximum = pathParameter.Maximum
                });
            }
            if (metadata.Input.BodyParameter != null)
            {
                Schema schema = _schemaRegistry.GetOrRegister(metadata.Input.BodyParameter.Type);
                parameters.Add(new BodyParameter
                {
                    Name = metadata.Input.BodyParameter.Name,
                    In = "body",
                    Required = true,
                    Schema = schema
                });
            }

            return parameters;
        }
    }
}