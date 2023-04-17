using System;
using UnityEngine;

namespace Assets.Scripts.Classes.Events
{
    public delegate void BuildingCreatedEvent(object sender, BuildingEventArgs args);

    public delegate void BuildingStateChanged(object sender, BuildingEventArgs args);

    public delegate void BuildingDestroyedEvent(object sender, BuildingEventArgs args);

    public class BuildingEventArgs : EventArgs
    {
        public BuildingEventArgs(BoundsInt? occupiedBounds = null, float? buildProgress = null)
        {
            OccupiedBounds = occupiedBounds;
            BuildProgress = buildProgress;
        }

        public BoundsInt? OccupiedBounds { get; private set; }
        public float? BuildProgress { get; private set; }

    }
}
