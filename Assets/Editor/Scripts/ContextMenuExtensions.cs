#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Tilemaps;
using System;
using System.Reflection;
using System.IO;
using Assets.Scripts.Managers;
using Assets.Scripts.Classes.Serialisers;

public static class ContextMenuExtensions
{
    private static IMapSerialiser mapSerialiser = new JsonMapSerialiser();

    [MenuItem("CONTEXT/Tilemap/Export Tilemap to file")]
    private static void TilemapToFile()
    {
        var map = Selection.activeGameObject.GetComponent<Tilemap>();
        string path = EditorUtility.SaveFilePanel("Save Tilemap to a file:", string.Empty, "ZasYTilemapLevel","json");
        mapSerialiser.SerialiseTilemapToAFile(map, path, Path.GetFileNameWithoutExtension(path), map.gameObject.GetComponentInParent<MapManager>());
    }

    [MenuItem("CONTEXT/Tilemap/Import File into selected Tilemap")]
    private static void FileToTilemap()
    {
        var map = Selection.activeGameObject.GetComponent<Tilemap>();
        string path = EditorUtility.OpenFilePanel("Import file as Tilemap:", string.Empty, "json");
        mapSerialiser.DeserialiseMapFromAFileToTileMap(map, path, map.gameObject.GetComponentInParent<MapManager>());
    }

    [MenuItem("CONTEXT/Tilemap/Export Tilemap to file", true)]
    [MenuItem("CONTEXT/Tilemap/Import File into selected Tilemap", true)]
    private static bool ValidateTilemapSerialisation() => Selection.activeGameObject.GetComponent<Tilemap>() != default;

}
#endif