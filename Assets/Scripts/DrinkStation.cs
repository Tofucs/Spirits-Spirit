using UnityEngine;
using System.Collections.Generic;

public class DrinkStation : MonoBehaviour
{
    [SerializeField] private Transform drinkPosition;
    private Drink currentDrink;
    private List<HoldableObject> addedIngredients = new List<HoldableObject>();
    
    public int CurrentIngredientIndex => addedIngredients.Count;
    public bool IsComplete => addedIngredients.Count >= currentDrink.ingredients.Count;

    [SerializeField] LayerMask layer;

    public void StartDrink(Drink drink)
    {
        currentDrink = drink;
        addedIngredients.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HoldableItem item = other.GetComponent<HoldableItem>();
        if (item != null)
        {
            if (item.itemData.isTool)
            {
                Debug.Log($"{item.itemData.itemName} is not edible, not adding to drink");
                return;
            }
            
            if (TryAddIngredient(item.itemData))
            {
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Item dropped but not added to drink");
            }
        }
    }

    public bool TryAddIngredient(HoldableObject ingredient)
    {
        if (currentDrink == null || IsComplete) return false;
        
        // can only add if next ingredient in list
        DrinkIngredient expectedIngredient = currentDrink.ingredients[CurrentIngredientIndex];
        
        if (ingredient.itemName == expectedIngredient.ingredient.itemName)
        {
            addedIngredients.Add(ingredient);
            Debug.Log($"Added {ingredient.itemName} to drink ({CurrentIngredientIndex}/{currentDrink.ingredients.Count})");
            
            GameManager.Instance.UpdateIngredientUI(CurrentIngredientIndex - 1, true);
            if (IsComplete)
            {
                OnDrinkComplete();
            }
            else
            {
                // Notify bartender
                BartenderAI.Instance?.OnIngredientAdded();
            }
            
            return true;
        }
        else
        {
            // idk do we just delete the item LOL that'd be hilarious. 
            Debug.Log($"Wrong ingredient! Expected {expectedIngredient.ingredient.itemName}");
            return false;
        }
    }

    void SpawnIngredientVisual(HoldableObject ingredient)
    {
        // FUCK NO I MEAN LIKE 
    }

    void OnDrinkComplete()
    {
        Debug.Log($"Drink complete: {currentDrink.drinkName}");
        GameManager.Instance.OnDrinkCompleted();
    }

    public DrinkIngredient GetNextIngredient()
    {
        if (currentDrink == null || IsComplete) return null;
        return currentDrink.ingredients[CurrentIngredientIndex];
    }
}