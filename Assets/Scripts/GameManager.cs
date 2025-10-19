using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level")]
    [SerializeField] private LevelData currentLevel;

    [Header("UI")]
    [SerializeField] private DrinkTimerUI timerUI; 
    [SerializeField] private IngredientsListUI ingredientsUI;
    
    // [Header("Objects")]
    [SerializeField] private BartenderAI bartender;
    [SerializeField] private DrinkStation drinkStation;
    [SerializeField] private InventoryManager playerInventory;
    [SerializeField] private CraftableRecipeList craftingRecipes;
    
    // [Header("UI")]
    // [SerializeField] private DrinkTimerUI timerUI;
    // [SerializeField] private HeartsUI heartsUI;
    
    /*** drink vars ***/
    private List<Drink> activeDrinkQueue = new List<Drink>();
    private int currentDrinkIndex = 0;
    private int hearts;
    private float currentTimer;
    private bool timerActive = false;
    public Drink CurrentDrink => currentDrinkIndex < activeDrinkQueue.Count
        ? activeDrinkQueue[currentDrinkIndex]
        : null;
        
    


    [SerializeField] private GameObject BartenderViewGroup;
    [SerializeField] private GameObject CustomerViewGroup;

    [SerializeField] private float swapDuration = 0.5f;
    [SerializeField] private AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); //bezier cubic

    private bool isBartenderSide = true; // start on bartender side

    [SerializeField] public PlayerController player;



    private SpriteRenderer[] frontSprites;
    private SpriteRenderer[] backSprites;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeLevel();
        if (BartenderViewGroup == null || CustomerViewGroup == null)
        {
            Debug.LogError("BartenderViewGroup or CustomerViewGroup is not found in inspector");
            return;
        }
        frontSprites = BartenderViewGroup.GetComponentsInChildren<SpriteRenderer>();
        backSprites = CustomerViewGroup.GetComponentsInChildren<SpriteRenderer>();

        SetGroupAlpha(BartenderViewGroup, 1f);
        SetGroupAlpha(CustomerViewGroup, 0f);
    }

    void InitializeLevel()
    {
        hearts = currentLevel.maxHearts;
        // heartsUI.SetHearts(hearts);
        activeDrinkQueue.Clear();

        foreach (var levelDrink in currentLevel.possibleDrinks)
        {
            if (levelDrink.isGuaranteed)
            {
                activeDrinkQueue.Add(levelDrink.drink);
            }
            else if (Random.value <= levelDrink.spawnChance)
            {
                activeDrinkQueue.Add(levelDrink.drink);
            }
        }

        StartNextDrink();
    }

    void StartNextDrink()
    {
        if (currentDrinkIndex >= activeDrinkQueue.Count)
        {
            OnLevelComplete();
            return;
        }

        Drink drink = CurrentDrink;
        drinkStation.StartDrink(drink);
        bartender.StartMakingDrink(drink);

        currentTimer = drink.timeLimit;
        timerActive = true;
        timerUI.ShowDrinkTimer(drink);
        ingredientsUI.ShowIngredientsList(drink);

        Debug.Log($"Drink ordered: {drink.drinkName}");
    }
    
    public void UpdateIngredientUI(int index, bool completed)
    {
        ingredientsUI.UpdateIngredientStatus(index, completed);
    }


    public void StartSwap()
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

            SetGroupAlpha(BartenderViewGroup, isBartenderSide ? 1f - curveValue : curveValue);
            SetGroupAlpha(CustomerViewGroup, isBartenderSide ? curveValue : 1f - curveValue);

            yield return null;
        }

        isBartenderSide = !isBartenderSide;
        player.isSwitchingViews = false;
    }

    private void SetGroupAlpha(GameObject group, float alpha)
    {
        SpriteRenderer[] sprites = group.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }
    }

    public void OnDrinkCompleted()
    {
        timerActive = false;
        timerUI.HideTimer();
        ingredientsUI.HideIngredientsList();
        Debug.Log($"Drink {CurrentDrink.drinkName} completed, move to next");

        currentDrinkIndex++;
        StartNextDrink();
    }

    void OnTimerExpired()
    {
        timerActive = false;
        hearts--;
        timerUI.HideTimer();
        ingredientsUI.HideIngredientsList();
        // heartsUI.SetHearts(hearts);

        Debug.Log("timer expired");

        if (hearts <= 0)
        {
            OnGameOver();
        }
        else
        {
            currentDrinkIndex++;
            StartNextDrink();
        }
    }

    void OnLevelComplete()
    {
        Debug.Log("Level Complete");
        // switch UI screen to score + next level
    }

    void OnGameOver()
    {
        Debug.Log("Game Over");
        // switch UI screen to restart? i guess.
    }
    
    public bool TryCraftItems(List<HoldableObject> ingredients)
    {
        List<string> ingredientNames = ingredients.Select(i => i.itemName).ToList();
        ingredientNames.Sort(); // if it was a set. would it be faster to comare? not sure if C# has set equals

        foreach (var recipe in craftingRecipes.recipes)
        {
            List<string> recipeNames = new List<string>(recipe.ingredientNames);
            recipeNames.Sort();

            if (ingredientNames.SequenceEqual(recipeNames))
            {
                Debug.Log($"Crafted: {recipe.result.itemName}");
                playerInventory.ClearInventory();
                playerInventory.AddItemFromData(recipe.result);
                return true;
            }
        }
        
        // yea we just fuck over the player haha get punished for curiosity
        Debug.Log("Invalid recipe! Items destroyed.");
        playerInventory.ClearInventory();
        return false;
    }


    void Update()
    {
        if (timerActive)
        {
            currentTimer -= Time.deltaTime;
            timerUI.UpdateTimer(currentTimer);
            
            if (currentTimer <= 0)
            {
                OnTimerExpired();
            }
        }
    }
}
