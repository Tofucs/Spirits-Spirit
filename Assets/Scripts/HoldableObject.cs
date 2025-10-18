using UnityEngine;

[CreateAssetMenu(fileName = "HoldableObject", menuName = "Scriptable Objects/HoldableObject")]
public class HoldableObject : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject worldPrefab;
}
