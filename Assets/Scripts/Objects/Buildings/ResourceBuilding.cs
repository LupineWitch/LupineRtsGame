using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Managers;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects.Buildings
{
    public class ResourceBuilding : BuildingBase
    {
        public PlayerResourceManager ResourceManager { get; set; }

        public bool TryToDepositResource(RtsResource resource, int amount)
        {
            this.ResourceManager.ChangeResourceLevel(resource, amount);
            return true;
        }

        protected override void Awake()
        {
            OwnerChanged += AssignResourceManager;
            base.Awake();
        }

        private void AssignResourceManager(object sender, EventArgs e)
        {
            PlayerResourceManager[] resourceManagers = GameObject.FindObjectsOfType<PlayerResourceManager>();
            this.ResourceManager = resourceManagers.FirstOrDefault(manager => manager.OwnerId == this.Owner.GetInstanceID());
        }
    }
}
