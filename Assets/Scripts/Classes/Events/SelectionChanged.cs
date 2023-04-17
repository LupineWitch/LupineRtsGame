using Assets.Scripts.Commandables;
using System;
using System.Collections.Generic;


namespace Assets.Scripts.Classes.Events
{
    public delegate void SelectionChangedEvent(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs : EventArgs
    {
        private List<ISelectable> selectedEntities;

        public IReadOnlyCollection<ISelectable> SelectedEntities { get => selectedEntities; }

        public SelectionChangedEventArgs(List<ISelectable> selectedEntities)
        {
            this.selectedEntities = selectedEntities;
        }
    }
}
