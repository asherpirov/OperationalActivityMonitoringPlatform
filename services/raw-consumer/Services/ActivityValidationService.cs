using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RawConsumer.Models;


namespace RawConsumer.Services;

public class ActivityValidationService
{
    private readonly ILogger<ActivityValidationService> _logger;
    public ActivityValidationService(ILogger<ActivityValidationService> logger)
    {
        _logger = logger;
    }

    public ActivityEvent? Validate(string jsonMessage)
    {
        try
        {
            var reading = JsonSerializer.Deserialize<ActivityReading>(jsonMessage);
            
            if (reading == null)
            {
                _logger.LogWarning("Invalid Json message.");
                return null;
            }

            
            if (string.IsNullOrWhiteSpace(reading.EventId))
            {
                _logger.LogWarning("EventId is missing.");
                return null;
            }
   
            if (string.IsNullOrWhiteSpace(reading.SourceId))
            {
                _logger.LogWarning("SourceId is missing.");
                return null;
            }    

            if (!DateTime.TryParse(reading.Timestamp, out DateTime timestamp))
            {
                _logger.LogWarning("Invalid timestamp: {Timestamp}",reading.Timestamp);
                return null;
            }

            if (!double.TryParse(reading.Value, out double value))
            {
                _logger.LogWarning("Invalid value: {Value}",reading.Value);
                return null;
            }
                
                var validActivity = new ActivityEvent
                {
                    EventId = reading.EventId,
                    SourceId = reading.SourceId,
                    Timestamp = timestamp,
                    Value = value
                };
                return validActivity;
        }         
             
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process ActivityEvent message.");
            return null;
        }
    }
}