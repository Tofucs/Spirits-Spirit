using UnityEngine;

[CreateAssetMenu(fileName = "InteractableObject", menuName = "Scriptable Objects/InteractableObject")]
public class InteractableObject : ScriptableObject
{
    public string itemName;
    public GameObject worldPrefab;

    public bool triggersEvent;

    // EVENT TRIGGER HOW
    public HoldableObject holdableObject;

    void Start()
    {
        if (triggersEvent)
        {
            holdableObject = null;
        }
    }
}
