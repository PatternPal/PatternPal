namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class PrioritySortTests
{
    [Test]
    public Task HighMostImportant()
    {
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

        RecognizerRunner.PrioritySort(rootCheckResult, null);

        return Verifier.Verify(rootCheckResult);
    }

    [Test]
    public Task SameHighThenMid()
    {
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

        RecognizerRunner.PrioritySort(rootCheckResult, null);

        return Verifier.Verify(rootCheckResult);
    }

    [Test]
    public Task SameHighAndMidThenLow()
    {
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

        RecognizerRunner.PrioritySort(rootCheckResult, null);

        return Verifier.Verify(rootCheckResult);
    }

    [Test]
    public void NotCorrectlyInverted()
    {

    }
}
