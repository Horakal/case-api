using Microsoft.AspNetCore.Mvc;
using RestApiCase.Application.Logging.DTOs;
using RestApiCase.Domain.Logging.Interfaces;
using System.Globalization;
using System.Net;

namespace RestApiCase.Api.Controllers
{
    [ApiController]
    public class MetricsController(IRequestLogRepository logRepository) : ControllerBase
    {
        private static readonly DateTime _startedAt = DateTime.UtcNow;
        private readonly IRequestLogRepository _logRepository = logRepository;

        [HttpGet("/metrics")]
        [ProducesResponseType(typeof(MetricsResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<MetricsResponse>> GetMetrics()
        {
            var totalRequests = await _logRepository.GetTotalRequestsAsync();
            var averageResponseTime = await _logRepository.GetAverageResponseTimeMsAsync();
            var totalErrors = await _logRepository.GetTotalErrorsAsync();
            var requestsByMethod = await _logRepository.GetRequestsByMethodAsync();
            var requestsByStatusCode = await _logRepository.GetRequestsByStatusCodeAsync();

            var metrics = new MetricsResponse
            {
                TotalRequests = totalRequests,
                AverageResponseTimeMs = Math.Round(averageResponseTime, 2),
                TotalErrors = totalErrors,
                ErrorRate = totalRequests > 0
                    ? Math.Round((double)totalErrors / totalRequests * 100, 2)
                    : 0,
                RequestsByMethod = requestsByMethod,
                RequestsByStatusCode = requestsByStatusCode,
                UptimeSince = _startedAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
            };

            return Ok(metrics);
        }
    }
}
