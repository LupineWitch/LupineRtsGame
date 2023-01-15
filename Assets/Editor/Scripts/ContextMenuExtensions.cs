#if UNITY_EDITOR
using UnityEditor;
using Assets.Editor.Classes;
using UnityEngine.Tilemaps;
using System;
using System.Reflection;
using System.IO;
using Assets.Scripts.Managers;

public static class ContextMenuExtensions
{
    [MenuItem("CONTEXT/Tilemap/Export Tilemap to XML")]
    private static void TilemapToXML()
    {
        var map = Selection.activeGameObject.GetComponent<Tilemap>();
        string path = EditorUtility.SaveFilePanel("Save Tilemap to XML:",string.Empty, "ZasYTilemapLevel","xml");
        IMapSerialiser mapSerialiser = new XMLMapSerialiser();
        mapSerialiser.SerialiseTilemapToAFile(map, path, Path.GetFileNameWithoutExtension(path), map.gameObject.GetComponentInParent<MapManager>());
    }

    [MenuItem("CONTEXT/Tilemap/Export Tilemap to XML", true)]
    private static bool ValidateTilemapToXML()
    {
        return Selection.activeGameObject.GetComponent<Tilemap>() != default;
    }
}
#endif