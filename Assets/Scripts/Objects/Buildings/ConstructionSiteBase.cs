using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using ISelectable = Assets.Scripts.Commandables.ISelectable;

namespace Assets.Scripts.Objects.Buildings
{
    public class ConstructionSiteBase : MonoBehaviour
    {
        public BuildingBase ParentBuilding { get => parentBuilding; }

        [SerializeField]
        private static readonly Vector2Int constructionSitePrefabSize = new Vector2Int(8, 8);
        private BuildingBase parentBuilding;

        public static ConstructionSiteBase GetConstructionSite(BuildingBase forBuilding, ConstructionSiteBase usedPrefab)
        {
            var constructionSite = Instantiate(usedPrefab, forBuilding.transform);
            constructionSite.gameObject.transform.localScale = new Vector3((float)forBuilding.BuildingSize.x / (float)constructionSitePrefabSize.x,
                                                                           (float)forBuilding.BuildingSize.y / (float)constructionSitePrefabSize.y,
                                                                           1);
            constructionSite.parentBuilding = forBuilding;
            return constructionSite;
        }
    }
}
