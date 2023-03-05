using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

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
