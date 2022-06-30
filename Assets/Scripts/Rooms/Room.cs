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
            List<Recipe> notFoundRecipes = new List<Recipe>();
            foreach (Recipe rec in possibleRecipes) {
                if (!rec.unlocked)
                    notFoundRecipes.Add(rec);
            }
            if (notFoundRecipes.Count > 0)
            {
                recipe.SetActive(true);
                recipe.GetComponent<RecipeInteractable>().Recipe = notFoundRecipes[Random.Range(0, notFoundRecipes.Count)];
            }
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
