
using PatternPal.LoggingServer.Data;
using Quartz;

namespace PatternPal.LoggingServer.LogJobs;
/// <summary>
/// Job that clears the codestates table every Monday at midnight
/// </summary>
public class ClearCodestatesJob : IJob
{
    private readonly ILogger<ClearCodestatesJob> _logger;
    private readonly EventRepository _eventRepository;

    public ClearCodestatesJob(ILogger<ClearCodestatesJob> logger, EventRepository repository)
    {
        _logger = logger;
        _eventRepository = repository;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {

        _logger.LogInformation("Clearing codestates table");
        List<Guid> uniqueCodeStates = await _eventRepository.GetUniqueCodeStates();

        foreach (string folderName in Directory.EnumerateDirectories("codestates"))
        {
            if (!uniqueCodeStates.Contains(Guid.Parse(folderName.Split('\\').Last())))
            {
                Directory.Delete(folderName, true);
                _logger.LogInformation($"Deleted {folderName}");
            }
        }

        _logger.LogInformation("Finished clearing codestates table");
        return;
    }
}      