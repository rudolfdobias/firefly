using System;
using System.Collections.Generic;

namespace Firefly.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Article> Articles { get; set; } = new List<Article>();
    }
}