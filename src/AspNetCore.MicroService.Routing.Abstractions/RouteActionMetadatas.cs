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

    public interface IParameter
    {
        Type Type { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        bool Required { get; set; }
        
        Dictionary<string, object> Extensions { get; }
    }

    public class Parameter : IParameter
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }
        
        public Dictionary<string, object> Extensions { get; }
    }

    public class UrlParameter : Parameter
    {
        public int? Maximum { get; set; }

        public bool? ExclusiveMaximum { get; set; }

        public int? Minimum { get; set; }
    } 

    public class QueryParameter : UrlParameter
    {
    }

    public class PathParameter : UrlParameter
    {
    }
}