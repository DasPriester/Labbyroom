using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : Hideable
{
    [SerializeField] public KeyCode key = default;

    private void Awake()
    {
        GameObject.Find("Key").GetComponent<Text>().text = key.ToString();
        RectTransform text = GetComponentInChildren<Text>().GetComponent<RectTransform>();
        text.transform.localScale = new Vector3(-text.transform.localScale.x, text.transform.localScale.y, text.transform.localScale.z);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
