# Recognizer Runner

The @PatternPal.Core.RecognizerRunner class is the main entry point of the design pattern recognition process. It
is responsible for initializing the selected recognizers, creating the
[`SyntaxGraph`](syntax_graph.md) of the input files, as well as for processing the
results of the recognizers.

## Initialization

When a new instance of a `RecognizerRunner` is created, it receives a list of recognizers to run,
and also a list of files or project files to run the recognizers on. These C# source files are
passed to Roslyn for parsing, the resulting AST is consumed by the @PatternPal.SyntaxTree.SyntaxGraph. After this initial
setup has been completed, the runner is ready to run the recognizers.

## Running the Recognizers

The runner is responsible for instantiating the individual recognizers, as well as creating the root
@PatternPal.Core.RecognizerContext. Implementations of the individual checks which make up a recognizer reside in
their respective classes, the runner does not actually implement any checking logic. To start the
recognition process, the only thing the runner has to do, is call the root check with a context and
a node. As each check is responsible for requesting its own data from the `SyntaxGraph`, or passing
that data to its child checks, the runner does not have any nodes to pass to the root check. To
avoid having to deal with a potentially `null` node input argument in each check, the runner has a
sentinel node which it passes to the root check. As this root check is guaranteed to not directly
access this node, the actual implementation is irrelevant.

## Processing the Results

Each recognizer has a root check, which also produces the root @PatternPal.Core.ICheckResult value. These check
results form a tree, on which the runner does some post processing, to find the best matches and to
reduce the amount of superfluous feedback the user receives. This process is split into several
individual steps, which are described in more detail below.

### Presorting the Results

The first step is presorting the results. This sorting happens based on the dependency count of a
result, which is equal to the dependency count of the check which produced the result. The
dependency count of a check is defined as the number of checks it uses, and it is a transitive
property. For example, a @PatternPal.Core.Checks.RelationCheck of kind `Uses` has a dependency count of 1 + the dependency
count of the check it depends on. By sorting the results in ascending order, so results with a lower
dependency count are put before results with a higher dependency count, we have more opportunities
to prune results, as we are guaranteed to encounter the results of the checks which are depended on
before we encounter the results of checks which depend on those previous checks. This means that if
we can prune results which are used later on, we can also decide to prune the dependent results if
necessary, because we are guaranteed to have processed the results which are depended on.

### Pruning the Results

The second step is pruning the results. A result can only be pruned if its priority is `Knockout`,
and it is incorrect. All other results are preserved for later processing. Pruning is done
recursively, to find as many results to prune as possible. If all child results of a result are
pruned, the result itself can also be pruned. Furthermore, if the @PatternPal.Core.Checks.CheckCollectionKind of a result is
`All`, and at least one of its child results is pruned, that result can also be pruned.

After we have collected the child results which can be pruned, we check if any of the child results
are the result of a `RelationCheck`. As this check uses the results of prior checks, which might
have been pruned, we have to check if we can prune some of the results of the `RelationCheck`
because they reference results which have been pruned already. If we find any, we also prune these
results.

### Postsorting the Results

The third step is postsorting the results. This happens based on the @PatternPal.Core.Checks.Priority of the results.
Each @PatternPal.Core.LeafCheckResult is given a @PatternPal.Core.Runner.Score based on its `Priority` and whether is was correct or not. A @PatternPal.Core.NodeCheckResult gets a
`Score` equal to the sum of all the `Score`s of its children. After the `Score`s are calculated, the sorting happens
based first on the `Knockout` `Score`. If two `Knockout` `Score`s are equal, then it is based on the `High` `Score`s.
When those are also equal, it is based on the `Mid` `Score`s, and when those are equal as well, it is based on the
`Low` `Score`s.

## Score Calculation

To determine the correctness of a result, we look at the number of requirements the implementation
has correctly implemented. These counts are accumulated in a @PatternPal.Core.Runner.Score object.
By comparing the actual scores to the score of a perfect implementation, we get a percentage of how
correctly the requirement has been implemented. This percentage is used to determine the color of
the progressbar in the extension. A threshold is also applied to only show results with a total
correct score of over 50%, results with scores below that are not returned to the user.

### Calculating Perfect Scores

When we are calculating the scores based on the results which are still present after pruning, we
also calculate the perfect scores. These perfect scores need to account for results which may have
been pruned, and generate a score for them based on the results which would have been present if
they had not been pruned.
