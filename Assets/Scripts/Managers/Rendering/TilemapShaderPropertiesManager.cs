using Assets.Scripts.Classes.Static;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapShaderPropertiesManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = this.gameObject.GetComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = this.gameObject.GetComponent<TilemapRenderer>();
        tilemapRenderer.material.SetInteger("_MaxZ", tilemap.cellBounds.max.z.GetUnevenInteger());
    }
}
