using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public SaveFile saveFile;

    [SerializeField] Button loadButton;
    [SerializeField] public Text title;

    private void Awake()
    {
        loadButton.onClick.AddListener(() => saveFile.Load());
    }

}
