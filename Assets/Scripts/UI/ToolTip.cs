using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : Hideable
{
    [SerializeField] private KeyCode key = default;

    public KeyCode Key {
        get { return key; }
        set { key = value; GetComponentInChildren<Text>().text = key.ToString(); }
    }

    private void Awake()
    {
        GetComponentInChildren<Text>().text = key.ToString();
        RectTransform text = GetComponentInChildren<Text>().GetComponent<RectTransform>();
        text.transform.localScale = new Vector3(-text.transform.localScale.x, text.transform.localScale.y, text.transform.localScale.z);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
