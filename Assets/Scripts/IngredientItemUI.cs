using UnityEngine;
using TMPro;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ingredientText;
    [SerializeField] private SpriteRenderer checkmark; // Changed from Image to SpriteRenderer
    
    private DrinkIngredient drinkIngredient;
    private Color pendingColor;
    private Color completedColor;
    private Color missingColor;
    private bool isCompleted = false;

    public void Setup(DrinkIngredient ingredient, Color pending, Color completed, Color missing)
    {
        drinkIngredient = ingredient;
        pendingColor = pending;
        completedColor = completed;
        missingColor = missing;
        
        // Set text
        string prefix = ingredient.isMissing ? "❓ " : "• ";
        ingredientText.text = prefix + ingredient.ingredient.itemName;
        
        ingredientText.color = ingredient.isMissing ? missingColor : pendingColor;
        
        if (checkmark != null)
            checkmark.gameObject.SetActive(false);
    }

    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        
        if (completed)
        {
            ingredientText.color = completedColor;
            ingredientText.fontStyle = FontStyles.Strikethrough;
            
            if (checkmark != null)
                checkmark.gameObject.SetActive(true);
        }
        else
        {
            ingredientText.color = drinkIngredient.isMissing ? missingColor : pendingColor;
            ingredientText.fontStyle = FontStyles.Normal;
            
            if (checkmark != null)
                checkmark.gameObject.SetActive(false);
        }
    }
}