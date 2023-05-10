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
    public IList< RecognitionResult > Run()
    {
        // If the graph is empty, we don't have to do any work.
        if (_graph.IsEmpty)
        {
            return new List< RecognitionResult >();
        }

        SingletonRecognizer recognizer = new();

        IEnumerable< ICheck > checkBuilders = recognizer.Create();
        Dictionary< string, IEntity >.ValueCollection entities = _graph.GetAll().Values;

        //List<ICheck> leastToMostDependantChecks = flattenChecks(new List<ICheck>(), checkBuilders.ToArray());

        
        

        RecognizerContext ctx = new()
                                {
                                    Graph = _graph,
                                };
        foreach (ICheck check in checkBuilders)
        {
            foreach (IEntity entity in entities)
            {
                //TODO make correct implementation using priorities
                check.Check(
                    ctx,
                    entity);
            }
            Console.WriteLine();
        }

        return new List< RecognitionResult >();
    }

    private List<ICheck> flattenChecks(List<ICheck> flattened, ICheck[] toFlatten)
    {
        for(int i = 0; i < toFlatten.Length; i++)
        {
            ICheck check = toFlatten[i];
            switch (check)
            {
                case ClassCheck classCheck:
                    //flattened.AddRange(flattenChecks(flattened, classCheck.));
                    flattened.Add(toFlatten[i]);
                    break;
                case CheckCollection:
                case ModifierCheck:
                case RelationCheck:
                case ParameterCheck:
                case MethodCheck methodCheck:
                case FieldCheck fieldCheck:
                case ConstructorCheck constructorCheck:
                case PropertyCheck propertyCheck:
                case NotCheck notCheck:
                case TypeCheck typeCheck:

                // Ensure all checks are handled.
                default:
                    throw new ArgumentException("This check doesn't exist.");
            }
        }
        return flattened;
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
