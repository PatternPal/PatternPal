namespace PatternPal.Core.Checks;
 internal class InterfaceCheck : CheckBase
 {
     private readonly IEnumerable<ICheck> _checks;

     internal InterfaceCheck(Priority priority, 
         IEnumerable<ICheck> checks) : base(priority)
     {
         _checks = checks;
     }

    public override ICheckResult Check(
         RecognizerContext ctx,
         INode node)
     {
        throw new NotImplementedException("Interface Check was not implemented");
    }
 }
