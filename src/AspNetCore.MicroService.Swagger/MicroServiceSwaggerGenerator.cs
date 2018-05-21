using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCore.MicroService.Swagger
{
    public class MicroServiceSwaggerGenerator : ISwaggerProvider
    {
        private static IDictionary<string, PathItem> _paths;

        internal static void AddPaths(IDictionary<string, PathItem> paths)
        {
            _paths = paths;
        }
        
        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            var swaggerDoc = new SwaggerDocument
            {
                Info = new Info(),
                Host = host,
                BasePath = basePath,
                Schemes = schemes,
                Paths = _paths,
                Definitions = new Dictionary<string, Schema>(),
                SecurityDefinitions = null,
                Security = null
            };
            return swaggerDoc;
        }
    }
}