using Assets.Scripts.Commandables;
using System;

namespace Assets.Scripts.Classes.Events
{
    public delegate void SelectedEvent(ISelectable sender, SelectedEventArgs e);

    public class SelectedEventArgs : EventArgs
    {
        public SelectedEventArgs(BasicCommandControler commandControler, bool selectionStatusChangedTo)
        {
            CommandControler = commandControler;
            SelectionChangedTo = selectionStatusChangedTo;
        }

        public BasicCommandControler CommandControler { get; }
        public bool SelectionChangedTo { get; }

    }
}