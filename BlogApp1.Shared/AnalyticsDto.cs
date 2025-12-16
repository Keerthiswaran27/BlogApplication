using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    // Represents analytics/statistics for reader
    public class AnalyticsDto
    {
        public int TotalRead { get; set; }        // Total blogs read
        public int TotalSaved { get; set; }
        public int TotalLiked { get; set; }
        public Dictionary<string, int> CategoryBreakdown { get; set; } = new(); // Category -> Count
        public int ReadingStreakDays { get; set; } // Optional: consecutive days reading
    }

}
