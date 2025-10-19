using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DrinkIngredient
{
    public HoldableObject ingredient;
    public bool isMissing;
}


[CreateAssetMenu(fileName = "Drink", menuName = "Scriptable Objects/Drink")]
public class Drink : ScriptableObject
{
    public string drinkName;
    public Sprite drinkIcon;
    public List<DrinkIngredient> ingredients;
    public float timeLimit = 60f; // the timer, can be adjusted per object.
}
