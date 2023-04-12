using Assets.Scripts.Classes.GameData;
using Assets.Scripts.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PlayerResourceManager : MonoBehaviour
    {
        public IReadOnlyDictionary<RtsResource, int> ResourceCounts { get; protected set; }
        public int OwnerId { get; private set; } = -1;
        public Dictionary<int, string> intList;

        protected Dictionary<RtsResource, int> resourceCounts;

        [SerializeField]
        private BasicCommandControler owner;
        [SerializeField]
        private List<ResourceGauge> resourceGauges;

        protected virtual void Awake()
        {
            OwnerId = owner.GetInstanceID();
            this.resourceCounts = new Dictionary<RtsResource, int>()
            {
                { new RtsResource("wood_log"){ DisplayName = "Timber logs"}, 0 },
                { new RtsResource("wood_plank"){ DisplayName = "Planks"}, 0 },
                { new RtsResource("stone"){ DisplayName = "Stone"}, 0 },
            };
        }
    
        public void ChangeResourceLevel(RtsResource resource, int value)
        {
            if (resourceCounts.ContainsKey(resource))
                resourceCounts[resource] += value;
            else
                Debug.LogWarningFormat("Resource {0} doesn't exists", string.IsNullOrEmpty(resource.DisplayName) ? resource.IdName : resource.DisplayName);
        }
    }
}
