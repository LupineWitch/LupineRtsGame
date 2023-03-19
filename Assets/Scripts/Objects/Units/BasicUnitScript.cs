using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Commandables;
using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class BasicUnitScript : EntityBase
{
    public float unitSpeed = 10f;

    private void Start()
    {
        defaultCommand = new ContextDirective();
        menuActions[0] = new MoveDirective();
        (this as EntityBase).DisplayLabel = "Soldier";
    }
}
