using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Models.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.ResourceNodes
{
    public class ResourceNodeBase : MonoBehaviour, ISerializableEntityComponent
    {

        public float TimeToGather = 4f;//s

        [SerializeField]
        [JsonProperty]
        private string resourceId;
        [SerializeField]
        [JsonProperty]
        private int amount;
        
        [SerializeField]
        private string prefabName;
        private RtsResource resource;
        [JsonProperty]
        public virtual Type ComponentsType { get => this.GetType(); }
        public RtsResource Resource { get => resource; set => resource = value; }
        public int Amount { get => amount; set => amount = value; }
        public bool CanBeMined => Resource != null && Amount > 0;
        public string PrefabName { get => prefabName; set => prefabName = value; }

        public virtual int TryGather(int howMuch)
        {
            int ableToGet = Math.Min(Amount, howMuch);
            Amount = Math.Max(0, Amount - ableToGet);
            return ableToGet;
        }

        protected virtual void Awake()
        {
            Resource = new RtsResource(resourceId);
        }
    }
}
