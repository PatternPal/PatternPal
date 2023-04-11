namespace PatternPal.Core.Checks;
 internal class InterfaceCheck : ICheck
 {
     private readonly IEnumerable<ICheck> _checks;

     internal InterfaceCheck(
         IEnumerable<ICheck> checks)
     {
         _checks = checks;
     }

    public bool Check(
         RecognizerContext ctx,
         INode node)
     {
         return true;
     }
 }
