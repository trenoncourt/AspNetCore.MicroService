using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MicroService.Routing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                Responses = metadata.ReturnType != null ? SuccessResponses(metadata.ReturnType) : SuccessResponses(),
                Produces = metadata.ContentTypes,
                Parameters = GetParameter(metadata.InputType, metadata.InputLocation)
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

        private List<IParameter> GetParameter(Type type, InputLocation inputLocation)
        {
            if (type == null) return null;
            if (inputLocation == InputLocation.Body)
            {
                return new List<IParameter>
                {
                    new BodyParameter
                    {
                        Name = type.Name,
                        In = inputLocation.ToString(),
                        Required = true,
                        Schema = _schemaRegistry.GetOrRegister(type)
                    }
                };
            }

            Schema schema = _schemaRegistry.GetOrRegister(type);
            return new List<IParameter>
            {
                new NonBodyParameter
                {
                    Name = "id",
                    In = inputLocation.ToLowerString(),
                    Required = inputLocation == InputLocation.Path ? true : false,
                    Type = schema.Type,
                    Format = schema.Format
                }
            };
        }
    }
}