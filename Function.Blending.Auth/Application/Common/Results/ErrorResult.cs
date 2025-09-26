using System;
using System.Collections.Generic;

namespace Function.Blending.Auth.Application.Common.Results
{
    public class ErrorResult
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; } = string.Empty;
        public Dictionary<string, object>? AdditionalData { get; set; }

        public static ErrorResult Create(string code, string message, string? details = null, string? traceId = null)
        {
            return new ErrorResult
            {
                Code = ExtractErrorCode(message),
                Message = message,
                Details = details,
                TraceId = traceId ?? Guid.NewGuid().ToString("N")[..8]
            };
        }

        public static ErrorResult FromException(Exception ex, string? traceId = null)
        {
            return new ErrorResult
            {
                Code = "ERR_001",
                Message = "Error interno del servidor",
                Details = ex.Message,
                TraceId = traceId ?? Guid.NewGuid().ToString("N")[..8]
            };
        }

        private static string ExtractErrorCode(string message)
        {
            if (string.IsNullOrEmpty(message)) return "UNKNOWN";
            
            var colonIndex = message.IndexOf(':');
            return colonIndex > 0 ? message[..colonIndex] : "UNKNOWN";
        }
    }
}