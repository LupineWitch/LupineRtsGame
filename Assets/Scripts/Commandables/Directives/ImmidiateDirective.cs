﻿using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Commandables.Directives
{
    public abstract class ImmidiateDirective : CommandDirective
    {
        public override ContextCommandDelegator ContextCommandDelegator => null;
        public abstract void ExecuteImmidiatly(BasicCommandControler commander, List<ISelectable> selectedObjects);
        
    }
}
