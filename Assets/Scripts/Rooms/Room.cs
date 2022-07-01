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
    [SerializeField] private SerializableDictionary<GameObject, float> chancedItems;

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


    public Transform AddAccessDoor()
    {
        if (hasAccessDoor)
        {
            if (Random.value <= recipeChance)
            {

                List<Recipe> notFoundRecipes = new List<Recipe>();
                foreach (Recipe rec in possibleRecipes)
                {
                    if (!rec.unlocked)
                        notFoundRecipes.Add(rec);
                }
                if (notFoundRecipes.Count > 0)
                {
                    recipe.GetComponent<RecipeInteractable>().Recipe = notFoundRecipes[Random.Range(0, notFoundRecipes.Count)];
                    recipe.SetActive(true);
                }

                foreach (KeyValuePair<GameObject, float> ci in chancedItems)
                {
                    if (Random.Range(0, 1) > ci.Value)
                        ci.Key.SetActive(true);
                }
            }
            accessWall.doors.Add(accessDoor);
            accessWall.UpdateWall();
        }

        return access;
    }
}
