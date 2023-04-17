using UnityEngine;

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
