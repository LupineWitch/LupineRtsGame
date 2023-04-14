using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Managers;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TMPro.EditorUtilities;
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
