using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader 
{
    private Tilemap assignedTilemap;

    private const string tilePalletsBasePath = "Graphics\\Tilepallets\\DefaultPaletteAssets\\";
   
    public LevelLoader(Tilemap tilemap)
    {
        this.assignedTilemap = tilemap;
    }

    Vector3Int[] mountainCoords =
    {
        new Vector3Int(32,22,2),
        new Vector3Int(33,22,2),
        new Vector3Int(34,22,2),
        new Vector3Int(35,22,2),
        new Vector3Int(36,22,2),
        new Vector3Int(37,22,2),
        
        new Vector3Int(32,23,2),
        new Vector3Int(32,24,2),
        new Vector3Int(32,25,2),
        new Vector3Int(36,26,2),
        new Vector3Int(37,27,2),
    };

    public void LoadMap()
    {
        assignedTilemap.ClearAllTiles();
        TileBase dirtTile = Resources.Load<TileBase>(Path.Combine(tilePalletsBasePath, "DirtTile"));
        TileBase rockTile = Resources.Load<TileBase>(Path.Combine(tilePalletsBasePath, "RockTile"));

        Vector3Int cellCoords = Vector3Int.zero;
        for (int y = 0; y < 100; y++)
        {
            cellCoords.y = y;
            for (int x = 0; x < 100; x++)
            {
                cellCoords.x = x;
                assignedTilemap.SetTile(cellCoords, dirtTile);
            }
        }
        //Make Custom ImpassableMountain
        foreach(var cell in mountainCoords)
            assignedTilemap.SetTile(cell, rockTile);
    }
}
