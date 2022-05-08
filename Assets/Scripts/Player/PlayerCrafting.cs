using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField]
    SerializableDictionary<KeyCode, Recipe> keyMap;

    PlayerInventory inv = null;
    Dictionary<KeyCode, float> cooldown = new Dictionary<KeyCode, float>();

    private void Awake()
    {
        inv = GetComponentInChildren<PlayerInventory>();

        foreach (KeyCode key in keyMap.Keys)
        {
            cooldown.Add(key, 0);
        }
    }

    private void Update()
    {
        foreach (KeyCode key in keyMap.Keys)
        {
            if (cooldown[key] > 0)
                cooldown[key] -= Time.deltaTime;
            else
                cooldown[key] = 0;

            if (Input.GetKey(key) && cooldown[key] == 0)
            {
                cooldown[key] = 1;
                inv.CraftRecipe(keyMap[key]);
            }
        }
    }

}
