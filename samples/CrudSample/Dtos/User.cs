using System;

namespace CrudSample.Dtos
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ushort Age { get; set; }

        public string Email { get; set; }
    }
}