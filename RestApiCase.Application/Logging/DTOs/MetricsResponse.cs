namespace RestApiCase.Application.Logging.DTOs
{
    public class MetricsResponse
    {
        public long TotalRequests { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public long TotalErrors { get; set; }
        public double ErrorRate { get; set; }
        public Dictionary<string, long> RequestsByMethod { get; set; } = [];
        public Dictionary<int, long> RequestsByStatusCode { get; set; } = [];
        public string UptimeSince { get; set; } = string.Empty;
    }
}
