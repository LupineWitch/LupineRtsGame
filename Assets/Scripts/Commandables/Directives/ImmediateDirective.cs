using Assets.Scripts.Controllers;
using System.Collections.Generic;

namespace Assets.Scripts.Commandables.Directives
{
    public abstract class ImmediateDirective : CommandDirective
    {
        public override ContextCommandDelegator ContextCommandDelegator => null;
        public abstract void ExecuteImmediately(BasicCommandControler commander, List<ISelectable> selectedObjects);

    }
}
