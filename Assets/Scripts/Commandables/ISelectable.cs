using Assets.Scripts.Classes.Events;
using System;
using UnityEngine;

namespace Assets.Scripts.Commandables
{

    public interface ISelectable
    {
        public event SelectedEvent Selected;
        public Sprite Preview { get; set; }
        public string DisplayLabel { get; set; }
        public bool TrySelect(BasicCommandControler selector);
        public bool TryUnselect(BasicCommandControler selector);
        public bool IsSelectedBy(BasicCommandControler possibleOwner);
        public bool CanBeSelectedBy(BasicCommandControler selector);
        public void RaiseSelectedEvent(object sender, EventArgs e);
    }

}
