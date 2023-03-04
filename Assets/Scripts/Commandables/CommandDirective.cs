using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Commandables
{
    public abstract class CommandDirective
    {
        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual Sprite ButtonIcon { get; protected set; }
        public abstract ContextCommandDelegator ContextCommandDelegator { get; }
    }
}
