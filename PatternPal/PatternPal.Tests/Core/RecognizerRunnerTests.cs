namespace PatternPal.Tests.Core;

[TestFixture]
public class RecognizerRunnerTests
{
    [Test]
    public void Incorrect_Knockout_Leaf_Check_Is_Pruned()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = false,
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // One child is pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);
    }

    [Test]
    public void Correct_Knockout_Leaf_Check_Is_Not_Pruned()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // The child is not pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);
    }

    [Test]
    public void Parent_Should_Be_Pruned_If_Empty()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = false,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        bool parentShouldBePruned = RecognizerRunner.FilterResults(rootCheckResult);

        // The root node should be pruned.
        Assert.IsTrue(parentShouldBePruned);

        // The child isn't pruned, because the parent can be pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);
    }

    [Test]
    public void Node_Check_Is_Pruned_If_Empty()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new NodeCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            ChildrenCheckResults =
                                new List< ICheckResult >
                                {
                                    new LeafCheckResult
                                    {
                                        FeedbackMessage = string.Empty,
                                        Priority = Priority.Knockout,
                                        Correct = false,
                                        DependencyCount = 0,
                                        MatchedNode = null
                                    }
                                },
                            Priority = Priority.Low,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        bool rootShouldBePruned = RecognizerRunner.FilterResults(rootCheckResult);

        // Root should not be pruned.
        Assert.IsFalse(rootShouldBePruned);

        // Nested NodeCheckResult is pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);

        // The leftover child result is the leaf result.
        Assert.IsInstanceOf< LeafCheckResult >(rootCheckResult.ChildrenCheckResults[ 0 ]);
    }

    [Test]
    public void Nested_Not_Check_Throws()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NotCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            NestedResult = new NotCheckResult
                                           {
                                               FeedbackMessage = string.Empty,
                                               Priority = Priority.Knockout,
                                               NestedResult =
                                                   new LeafCheckResult
                                                   {
                                                       FeedbackMessage = string.Empty,
                                                       Priority = Priority.Knockout,
                                                       Correct = true,
                                                       DependencyCount = 0,
                                                       MatchedNode = null
                                                   },
                                               DependencyCount = 0,
                                               MatchedNode = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        // Nested not check throws ArgumentException.
        Assert.Throws< ArgumentException >(() => RecognizerRunner.FilterResults(rootCheckResult));
    }

    [Test]
    public void Not_Check_Incorrect_Nested_Check_Not_Pruned()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NotCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            NestedResult = new LeafCheckResult
                                           {
                                               FeedbackMessage = string.Empty,
                                               Priority = Priority.Knockout,
                                               Correct = false,
                                               DependencyCount = 0,
                                               MatchedNode = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // Not check itself is not pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            2);

        // Not check still contains leaf check.
        Assert.IsInstanceOf< LeafCheckResult >(((NotCheckResult)rootCheckResult.ChildrenCheckResults[ 0 ]).NestedResult);
    }

    [Test]
    public void Not_Check_Correct_Nested_Check_Is_Pruned()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NotCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            NestedResult = new LeafCheckResult
                                           {
                                               FeedbackMessage = string.Empty,
                                               Priority = Priority.Knockout,
                                               Correct = true,
                                               DependencyCount = 0,
                                               MatchedNode = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // Not check itself is pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);

        // The leftover child result is the leaf result.
        Assert.IsInstanceOf< LeafCheckResult >(rootCheckResult.ChildrenCheckResults[ 0 ]);
    }

    [Test]
    public void Not_Check_Is_Pruned_With_Correct_Nested_Node_Check()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NotCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            NestedResult =
                                new NodeCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    ChildrenCheckResults =
                                        new List< ICheckResult >
                                        {
                                            new LeafCheckResult
                                            {
                                                FeedbackMessage = string.Empty,
                                                Priority = Priority.Knockout,
                                                Correct = true,
                                                DependencyCount = 0,
                                                MatchedNode = null
                                            }
                                        },
                                    Priority = Priority.Low,
                                    DependencyCount = 0,
                                    MatchedNode = null
                                },
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // Not check is pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            1);

        // Not check still contains leaf check.
        Assert.IsInstanceOf< LeafCheckResult >(rootCheckResult.ChildrenCheckResults[ 0 ]);
    }

    [Test]
    public void Not_Check_Is_Not_Pruned_With_Incorrect_Nested_Node_Check()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NotCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            NestedResult =
                                new NodeCheckResult
                                {
                                    FeedbackMessage = string.Empty,
                                    ChildrenCheckResults =
                                        new List< ICheckResult >
                                        {
                                            new LeafCheckResult
                                            {
                                                FeedbackMessage = string.Empty,
                                                Priority = Priority.Knockout,
                                                Correct = false,
                                                DependencyCount = 0,
                                                MatchedNode = null
                                            }
                                        },
                                    Priority = Priority.Low,
                                    DependencyCount = 0,
                                    MatchedNode = null
                                },
                            DependencyCount = 0,
                            MatchedNode = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        RecognizerRunner.FilterResults(rootCheckResult);

        // Not check is not pruned.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            2);

        // Root check still contains the not check.
        Assert.IsInstanceOf< NotCheckResult >(rootCheckResult.ChildrenCheckResults[ 0 ]);
    }

    [Test]
    public void Results_Are_Sorted_Based_On_Dependency_Count()
    {
        LeafCheckResult result1 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = false,
                                      DependencyCount = 1,
                                      MatchedNode = null
                                  };
        LeafCheckResult result2 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = true,
                                      DependencyCount = 0,
                                      MatchedNode = null
                                  };

        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        result1,
                        result2
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        // Guarantee order.
        Assert.Greater(
            result1.DependencyCount,
            result2.DependencyCount);

        RecognizerRunner.SortCheckResults(rootCheckResult);

        // Results are sorted.
        Assert.AreEqual(
            result2,
            rootCheckResult.ChildrenCheckResults[ 0 ]);
        Assert.AreEqual(
            result1,
            rootCheckResult.ChildrenCheckResults[ 1 ]);
    }

    [Test]
    public void Results_Are_Sorted_Recursively()
    {
        LeafCheckResult result1 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = false,
                                      DependencyCount = 1,
                                      MatchedNode = null
                                  };
        LeafCheckResult result2 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = true,
                                      DependencyCount = 0,
                                      MatchedNode = null
                                  };

        NodeCheckResult nestedNodeCheckResult = new()
                                                {
                                                    FeedbackMessage = string.Empty,
                                                    ChildrenCheckResults =
                                                        new List< ICheckResult >
                                                        {
                                                            result1,
                                                            result2
                                                        },
                                                    Priority = Priority.Low,
                                                    DependencyCount = 3,
                                                    MatchedNode = null
                                                };

        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        nestedNodeCheckResult,
                        result1,
                        result2
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null
            };

        // Guarantee order.
        Assert.Greater(
            nestedNodeCheckResult.DependencyCount,
            result1.DependencyCount);

        // Guarantee order.
        Assert.Greater(
            result1.DependencyCount,
            result2.DependencyCount);

        RecognizerRunner.SortCheckResults(rootCheckResult);

        // Top-level results are sorted.
        Assert.AreEqual(
            result2,
            rootCheckResult.ChildrenCheckResults[ 0 ]);
        Assert.AreEqual(
            result1,
            rootCheckResult.ChildrenCheckResults[ 1 ]);
        Assert.AreEqual(
            nestedNodeCheckResult,
            rootCheckResult.ChildrenCheckResults[ 2 ]);

        // Results are sorted recursively.
        Assert.AreEqual(
            result2,
            nestedNodeCheckResult.ChildrenCheckResults[ 0 ]);
        Assert.AreEqual(
            result1,
            nestedNodeCheckResult.ChildrenCheckResults[ 1 ]);
    }
}
