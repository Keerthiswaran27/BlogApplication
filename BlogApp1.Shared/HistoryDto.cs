using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    [Table("history")]
    public class HistoryDto : Supabase.Postgrest.Models.BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("blog_id")]
        public int[] BlogId { get; set; }

        [Column("viewed_at")]
        public DateTime ViewedAt { get; set; }
    }
}
