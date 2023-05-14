using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PlayerResourceManager : MonoBehaviour
    {
        public IReadOnlyDictionary<RtsResource, int> ResourceCounts { get; protected set; }
        public int OwnerId { get; private set; } = -1;

        protected Dictionary<RtsResource, int> resourceCounts;

        [SerializeField]
        private CommandControllerBase owner;
        [SerializeField]
        private List<ResourceGauge> resourceGauges;

        protected virtual void Awake()
        {
            OwnerId = owner.GetInstanceID();
            this.resourceCounts = new Dictionary<RtsResource, int>()
            {
                { new RtsResource("wood"){ DisplayName = "Wood"}, 0 },
                { new RtsResource("food"){ DisplayName = "Food"}, 0 },
                { new RtsResource("ore"){ DisplayName = "Ore"}, 0 },
            };

            if( owner.Faction.WhoIsControlling == Faction.ControllerType.Player && (resourceGauges == null || resourceGauges.Count <= 0))
            {
                var refManager = this.GetReferenceManagerInScene();
                resourceGauges = new();
                resourceGauges.AddRange(refManager.ResourceReadout.GetComponentsInChildren<ResourceGauge>());
            }    
        }

        public void ChangeResourceLevel(string resourceIdName, int value)
        {
            var rtsResource = resourceCounts.Keys.FirstOrDefault(resource => resource.IdName == resourceIdName);
            if (rtsResource == default)
                throw new ArgumentException($"Can't find resource with {nameof(resourceIdName)}: \"{resourceIdName}\"");

            ChangeResourceLevel(rtsResource, value);
        }

        public void ChangeResourceLevel(RtsResource resource, int value)
        {
            if (resourceCounts.ContainsKey(resource))
            {
                resourceCounts[resource] += value;
                var gauge = resourceGauges.Find(g => g.ForResource == resource.IdName);
                if (gauge != null)
                    gauge.ResourceValueChanged(resourceCounts[resource]);
            }
            else
                Debug.LogWarningFormat("Resource {0} doesn't exist", string.IsNullOrEmpty(resource.DisplayName) ? resource.IdName : resource.DisplayName);
        }
    }
}
