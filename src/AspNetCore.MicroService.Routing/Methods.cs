using System;

namespace AspNetCore.MicroService.Routing
{
    [Flags]
    public enum Methods
    {
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8
    }
}