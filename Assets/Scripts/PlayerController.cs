using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;

    private bool isDashing = false;
    public bool isSwitchingViews;

    private bool facingRight = true;


    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject spriteObject;
    [SerializeField] private AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); //bezier cubic
    [SerializeField] private float swapDuration = 0.5f;

    [SerializeField] LayerMask holdableLayer;
    [SerializeField] LayerMask interactionLayer;
    private LayerMask interactMask;

    [SerializeField] InventoryManager inventoryManager;

    private List<HoldableItem> nearbyItems = new List<HoldableItem>();
    private List<InteractableItem> nearbyInteractables = new List<InteractableItem>();
    private float time = 0f;

   
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
        FaceDirection(facingRight);
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
            GameManager.Instance.StartSwap();
            isSwitchingViews = true;
        }
    }

    void HandleItemPickup()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift);

        if (nearbyInteractables.Count != 0 && !shiftHeld)
        {
            // prioritize an interactable over any pick up ables
            nearbyInteractables.RemoveAll(item => item == null);

            InteractableItem nearest = nearbyInteractables[0];
            float nearestDist = Vector2.Distance(transform.position, nearest.transform.position);

            foreach (var item in nearbyInteractables)
            {
                float dist = Vector2.Distance(transform.position, item.transform.position);
                if (dist < nearestDist)
                {
                    nearest = item;
                    nearestDist = dist;
                }
            }

            //highlight

            if (Input.GetKeyDown(KeyCode.J))
            {
                TryToInteract(nearest);
                // Debug.Log("interactable nearby");

           
            }
            return;
        }


        if (nearbyItems.Count == 0)
        {
            // Debug.Log("No items nearby");
            if (Input.GetKeyDown(KeyCode.J))
            {
                inventoryManager.DropActiveItem(transform.position, facingRight);
            }

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

        // highlight closest

        if (Input.GetKeyDown(KeyCode.J))
        {
            TryToPickup(closest);
        }
    }
    
    void TryToInteract(InteractableItem closest)
    {
        // idk trigger the spawn of the holdable object that the interactable spawns, or the event
        Debug.Log($"Interacted with {closest.itemData.itemName}");
        if (closest.itemData.triggersEvent)
        {

        }
        else
        {
            if (inventoryManager.AddItemFromData(closest.itemData.holdableObject))
            {
                Debug.Log($"Picked up item from {closest.itemData.itemName}");
            }
            else
            {
                Debug.Log("Inventory full!");
            }
        }
    }

    void TryToPickup(HoldableItem closest)
    {
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
            if (((1 << other.gameObject.layer) & holdableLayer.value) > 0)
            {
                HoldableItem item = other.GetComponent<HoldableItem>();
                if (item != null && !nearbyItems.Contains(item))
                {
                    nearbyItems.Add(item);
                }
            }
            if (((1 << other.gameObject.layer) & interactionLayer.value) > 0)
            {
                InteractableItem item = other.GetComponent<InteractableItem>();
                if (item != null && !nearbyInteractables.Contains(item))
                {
                    nearbyInteractables.Add(item);
                    Debug.Log($"{item.itemData.itemName} is now in range");
                }
            }
            // highlight the object
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
        if (((1 << other.gameObject.layer) & interactionLayer.value) > 0)
        {
            InteractableItem item = other.GetComponent<InteractableItem>();
            if (item != null)
            {
                nearbyInteractables.Remove(item);
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
    
    public void FaceDirection(bool facingRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX < 0)
        {
            facingRight = true;
        }
        else if (moveX > 0)
        {
            facingRight = false;
        }

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        float moveSpeed = isDashing ? 8f : 4f;

        time += Time.fixedDeltaTime;
        float bobbingVelocity = Mathf.Sin(time * 0.3f) * 0.04f * 2f * Mathf.PI;

        rb.linearVelocity = new Vector3(
            movement.x * moveSpeed, 
            movement.y * moveSpeed + bobbingVelocity, 
            0f
        );
    }
}
