using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaveItem", menuName = "Scriptable Object/Task/HaveItem", order = 1)]
public class HaveItem : Task
{
    [SerializeField] private string name;

    private Inventory inv;

    private void Awake()
    {
        inv = GameObject.Find("Player").GetComponent<PlayerController>().GetComponent<Inventory>();
    }

    override public int Done()
    {
        return inv.GetAmmount(name);
    }
}
