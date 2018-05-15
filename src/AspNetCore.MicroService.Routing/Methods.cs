using System;

namespace AspNetCore.MicroService.Routing
{
    [Flags]
    public enum Methods
    {
        Get,
        Post,
        Put,
        Delete
    }
}