﻿namespace PatternPal.Core.Checks;
 internal class InterfaceCheck : ICheck
 {
     private readonly IEnumerable<ICheck> _checks;

     internal InterfaceCheck(
         IEnumerable<ICheck> checks)
     {
         _checks = checks;
     }

    public ICheckResult Check(
         RecognizerContext ctx,
         INode node)
     {
        throw new NotImplementedException("Interface Check was not implemented");
    }
 }
