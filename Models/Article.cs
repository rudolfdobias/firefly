using System;

namespace Firefly.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }

        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }
}