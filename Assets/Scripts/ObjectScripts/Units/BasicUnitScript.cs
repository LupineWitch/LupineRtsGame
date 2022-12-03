using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicUnitScript : MonoBehaviour
{
    private Material outlineMaterial;
    public bool isSelected { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        outlineMaterial.SetInteger("_IsSelected", isSelected ? 1 : 0);
    }
}
