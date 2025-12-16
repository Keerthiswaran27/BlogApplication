using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string CoverImageUrl { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUid { get; set; }
        public string Domain { get; set; }
        public List<string> Tags { get; set; }
        public int ViewCount { get; set; }
            
        public int LikesCount { get; set; }
        public string Status { get; set; }
        public string MetaDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public long? ReadingTime { get; set; }
    }
}
