using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Events
{
    public delegate void BuildingCreatedEvent(object sender, BuildingEventArgs args);

    public delegate void BuildingStateChanged(object sender, BuildingEventArgs args);

    public delegate void BuildingDestroyedEvent(object sender, BuildingEventArgs args);

    public class BuildingEventArgs : EventArgs
    {
        public IEnumerable<Vector3> OccupiedWorldPositions{ get; set; }

        public BuildingEventArgs() { }
    }
}
