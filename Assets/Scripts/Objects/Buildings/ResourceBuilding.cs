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

namespace Assets.Scripts.Objects.Buildings
{
    public class ResourceBuilding : BuildingBase
    {
        public PlayerResourceManager ResourceManager { get; set; } 

        public bool TryToDepositResource(RtsResource resource, int amount)
        {

            return true;
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
