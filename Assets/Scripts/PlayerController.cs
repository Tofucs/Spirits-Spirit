using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;

    private bool isDashing = false;
    public bool isSwitchingViews;


    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject spriteObject;
    [SerializeField] private AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); //bezier cubic
    [SerializeField] private float swapDuration = 0.5f;

    [SerializeField] LayerMask holdableLayer;
    [SerializeField] LayerMask interactionLayer;
    private LayerMask interactMask;

    [SerializeField] InventoryManager inventoryManager;

    private List<HoldableItem> nearbyItems = new List<HoldableItem>();

   
    void Start()
    {
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        isSwitchingViews = false;

        interactMask = holdableLayer.value | interactionLayer.value;
    }

    void Update()
    {
        HandleSwitchViews();
        HandleItemPickup();
        HandleMovement();
    }

    void StartSwap()
    {
        StartCoroutine(SwapCoroutine());
    }

    private IEnumerator SwapCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < swapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / swapDuration);
            float curveValue = swapCurve.Evaluate(t);


            yield return null;
        }
    }

    void HandleSwitchViews()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isSwitchingViews)
        {
            BarSceneSwapper.Instance.StartSwap();
            isSwitchingViews = true;
        }
    }

    void HandleItemPickup()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TryToPickup();
        }
    }

    void TryToPickup()
    {

        for (int i = 0; i < nearbyItems.Count; i++)
        {
            if (nearbyItems[i] == null)
                Debug.Log($"Item {i}: NULL/DESTROYED");
            else
                Debug.Log($"Item {i}: {nearbyItems[i].name}");
        }
    
    
        if (nearbyItems.Count == 0)
        {
            Debug.Log("No items nearby");
            return;
        }

        nearbyItems.RemoveAll(item => item == null); // liikeeeeeeee bruhhhh

        HoldableItem closest = nearbyItems[0];
        float closestDist = Vector2.Distance(transform.position, closest.transform.position);

        foreach (var item in nearbyItems)
        {
            float dist = Vector2.Distance(transform.position, item.transform.position);
            if (dist < closestDist)
            {
                closest = item;
                closestDist = dist;
            }
        }

        if (inventoryManager.AddItem(closest))
        {
            nearbyItems.Remove(closest);
            Destroy(closest.gameObject);
            Debug.Log($"Picked up {closest.itemData.itemName}");
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & interactMask.value) > 0)
        {
            HoldableItem item = other.GetComponent<HoldableItem>();
            if (item != null && !nearbyItems.Contains(item))
            {
                nearbyItems.Add(item);
                Debug.Log($"{item.itemData.itemName} is now in range");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & holdableLayer.value) > 0)
        {
            HoldableItem item = other.GetComponent<HoldableItem>();
            if (item != null)
            {
                nearbyItems.Remove(item);
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Input.GetKeyDown(KeyCode.J) && (interactMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (((1 << collision.gameObject.layer) & interactionLayer.value) > 0)
            {
                //handle interactions
            }
            if (((1 << collision.gameObject.layer) & holdableLayer.value) > 0)
            {
                
            }
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        float moveSpeed = isDashing ? 8f : 4f;

        rb.linearVelocity = movement * moveSpeed;
    }
}
