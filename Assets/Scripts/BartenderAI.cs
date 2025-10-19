using UnityEngine;
using System.Collections;

public class BartenderAI : MonoBehaviour
{
    public static BartenderAI Instance { get; private set; }

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private DrinkStation drinkStation;
    [SerializeField] private Transform ingredientStorage; // this needs to be an array, but still not sure how to path find bartender

    // lowkey if no time have him move randomly when he "grabs" ingredient LMFAOAOAOOA
    
    private Drink currentDrink;
    private bool isWorking = false;
    private bool isWaitingForPlayer = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartMakingDrink(Drink drink)
    {
        currentDrink = drink;
        isWorking = true;
        isWaitingForPlayer = false;
        StartCoroutine(MakeDrinkRoutine());
    }

    IEnumerator MakeDrinkRoutine()
    {
        while (isWorking && !drinkStation.IsComplete)
        {
            DrinkIngredient nextIngredient = drinkStation.GetNextIngredient();
            
            if (nextIngredient == null) break;
            
            if (nextIngredient.isMissing)
            {
                // wait for player
                Debug.Log($"Bartender does not know where {nextIngredient.ingredient.itemName} is");
                isWaitingForPlayer = true;

                // bro will just stand there
                
                yield return new WaitUntil(() => !isWaitingForPlayer);
            }
            else
            {
                yield return StartCoroutine(FetchIngredient(nextIngredient.ingredient));
                
                drinkStation.TryAddIngredient(nextIngredient.ingredient);
            }
        }
        
        isWorking = false;
    }

    IEnumerator FetchIngredient(HoldableObject ingredient)
    {
        Debug.Log($"Bartender fetching {ingredient.itemName}");
        
        yield return StartCoroutine(MoveToPosition(ingredientStorage.position));
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(MoveToPosition(drinkStation.transform.position));
        
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void OnIngredientAdded()
    {
        // Player added a missing ingredient
        if (isWaitingForPlayer)
        {
            isWaitingForPlayer = false;
        }
    }
}