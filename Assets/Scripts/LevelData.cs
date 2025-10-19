using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class LevelDrink
{
    public Drink drink;
    public bool isGuaranteed;
    [Range(0f, 1f)] public float spawnChance = 0.5f; // if not guaranteed, adds variance to level on replay
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<LevelDrink> possibleDrinks;
    public int maxHearts = 3;
}
