using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : Hideable
{
    [SerializeField] public KeyCode key = default;

    private void Start()
    {
        GameObject.Find("Key").GetComponent<Text>().text = key.ToString();
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
