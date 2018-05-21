using System;
using System.Collections.Generic;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    public class RouteActionMetadata
    {
        /// <summary>
        /// Gets or sets the supported HTTP method for this action.
        /// </summary>
        public string HttpMethod { get; set; }
        
        /// <summary>
        /// Gets or sets relative url path template (relative to application root) for this action.
        /// </summary>
        public string RelativePath { get; set; }
        
        /// <summary>
        /// Gets or sets the return type for this action.
        /// </summary>
        public Type ReturnType { get; set; }
        
        /// <summary>
        /// Gets or sets the input type for this action.
        /// </summary>
        public Type InputType { get; set; }

        public InputLocation InputLocation { get; set; }

        public List<string> ContentTypes { get; set; }
    }
}