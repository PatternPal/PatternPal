namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class PrioritySortTests
{
    private VerifySettings _settings;

    [OneTimeSetUp]
    public void Init()
    {
        _settings = new();
        _settings.AddExtraSettings(_ => _.Converters.Add(new ScoreConverter()));
    }

    [Test]
    public Task KnockoutThenHighThenMidThenLow()
    {
        //Testing multiple nodecheckresults with knockouts is not that interesting, since an incorrect knockout gets pruned.
        //Thus all knockouts that are sorted by priority sorting must be correct.
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List<ICheckResult>
                    {
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.Knockout,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult()
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Knockout,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        },
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortResultsByPriorities(null,rootCheckResult);

        return Verifier.Verify(rootCheckResult, _settings);
    }

    [Test]
    public Task HighMostImportant()
    {
        //When it comes to priorities that can actually be incorrect in the final result, high is the most important one
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List<ICheckResult>
                    {
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        },
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortResultsByPriorities(null,rootCheckResult);

        return Verifier.Verify(rootCheckResult, _settings);
    }

    [Test]
    public Task SameHighThenMid()
    {
        //When it comes to priorities that can actually be incorrect in the final result, mid is the second most important one
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List<ICheckResult>
                    {
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        },
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortResultsByPriorities(null,rootCheckResult);

        return Verifier.Verify(rootCheckResult, _settings);
    }

    [Test]
    public Task SameHighAndMidThenLow()
    {
        //When it comes to priorities that can actually be incorrect in the final result, low is the least important one
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List<ICheckResult>
                    {
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = false,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        },
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            CollectionKind = CheckCollectionKind.Any,
                            Priority = Priority.High,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List<ICheckResult>
                            {
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.High,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Mid,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                },
                                new LeafCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    Priority = Priority.Low,
                                    Correct = true,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null,
                                }
                            }
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortResultsByPriorities(null,rootCheckResult);

        return Verifier.Verify(rootCheckResult, _settings);
    }

    [Test]
    public Task NotCorrectlyInverted()
    {
        NodeCheckResult rootCheckResult = 
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List<ICheckResult>
                    {
                        new NotCheckResult()
                        {
                            Priority = Priority.Low,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            FeedbackMessage = string.Empty,
                            NestedResult = new LeafCheckResult()
                            {
                                FeedbackMessage = string.Empty,
                                Priority = Priority.Mid,
                                Correct = false,
                                DependencyCount = 0,
                                MatchedNode = null,
                                Check = null,
                            }
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortResultsByPriorities(null,rootCheckResult);

        return Verifier.Verify(rootCheckResult, _settings);
    }
}

public class ScoreConverter : JsonConverter
{
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        if (value is Score score)
        {
            serializer.Serialize(
                writer,
                score.ToString());
        }
    }

    public override object? ReadJson(
        JsonReader reader,
        Type type,
        object? existingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(
        Type type)
    {
        // Check if the `type` we received is a `Score`.
        Type scoreType = typeof(Score);
        return scoreType.IsAssignableFrom(type);
    }

}
