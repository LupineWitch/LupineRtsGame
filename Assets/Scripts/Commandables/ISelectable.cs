﻿using Assets.Scripts.Classes.Events;
using System;
using UnityEngine;

namespace Assets.Scripts.Commandables
{

    public interface ISelectable
    {
        public event SelectedEvent Selected;
        public Sprite Preview { get; set; }
        public string DisplayLabel { get; set; }
        public bool TrySelect(CommandControllerBase selector);
        public bool TryUnselect(CommandControllerBase selector);
        public bool IsSelectedBy(CommandControllerBase possibleOwner);
        public bool CanBeSelectedBy(CommandControllerBase selector);
        public void RaiseSelectedEvent(object sender, EventArgs e);
    }

}
