using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    // A clean DTO for API responses
    public class HistoryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int[] BlogId { get; set; } = Array.Empty<int>();
        public DateTime ViewedAt { get; set; }
    }

}
