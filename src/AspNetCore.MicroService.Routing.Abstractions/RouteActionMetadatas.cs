using System;
using System.Collections.Generic;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    public class RouteActionMetadata
    {
        public RouteActionMetadata()
        {
            Input = new Input();
            Output = new Output();
            ContentTypes = new List<string>();
        }
        
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
        public Output Output { get; set; }
        
        /// <summary>
        /// Gets or sets the input for this action.
        /// </summary>
        public Input Input { get; set; }
        
        public IList<string> ContentTypes { get; }
    }

    public class Output
    {
        public Type Type { get; set; }
    }

    public class Input
    {
        public Input()
        {
            PathParameters = new List<PathParameter>();
            QueryParameters = new List<QueryParameter>();
        }
        
        public IList<PathParameter> PathParameters { get; }
        
        public IList<QueryParameter> QueryParameters { get; }
        
        public BodyParameter BodyParameter { get; set; }
    }

    public interface IParameter
    {
        Type Type { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        bool Required { get; set; }

        object Default { get; set; }
        
        Dictionary<string, object> Extensions { get; }
    }

    public class Parameter : IParameter
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }
        
        public object Default { get; set; }

        public Dictionary<string, object> Extensions { get; }
    }

    public class UrlParameter : Parameter
    {
        public int? Maximum { get; set; }

        public bool? ExclusiveMaximum { get; set; }

        public int? Minimum { get; set; }
    } 

    public class BodyParameter : Parameter
    {
    } 

    public class QueryParameter : UrlParameter
    {
    }

    public class PathParameter : UrlParameter
    {
        public PathParameter()
        {
            Required = true;
        }
    }
}