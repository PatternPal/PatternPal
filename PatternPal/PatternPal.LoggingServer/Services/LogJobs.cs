#region

using PatternPal.LoggingServer.Data;
using Quartz;

#endregion

namespace PatternPal.LoggingServer.LogJobs;

// TODO Global CodeStates variable

/// <summary>
/// Job that removes all redundant CodeStates (those that are not present in the database anymore) at midnight.
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

        _logger.LogInformation("Clearing CodeState folders");
        List<Guid?> uniqueCodeStates = await _eventRepository.GetUniqueCodeStates();

        foreach (string folderName in Directory.EnumerateDirectories("CodeStates"))
        {
            if (!uniqueCodeStates.Contains(Guid.Parse(folderName.Split('\\').Last())))
            {
                Directory.Delete(folderName, true);
                _logger.LogInformation($"Deleted {folderName}");
            }
        }

        _logger.LogInformation("Finished clearing CodeStates-directory");
        return;
    }
}      
