using System;
using AspNetCore.MicroService.Routing.Abstractions;
using AspNetCore.MicroService.Routing.Abstractions.Builder;

namespace AspNetCore.MicroService.Routing.Metadatas
{
    public static class RouteBuilderMetadataExtensions
    {
        public static void AddMetadata(this IRouteBuilder routeBuilder, Action<RouteActionMetadata> handler)
        {
            if (!routeBuilder.Settings.EnableMetadatas) return;
            
            var routeMetadata = new RouteActionMetadata();
            handler?.Invoke(routeMetadata);
            routeBuilder.Metadatas.Add(routeMetadata);
        }
    }
}