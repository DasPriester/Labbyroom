using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single save unit in the list of saves to load
/// </summary>
public class SaveSlot : MonoBehaviour
{
    public SaveFile saveFile;

    [SerializeField] private Button loadButton;
    public Text title;

    private void Awake()
    {
        loadButton.onClick.AddListener(() => saveFile.Load());
    }

}
