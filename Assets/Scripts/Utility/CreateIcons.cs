#if (UNITY_EDITOR)

using UnityEngine;
using UnityEditor;
    
/// <summary>
/// Editor window to create icons for all PickUpInteractable prefabs
/// </summary>
public class CreateIcons : EditorWindow
{
    [MenuItem("Window/Create Icons")]
    public static void ShowWindow()
    {
        GetWindow<CreateIcons>("Create Icons");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create"))
        {
            CreateAllIcons();
        }
    }

    private void CreateAllIcons()
    {
        foreach (PickUpInteractable pi in Resources.LoadAll<PickUpInteractable>("Prefabs"))
        {
            if (pi.gameObject)
                Utility.CreateIconFor(new Item(pi.gameObject, pi.name, 0));
        }

        Debug.Log("Icons created");
    }
}
#endif