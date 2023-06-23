#region

using PatternPal.LoggingServer.Data;
using Quartz;

#endregion

namespace PatternPal.LoggingServer.LogJobs;

/// <summary>
/// Job that removes all redundant CodeStates (those that are not present in the database anymore) at midnight.
/// </summary>
public class ClearCodeStatesJob : IJob
{
    private readonly ILogger<ClearCodeStatesJob> _logger;
    private readonly EventRepository _eventRepository;

    public ClearCodeStatesJob(ILogger<ClearCodeStatesJob> logger, EventRepository repository)
    {
        _logger = logger;
        _eventRepository = repository;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {

        _logger.LogInformation("Clearing CodeState folders");
        List<Guid?> uniqueCodeStates = await _eventRepository.GetUniqueCodeStates();

        foreach (string folderName in Directory.EnumerateDirectories(Path.Combine(Directory.GetCurrentDirectory(), "CodeStates")))
        {
            DirectoryInfo currentDirectory = new(folderName);
            if (Guid.TryParse(currentDirectory.Name, out Guid directoryGuid))
            {
                if (!uniqueCodeStates.Contains(directoryGuid))
                {
                    Directory.Delete(folderName, true);
                    _logger.LogInformation($"Deleted {folderName}");
                }
            }
            else
            {
                _logger.LogWarning($"Could not parse {folderName} to a Guid");
            }
        }

        _logger.LogInformation("Finished clearing CodeStates-directory");
    }
}      
