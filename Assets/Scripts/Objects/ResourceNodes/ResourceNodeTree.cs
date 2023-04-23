using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Models.Entity;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.Scripts.Objects.ResourceNodes
{
    public class ResourceNodeTree : MonoBehaviour, ISerializableEntityComponent
    {
        public RtsResource Resource { get;  set; }
        public int Amount { get => amount; set => amount = value; }
        public bool CanBeMined => Resource != null && Amount > 0;
        public string PrefabName { get => prefabName; set => prefabName = value; }
        [JsonProperty]
        public Type ComponentsType { get => typeof(ResourceNodeTree);  }

        public float TimeToGather = 4f;//s

        [SerializeField][JsonProperty]
        private string resourceId;
        [SerializeField]
        private string prefabName;
        [SerializeField][JsonProperty]
        private int amount;

        protected void Awake()
        {
            Resource = new RtsResource(resourceId);
        }

        public int TryGather(int howMuch)
        {
            int ableToGet = Math.Min(Amount, howMuch);
            Amount = Math.Max(0, Amount - ableToGet);
            return ableToGet;
        }

    }
}
