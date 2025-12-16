using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    public class NewBlog
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string AuhtorName { get; set;}
        public string AuhtorUID { get; set;}
        public List<string> Tags { get; set; } = new();
        public string Domain { get; set; }
        public string Status { get; set; }
        public string MetaDescription { get; set; }
    }
}
