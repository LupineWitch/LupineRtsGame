using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Commandables.Directives
{
    public abstract class CommandDirective
    {
        public virtual string Name { get; protected set; } = "Default Command Name";
        public virtual string Description { get; protected set; } = String.Empty;
        public virtual Sprite ButtonIcon { get; protected set; } = null;
        public abstract ContextCommandDelegator ContextCommandDelegator { get; }
    }
}
