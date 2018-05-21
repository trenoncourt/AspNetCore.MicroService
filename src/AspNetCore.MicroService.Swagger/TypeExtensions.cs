using System;

namespace AspNetCore.MicroService.Swagger
{
    public static class TypeExtensions
    {
        public static string ToSwaggerType(this Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }

            return null;
        }
    }
}