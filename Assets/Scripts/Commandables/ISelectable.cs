using Assets.Scripts.Classes.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Commandables
{
    public interface ISelectable
    {
        public Sprite Preview { get; protected set; }
        public string DisplayLabel { get; protected set; }
        public bool TrySelect(BasicCommandControler selector);
        public bool TryUnselect(BasicCommandControler selector);
        public bool IsSelectedBy(BasicCommandControler possibleOwner);
        public bool CanBeSelectedBy(BasicCommandControler selector);
    }

}
