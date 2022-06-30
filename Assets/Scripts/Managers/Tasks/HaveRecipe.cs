using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HaveRecipe : Task
{

    [SerializeField] private List<Recipe> recipes;
    public HaveRecipe()
    {
        recipes = new List<Recipe>();
    }

    public HaveRecipe(List<Recipe> recipes)
    {
        this.recipes = recipes;
    }

    override public float Done()
    {
        int k = 0;
        foreach (Recipe rec in recipes)
        {
            if (rec.unlocked)
                k++;
        }
        return k;
    }

    public override string Progress()
    {
        return "Recipes: " + Done();
    }
}
