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

    public void LoadMap()
    {
        assignedTilemap.ClearAllTiles();
        TileBase dirtTile = Resources.Load<TileBase>(Path.Combine(tilePalletsBasePath, "DirtTile"));

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

    }
}
