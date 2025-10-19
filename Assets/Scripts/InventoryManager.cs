using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxHeldItems = 4;
    private List<InventoryItem> items = new List<InventoryItem>();
    private int activeItemIndex = -1;
    public InventoryItem ActiveItem => items.Count > 0 ? items[activeItemIndex] : null;
    [SerializeField] private Transform playerCenter;

    private float orbitRadius = 1f;
    private float orbitSpeed = 20f;

    [SerializeField] private float dropDistance = 1.5f;

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleActiveItem(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CycleActiveItem(1);
        }

        UpdateItemPositions();
    }

    void UpdateItemPositions()
    {
        for (int i = 0; i < items.Count; i++)
        {
            float angle = (360f / items.Count) * i + Time.time * 50f;
            float rad = angle * Mathf.Deg2Rad;
            float radius = orbitRadius + 0.5f + 0.5f * Mathf.Sin(1.3f * Time.time);
            Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
            items[i].visual.localPosition = playerCenter.position + offset;
            items[i].visual.Rotate(Vector3.forward * orbitSpeed * Time.deltaTime);

            float scale = (i == activeItemIndex) ? 1.2f : 0.8f;
            items[i].visual.localScale = Vector3.Lerp(
                items[i].visual.localScale, 
                Vector3.one * scale, 
                Time.deltaTime * 10f
            );
        }
    }

    void CycleActiveItem(int dir)
    {
        if (items.Count == 0) return;

        activeItemIndex = (activeItemIndex + dir) % items.Count;
        if (activeItemIndex < 0) activeItemIndex += items.Count;
    }

    public bool AddItem(HoldableItem worldItem)
    {
        if (this.items.Count >= maxHeldItems) return false;

        GameObject itemVisual = Instantiate(worldItem.itemData.visualPrefab,
            playerCenter.position,
            Quaternion.identity
        );

        items.Add(new InventoryItem
        {
            data = worldItem.itemData,
            visual = itemVisual.transform
        });

        if (items.Count == 1)
            activeItemIndex = 0;
        return true;
    }
    
    public bool AddItemFromData(HoldableObject itemData)
    {
        if (this.items.Count >= maxHeldItems) return false;

        GameObject itemVisual = Instantiate(
            itemData.visualPrefab,
            playerCenter.position,
            Quaternion.identity
        );
        
        items.Add(new InventoryItem { 
            data = itemData,
            visual = itemVisual.transform 
        });

        if (items.Count == 1) 
            activeItemIndex = 0;
        return true;
    }

    public void RemoveItem()
    {
        if (items.Count == 0) return;

        Destroy(items[activeItemIndex].visual.gameObject);
        items.RemoveAt(activeItemIndex);
        activeItemIndex = activeItemIndex < 0 ? 0 : activeItemIndex - 1;
    }

    public void DropActiveItem(Vector3 playerPosition, bool facingRight)
    {
        if (items.Count == 0) return;

        InventoryItem itemToDrop = items[activeItemIndex];
        Vector3 dir = facingRight ? Vector3.right : Vector3.left;

        Vector3 dropPosition = playerPosition + dir * dropDistance;

        GameObject droppedObj = Instantiate(
            itemToDrop.data.worldPrefab,
            dropPosition,
            Quaternion.identity
        );
        
        HoldableItem holdableComponent = droppedObj.GetComponent<HoldableItem>();
        if (holdableComponent == null)
        {
            holdableComponent = droppedObj.AddComponent<HoldableItem>();
        }
        holdableComponent.itemData = itemToDrop.data;

        // 6
        droppedObj.layer = 7; // sooooo hardcode coded
        Destroy(itemToDrop.visual.gameObject);
        items.RemoveAt(activeItemIndex);

        if (items.Count > 0)
        {
            activeItemIndex = Mathf.Clamp(activeItemIndex, 0, items.Count - 1); // ehhhhhh honestly yea although itd be cool to not have any active items
        }
        else
        {
            activeItemIndex = -1;
        }

    }

    void OnActiveItemChanged()
    {
        // does this need to change at all?? 
    }
}

[System.Serializable]
public class InventoryItem
{
    public HoldableObject data;
    public Transform visual;
}
