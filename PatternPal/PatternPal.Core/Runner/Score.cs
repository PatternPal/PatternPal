namespace PatternPal.Core.Runner;

/// <summary>
/// The Score of a <see cref="ICheckResult"/>, used to sort the final root <see cref="ICheckResult"/>.
/// </summary>
public struct Score : IComparable< Score >,
                      IEquatable< Score >
{
    private int _knockout;
    private int _high;
    private int _mid;
    private int _low;

    /// <summary>
    /// Adds up every component of the right <see cref="Score"/> from the left <see cref="Score"/>.
    /// </summary>
    public static Score operator +(
        Score a,
        Score b)
    {
        if (a._mid > 0
            && b._mid > 0)
        {
        }
        return new()
        {
            _knockout = a._knockout + b._knockout,
            _high = a._high + b._high,
            _mid = a._mid + b._mid,
            _low = a._low + b._low
        };
    }

    /// <summary>
    /// Subtracts every component of the right <see cref="Score"/> from the left <see cref="Score"/>.
    /// </summary>
    public static Score operator -(
        Score a,
        Score b) =>
        new()
        {
            _knockout = a._knockout - b._knockout,
            _high = a._high - b._high,
            _mid = a._mid - b._mid,
            _low = a._low - b._low
        };

    /// <summary>
    /// Calculates and returns the <see cref="Score"/> property belonging to a <see cref="LeafCheckResult"/>.
    /// </summary>
    /// <param name="priority">The priority of the <see cref="LeafCheckResult"/></param>
    /// <param name="correct">Whether the <see cref="LeafCheckResult"/> was correct</param>
    internal static Score CreateScore(
        Priority priority,
        bool correct)
    {
        int score = correct
            ? 1
            : 0;
        return priority switch
        {
            Priority.Knockout => new Score
                                 {
                                     _knockout = score
                                 },
            Priority.High => new Score
                             {
                                 _high = score
                             },
            Priority.Mid => new Score
                            {
                                _mid = score
                            },
            Priority.Low => new Score
                            {
                                _low = score
                            },
            _ => throw new ArgumentException($"{priority} is an unhandled type of priority.")
        };
    }

    /// <summary>
    /// Calculates what the <see cref="Score"/> of the <see cref="NodeCheckResult"/> of a <see cref="NotCheck"/>,
    /// based on the <see cref="Score"/> of the <see cref="ICheckResult"/> of its <see cref="NotCheck.NestedCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of the parent <see cref="NotCheck"/></param>
    /// <param name="score">The <see cref="Score"/> of the computed <see cref="NotCheck.NestedCheck"/></param>
    /// <returns>The <see cref="Score"/> belonging to the parent <see cref="NotCheck"/></returns>
    internal static Score GetNot(
        Priority priority,
        Score score) =>
        new()
        {
            _knockout = priority == Priority.Knockout && score._knockout == 0
                ? 1
                : 0,
            _high = priority == Priority.High && score._high == 0
                ? 1
                : 0,
            _mid = priority == Priority.Mid && score._mid == 0
                ? 1
                : 0,
            _low = priority == Priority.Low && score._low == 0
                ? 1
                : 0
        };

    /// <inheritdoc />>
    public int CompareTo(
        Score other)
    {
        if (_knockout > other._knockout)
            return -1;
        if (_knockout < other._knockout)
            return 1;
        if (_high > other._high)
            return -1;
        if (_high < other._high)
            return 1;
        if (_mid > other._mid)
            return -1;
        if (_mid < other._mid)
            return 1;
        if (_low > other._low)
            return -1;
        if (_low < other._low)
            return 1;
        return 0;
    }

    public int PercentageTo(
        Score perfectScore)
    {
        int perfect = perfectScore._knockout * 4 + perfectScore._high * 3 + perfectScore._mid * 2 + perfectScore._low;
        int own = _knockout * 4 +_high * 3 + _mid * 2 + _low;

        return own / perfect * 100;
    }

    public bool Equals(
        Score other) => _knockout == other._knockout && _high == other._high && _mid == other._mid && _low == other._low;

    public override string ToString()
    {
        return $"Knockout: {_knockout}, High: {_high}, Mid: {_mid}, Low: {_low}";
    }
}
