using Assets.Scripts.Classes.GameData;
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
                { new RtsResource("wood_log"){ DisplayName = "Timber logs"}, 0 },
                { new RtsResource("wood_plank"){ DisplayName = "Planks"}, 0 },
                { new RtsResource("ore"){ DisplayName = "Ore"}, 0 },
            };
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
                resourceGauges.Find(g => g.ForResource == resource.IdName)?.ResourceValueChanged(resourceCounts[resource]); // we dont care if no gauge, just dont throw
            }
            else
                Debug.LogWarningFormat("Resource {0} doesn't exist", string.IsNullOrEmpty(resource.DisplayName) ? resource.IdName : resource.DisplayName);
        }
    }
}
