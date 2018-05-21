using System.Collections.Generic;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    public class MicroServiceMetadatas
    {
        public MicroServiceMetadatas()
        {
            RouteActionMetadatas = new List<RouteActionMetadata>();
        }
        
        public List<RouteActionMetadata> RouteActionMetadatas { get; }
    }
}