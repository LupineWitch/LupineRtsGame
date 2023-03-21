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
        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; } = String.Empty;
        public virtual Sprite ButtonIcon { get; protected set; } = null;
        public abstract ContextCommandDelegator ContextCommandDelegator { get; }

        public CommandDirective()
        {
            Name = this.GetType().Name;
        }

        public abstract void OnDirectiveSelection(BasicCommandControler controller);
        
        public abstract void OnDirectiveDeselection(BasicCommandControler controller); 

        public override bool Equals(object obj)
        {
            return obj is CommandDirective directive &&
                   Name == directive.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
