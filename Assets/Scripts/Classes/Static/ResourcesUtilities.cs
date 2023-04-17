using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Classes.Static
{
    public static class ResourcesUtilities
    {
        public static Sprite LoadSpriteFromSpritesheet(string sheetPath, string spriteName)
        {
            Sprite[] spritesFromSheet = Resources.LoadAll<Sprite>(sheetPath);
            return spritesFromSheet.FirstOrDefault(sprite => sprite.name.Equals(spriteName));
        }
    }
}
