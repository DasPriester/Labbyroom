using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for each item that can be moved
/// </summary>
public class Moveable : MonoBehaviour
{
    [SerializeField] protected string prefabName = "";
    public string PrefabName { get => prefabName; set => prefabName = value; }
}