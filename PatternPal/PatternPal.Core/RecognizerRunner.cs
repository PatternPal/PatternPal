#region

using PatternPal.Core.Models;
using PatternPal.Core.Recognizers;
using PatternPal.Protos;

#endregion

namespace PatternPal.Core;

/// <summary>
/// This class is the driver which handles running the recognizers.
/// </summary>
public class RecognizerRunner
{
    private readonly IList< DesignPattern > _patterns;
    private SyntaxGraph _graph;

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="recognizers">The recognizers to run.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IEnumerable< Recognizer > recognizers)
    {
        CreateGraph(files);

        // Get the design patterns which correspond to the given recognizers.
        _patterns = new List< DesignPattern >();
        foreach (Recognizer recognizer in recognizers)
        {
            // `Recognizer.Unknown` is the default value of the `Recognizer` enum, as required
            // by the Protocol Buffer spec. This value should never be used.
            if (recognizer == Recognizer.Unknown)
            {
                continue;
            }

            _patterns.Add(DesignPattern.SupportedPatterns[ ((int)recognizer) - 1 ]);
        }
    }

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="patterns">The design patterns for which to run the recognizers.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IList< DesignPattern > patterns)
    {
        CreateGraph(files);
        _patterns = patterns;
    }

    /// <summary>
    /// Creates a <see cref="SyntaxGraph"/> from the given files.
    /// </summary>
    /// <param name="files">The files from which to create a <see cref="SyntaxGraph"/></param>
    private void CreateGraph(
        IEnumerable< string > files)
    {
        _graph = new SyntaxGraph();
        foreach (string file in files)
        {
            string content = FileManager.MakeStringFromFile(file);
            _graph.AddFile(
                content,
                file);
        }
        _graph.CreateGraph();
    }

    public event EventHandler< RecognizerProgress > OnProgressUpdate;

    /// <summary>
    /// Run the recognizers.
    /// </summary>
    /// <returns>A list of <see cref="RecognitionResult"/>, one per given design pattern.</returns>
    public IList< ICheckResult > Run()
    {
        // If the graph is empty, we don't have to do any work.
        if (_graph.IsEmpty)
        {
            return new List< ICheckResult >();
        }

        SingletonRecognizer recognizer = new();

        // TODO: Create a new ctx object on every level of the checks tree, and set the current
        // entity accordingly.
        RecognizerContext ctx = new()
                                {
                                    Graph = _graph,
                                };

        // TODO: Handle priorities
        // TODO: Return ICheckResult directly, no list required
        IList< ICheckResult > results = new List< ICheckResult >();
        foreach (ICheck check in recognizer.Create())
        {
            foreach (IEntity entity in _graph.GetAll().Values)
            {
                ICheckResult result = check.Check(
                    ctx,
                    entity);

                if (CheckIsCorrect(result))
                {
                    results.Add(result);
                }
            }
        }

        return results;
    }

    // TODO: Handle returning partially correct results
    // TODO: Construct check results to be returned by the service, instead of returning results of
    // recognizers directly.
    private bool CheckIsCorrect(
        ICheckResult result)
    {
        switch (result)
        {
            case LeafCheckResult leafCheckResult:
            {
                return leafCheckResult.Correct;
            }
            case NodeCheckResult nodeCheckResult:
            {
                switch (nodeCheckResult.CollectionKind)
                {
                    case CheckCollectionKind.All:
                    {
                        foreach (ICheckResult childResult in nodeCheckResult.ChildrenCheckResults)
                        {
                            if (!CheckIsCorrect(childResult))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    case CheckCollectionKind.Any:
                    {
                        foreach (ICheckResult childResult in nodeCheckResult.ChildrenCheckResults)
                        {
                            if (CheckIsCorrect(childResult))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    default:
                        throw new ArgumentException($"Unsupported CheckCollectionKind '{nodeCheckResult.CollectionKind}'");
                }
            }
            default:
                throw new ArgumentException($"Unsupported check result '{result.GetType()}'");
        }
    }

    /// <summary>
    /// Report a progress update.
    /// </summary>
    /// <param name="percentage">The current progress as a percentage.</param>
    /// <param name="status">A status message associated with the current progress.</param>
    private void ReportProgress(
        int percentage,
        string status)
    {
        OnProgressUpdate?.Invoke(
            this,
            new RecognizerProgress
            {
                CurrentPercentage = percentage,
                Status = status
            });
    }
}
