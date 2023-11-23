namespace Lesson_33_MVC.Services;

public class DateTimeService : Interfaces.IDateTimeService
{
    public DateTime CurrentTime => DateTime.UtcNow;
}


public class DateTimeWithLoggerService : Interfaces.IDateTimeService
{
    private readonly ILogger<DateTimeWithLoggerService> _logger;

    public DateTimeWithLoggerService(ILogger<DateTimeWithLoggerService> logger)
    {
        _logger = logger;
        _logger.LogWarning("Date time service has been created.");
    }

    public DateTime CurrentTime {
        get {
            _logger.LogWarning("Current time is {time}", DateTime.UtcNow);
            return DateTime.UtcNow;
        }
    }
}