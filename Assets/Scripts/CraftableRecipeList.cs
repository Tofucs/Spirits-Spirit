using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CraftingRecipe
{
    public List<string> ingredientNames; // if hand is equal to this list as a set, you can craft a new item.
    public HoldableObject result;
}

[CreateAssetMenu(fileName = "CraftableRecipeList", menuName = "Scriptable Objects/CraftableRecipeList")]
public class CraftableRecipeList : ScriptableObject
{
    public List<CraftingRecipe> recipes;
}
