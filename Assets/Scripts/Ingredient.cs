using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Scriptable Objects/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;
    public GameObject worldPickupPrefab;
    public GameObject orbitalVisualPrefab;
}
