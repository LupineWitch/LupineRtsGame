using Assets.Scripts.Classes.GameData;
using System;
using UnityEngine;

namespace Assets.Scripts.Objects.ResourceNodes
{
    public class ResourceNodeTree : MonoBehaviour
    {
        public RtsResource Resource { get; protected set; }
        public int Amount { get => amount; protected set => amount = value; }
        public bool CanBeMined => Resource != null && Amount > 0;
        public float TimeToGather = 4f;//s

        [SerializeField]
        private string resourceId;
        [SerializeField]
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
