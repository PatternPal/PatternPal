#region

using PatternPal.Core.Resources;
using PatternPal.Protos;
using PatternPal.Recognizers;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Recognizers;

#endregion

namespace PatternPal.Core.Models;

/// <summary>
/// Represents a design pattern.
/// </summary>
public class DesignPattern
{
    /// <summary>
    /// Adapter pattern.
    /// </summary>
    private static readonly DesignPattern s_Adapter = new(
        DesignPatternNameResources.Adapter,
        new AdapterRecognizer(),
        WikiPageResources.Adapter,
        Protos.Recognizer.Adapter );

    /// <summary>
    /// Bridge pattern.
    /// </summary>
    private static readonly DesignPattern s_Bridge = new(
        DesignPatternNameResources.Bridge,
        new BridgeRecognizer(),
        WikiPageResources.Bridge,
        Protos.Recognizer.Bridge );

    /// <summary>
    /// Decorator pattern.
    /// </summary>
    private static readonly DesignPattern s_Decorator = new(
        DesignPatternNameResources.Decorator,
        new DecoratorRecognizer(),
        WikiPageResources.Decorator,
        Protos.Recognizer.Decorator );

    /// <summary>
    /// Factory Method pattern.
    /// </summary>
    private static readonly DesignPattern s_FactoryMethod = new(
        DesignPatternNameResources.FactoryMethod,
        new FactoryMethodRecognizer(),
        WikiPageResources.FactoryMethod,
        Protos.Recognizer.FactoryMethod );

    /// <summary>
    /// Observer pattern.
    /// </summary>
    private static readonly DesignPattern s_Observer = new(
        DesignPatternNameResources.Observer,
        new ObserverRecognizer(),
        WikiPageResources.Observer,
        Protos.Recognizer.Observer );

    /// <summary>
    /// Singleton pattern.
    /// </summary>
    private static readonly DesignPattern s_Singleton = new(
        DesignPatternNameResources.Singleton,
        new SingletonRecognizer(),
        WikiPageResources.Singleton,
        Protos.Recognizer.Singleton );

    /// <summary>
    /// State pattern.
    /// </summary>
    private static readonly DesignPattern s_State = new(
        DesignPatternNameResources.State,
        new StateRecognizer(),
        WikiPageResources.State,
        Protos.Recognizer.State );

    /// <summary>
    /// Strategy pattern.
    /// </summary>
    private static readonly DesignPattern s_Strategy = new(
        DesignPatternNameResources.Strategy,
        new StrategyRecognizer(),
        WikiPageResources.Strategy,
        Protos.Recognizer.Strategy );

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
    public IRecognizer Recognizer { get; }

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
                                                            s_Adapter,
                                                            s_Bridge,
                                                            s_Decorator,
                                                            s_FactoryMethod,
                                                            s_Observer,
                                                            s_Singleton,
                                                            s_State,
                                                            s_Strategy,
                                                        };
}
