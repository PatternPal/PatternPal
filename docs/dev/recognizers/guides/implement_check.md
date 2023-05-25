# Implement a check
For more details, see [Checks](~/dev/recognizers/design/checks.md).

1. Create `<name>Check.cs` in the `PatternPal.Core/Checks` folder 
2. Decide whether the check is a `NodeCheck`, i.o.w. if the check can have child checks
   - If the check is a nodeCheck implement the `NodeCheck< TNode >`
   - Otherwise, implement the @PatternPal.Core.Checks.CheckBase
3. Add the check to the switch in the `NodeCheck< TNode >.RunCheck()` for a derivation from the `NodeCheck< INode >` one should probably add some extra logic. Otherwise this is done in the override of `Check()`.
4. Add a static method to the `CheckBuilder` class in `PatternPal.Core/Checks/ICheck.cs` returning an instance of the created check.
5. Create `<name>CheckTest.cs` in the `PatternPal.Tests/Checks` folder and check the functionality. There are some helpful methods in `PatternPal.Tests/Utils`.