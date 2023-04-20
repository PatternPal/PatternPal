#region

using PatternPal.Core.Recognizers;
using PatternPal.Core.Resources;
using PatternPal.Protos;

#endregion

namespace PatternPal.Core.Models;

/// <summary>
/// Represents a design pattern.
/// </summary>
public class DesignPattern
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    private static readonly DesignPattern s_Singleton = new(
        DesignPatternNameResources.Singleton,
        new SingletonRecognizer(),
        WikiPageResources.Singleton,
        Protos.Recognizer.Singleton );

    /// <summary>
    /// Creates a new <see cref="DesignPattern"/> instance.
    /// </summary>
    /// <param name="name">Pretty name of the pattern.</param>
    /// <param name="recognizer">Implementation of the recognizer for this pattern.</param>
    /// <param name="wikiPage">Url of the Wikipedia page of this pattern.</param>
    /// <param name="recognizerType">Type of this recognizer, as defined in the Protocol Buffer.</param>
    private DesignPattern(
        string name,
        IRecognizer recognizer,
        string wikiPage,
        Recognizer recognizerType)
    {
        Name = name;
        Recognizer = recognizer;
        WikiPage = wikiPage;
        RecognizerType = recognizerType;
    }

    /// <summary>
    /// Pretty name of the pattern, can be presented to the user.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Implementation of the recognizer for this pattern.
    /// </summary>
    internal IRecognizer Recognizer { get; }

    /// <summary>
    /// Url of the Wikipedia page of this pattern.
    /// </summary>
    public string WikiPage { get; }

    /// <summary>
    /// Type of this recognizer, as defined in the Protocol Buffer.
    /// </summary>
    public Recognizer RecognizerType { get; }

    /// <summary>
    /// List of supported design patterns.
    /// </summary>
    public static DesignPattern[ ] SupportedPatterns => new[ ]
                                                        {
                                                            s_Singleton,
                                                        };
}
