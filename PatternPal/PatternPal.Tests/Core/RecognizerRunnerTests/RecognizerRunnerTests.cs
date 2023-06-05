using PatternPal.SyntaxTree.Models.Entities;

namespace PatternPal.Tests.Core.RecognizerRunnerTests;

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
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = false,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        bool parentShouldBePruned = RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

        // The root node should be pruned.
        Assert.IsTrue(parentShouldBePruned);

        // The `ChildrenCheckResults` list is cleared before pruning the parent.
        Assert.AreEqual(
            rootCheckResult.ChildrenCheckResults.Count,
            0);
    }

    [Test]
    public void Node_Check_Is_Pruned_If_Empty()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                CollectionKind = CheckCollectionKind.Any,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
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
                                        MatchedNode = null,
                                        Check = null
                                    }
                                },
                            Priority = Priority.Low,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        bool rootShouldBePruned = RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                                                       MatchedNode = null,
                                                       Check = null
                                                   },
                                               DependencyCount = 0,
                                               MatchedNode = null,
                                               Check = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        // Nested not check throws ArgumentException.
        Assert.Throws< ArgumentException >(
            () => RecognizerRunner.PruneResults(
                null,
                rootCheckResult));
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
                                               MatchedNode = null,
                                               Check = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                CollectionKind = CheckCollectionKind.Any,
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
                                               MatchedNode = null,
                                               Check = null
                                           },
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                CollectionKind = CheckCollectionKind.Any,
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
                                                MatchedNode = null,
                                                Check = null
                                            }
                                        },
                                    Priority = Priority.Low,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null
                                },
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                                                MatchedNode = null,
                                                Check = null
                                            }
                                        },
                                    Priority = Priority.Low,
                                    DependencyCount = 0,
                                    MatchedNode = null,
                                    Check = null
                                },
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        },
                        new LeafCheckResult
                        {
                            FeedbackMessage = string.Empty,
                            Priority = Priority.Knockout,
                            Correct = true,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.PruneResults(
            null,
            rootCheckResult);

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
                                      MatchedNode = null,
                                      Check = null
                                  };
        LeafCheckResult result2 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = true,
                                      DependencyCount = 0,
                                      MatchedNode = null,
                                      Check = null
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
                MatchedNode = null,
                Check = null
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
                                      MatchedNode = null,
                                      Check = null
                                  };
        LeafCheckResult result2 = new()
                                  {
                                      FeedbackMessage = string.Empty,
                                      Priority = Priority.Knockout,
                                      Correct = true,
                                      DependencyCount = 0,
                                      MatchedNode = null,
                                      Check = null
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
                                                    MatchedNode = null,
                                                    Check = null
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
                MatchedNode = null,
                Check = null
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

    [Test]
    public Task All_Bad_Results_Are_Pruned_When_PruneAll_Enabled()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NodeCheckResult
                        {
                            Priority = Priority.Mid,
                            FeedbackMessage = String.Empty,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List< ICheckResult >
                                                   {
                                                       new LeafCheckResult
                                                       {
                                                           Correct = false,
                                                           Priority = Priority.High,
                                                           FeedbackMessage = String.Empty,
                                                           DependencyCount = 0,
                                                           MatchedNode = null,
                                                           Check = null
                                                       }
                                                   }
                        },
                        new LeafCheckResult
                        {
                            Correct = false,
                            Priority = Priority.Mid,
                            FeedbackMessage = String.Empty,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null
            };

        RecognizerRunner.SortCheckResults(rootCheckResult);
        RecognizerRunner.PruneResults(
            null,
            rootCheckResult,
            true);

        return Verifier.Verify(rootCheckResult);
    }

    [Test]
    public Task Good_Results_Not_Pruned_When_PruneAll_Enabled()
    {
        NodeCheckResult rootCheckResult =
            new()
            {
                FeedbackMessage = string.Empty,
                ChildrenCheckResults =
                    new List< ICheckResult >
                    {
                        new NodeCheckResult
                        {
                            Priority = Priority.Mid,
                            FeedbackMessage = String.Empty,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null,
                            ChildrenCheckResults = new List< ICheckResult >
                                                   {
                                                       new LeafCheckResult
                                                       {
                                                           Correct = true,
                                                           Priority = Priority.High,
                                                           FeedbackMessage = String.Empty,
                                                           DependencyCount = 0,
                                                           MatchedNode = null,
                                                           Check = null
                                                       }
                                                   }
                        },
                        new LeafCheckResult
                        {
                            Correct = false,
                            Priority = Priority.Mid,
                            FeedbackMessage = String.Empty,
                            DependencyCount = 0,
                            MatchedNode = null,
                            Check = null
                        }
                    },
                Priority = Priority.Low,
                DependencyCount = 0,
                MatchedNode = null,
                Check = null,
                CollectionKind = CheckCollectionKind.Any
            };

        RecognizerRunner.SortCheckResults(rootCheckResult);
        RecognizerRunner.PruneResults(
            null,
            rootCheckResult,
            true);

        return Verifier.Verify(rootCheckResult);
    }

    [Test]
    public Task Results_Are_Pruned_Once()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateGraphFromInput(
            """
            public class C1
            {
            }

            internal class C2
            {
            }

            public class C3
            {
                public C3()
                {
                    new C1();
                    new C2();
                }
            }
            """);
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        IEntity classEntity = graph.GetAll()[ "C1" ];

        ClassCheck classCheck = Class(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Internal
            )
        );
        ICheck check = All(
            Priority.Knockout,
            classCheck,
            Class(
                Priority.Knockout,
                Creates(
                    Priority.Knockout,
                    classCheck
                )
            )
        );

        NodeCheckResult result = (NodeCheckResult)check.Check(
            ctx,
            classEntity);

        RecognizerRunner.SortCheckResults(result);

        Dictionary< INode, List< ICheckResult > > resultsByNode = new();
        RecognizerRunner.PruneResults(
            resultsByNode,
            result);

        return Verifier.Verify(result);
    }
}
