using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manage a Room
/// </summary>
public class Room : MonoBehaviour
{
    [SerializeField] private Transform access = null;
    [SerializeField] private bool hasAccessDoor = false;
    [SerializeField] private Vector2 accessDoor = Vector2.zero;
    [SerializeField] private WallManager accessWall = null;
    [SerializeField] private string prefabName = null;
    [SerializeField] private List<Recipe> possibleRecipes = new List<Recipe>();
    [SerializeField] private float recipeChance = 0f;
    [SerializeField] private GameObject recipe;

    private bool isTemporary = false;

    public string PrefabName { get => prefabName; set => prefabName = value; }
    public bool IsTemporary {
        get => isTemporary;
        set {
            isTemporary = value;
            foreach (WallManager wm in GetComponentsInChildren<WallManager>())
            {
                wm.IsTemporary = value;
            }
        }
    }

    private void Awake()
    {
        if(Random.value <= recipeChance)
        {
            recipe.SetActive(true);
            foreach (Recipe rec in possibleRecipes) {
                if (rec.unlocked)
                    possibleRecipes.Remove(rec);
            }
            recipe.GetComponent<RecipeInteractable>().Recipe = possibleRecipes[Random.Range(0, possibleRecipes.Count)];
        }
    }

    public Transform AddAccessDoor()
    {
        if (hasAccessDoor)
        {
            accessWall.doors.Add(accessDoor);
            accessWall.UpdateWall();
        }

        return access;
    }
}
