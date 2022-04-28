using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<string, int> invDict = new Dictionary<string, int>();

    public void AddItem(string item)
    {
        invDict.TryGetValue(item, out int amount);
        if (amount > 0)
            invDict[item] = amount + 1;
        else
            invDict.Add(item, 1);
    }
}
