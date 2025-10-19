using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class IngredientsListUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ingredientsContainer;
    [SerializeField] private GameObject ingredientItemPrefab;
    [SerializeField] private RectTransform containerRect;
    
    [Header("Animation")]
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector2 hiddenPosition = new Vector2(-300, 0); 
    [SerializeField] private Vector2 visiblePosition = new Vector2(20, 0); 
    
    [Header("Colors")]
    [SerializeField] private Color pendingColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color missingColor = Color.yellow;
    
    private List<IngredientItemUI> ingredientItems = new List<IngredientItemUI>();
    private Coroutine slideCoroutine;

    public void ShowIngredientsList(Drink drink)
    {
        // Clear old ingredients
        ClearIngredients();
        
        // Create ingredient items
        foreach (var drinkIngredient in drink.ingredients)
        {
            GameObject itemObj = Instantiate(ingredientItemPrefab, ingredientsContainer);
            IngredientItemUI itemUI = itemObj.GetComponent<IngredientItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Setup(drinkIngredient, pendingColor, completedColor, missingColor);
                ingredientItems.Add(itemUI);
            }
        }
        
        // Slide in
        gameObject.SetActive(true);
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideIn());
    }

    public void HideIngredientsList()
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideOut());
    }

    public void UpdateIngredientStatus(int ingredientIndex, bool isCompleted)
    {
        if (ingredientIndex >= 0 && ingredientIndex < ingredientItems.Count)
        {
            ingredientItems[ingredientIndex].SetCompleted(isCompleted);
        }
    }

    void ClearIngredients()
    {
        foreach (var item in ingredientItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        ingredientItems.Clear();
    }

    System.Collections.IEnumerator SlideIn()
    {
        float elapsed = 0f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            float curveValue = slideInCurve.Evaluate(t);
            
            containerRect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, curveValue);
            
            yield return null;
        }
        
        containerRect.anchoredPosition = visiblePosition;
    }

    System.Collections.IEnumerator SlideOut()
    {
        float elapsed = 0f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            float curveValue = slideInCurve.Evaluate(t);
            
            containerRect.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, curveValue);
            
            yield return null;
        }
        
        containerRect.anchoredPosition = hiddenPosition;
        gameObject.SetActive(false);
    }
}