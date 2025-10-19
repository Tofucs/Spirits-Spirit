using UnityEngine;

[CreateAssetMenu(fileName = "HoldableObject", menuName = "Scriptable Objects/HoldableObject")]
public class HoldableObject : ScriptableObject
{
    public string itemName;
    public bool isTool;
    public Sprite icon;
    public GameObject worldPrefab;

    public GameObject visualPrefab;
}
