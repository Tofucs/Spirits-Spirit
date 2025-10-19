using UnityEngine;
using TMPro;

public class DrinkTimerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI drinkNameText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Animation")]
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0, 100); // Off-screen above
    [SerializeField] private Vector2 visiblePosition = new Vector2(0, -50); // Top center
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private float warningThreshold = 0.5f; // 50%
    [SerializeField] private float dangerThreshold = 0.25f; // 25%
    
    private float maxTime;
    private float currentTime;
    private bool isActive = false;
    private Coroutine slideCoroutine;

    void Start()
    {

        gameObject.SetActive(false);
    }

    public void ShowDrinkTimer(Drink drink)
    {
        gameObject.SetActive(true);
        
        drinkNameText.text = drink.drinkName;
        maxTime = drink.timeLimit;
        currentTime = maxTime;
        UpdateTimerDisplay();
        
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideIn());
        
        isActive = true;
    }

    public void HideTimer()
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideOut());
        isActive = false;
    }

    public void UpdateTimer(float timeRemaining)
    {
        if (!isActive) return;
        
        currentTime = timeRemaining;
        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timeText.text = $"{seconds}s";
        
        // Change color based on time remaining
        float fillAmount = currentTime / maxTime;
        
        if (fillAmount > warningThreshold)
        {
            timeText.color = normalColor;
        }
        else if (fillAmount > dangerThreshold)
        {
            timeText.color = warningColor;
        }
        else
        {
            timeText.color = dangerColor;
        }
    }

    System.Collections.IEnumerator SlideIn()
    {
        float elapsed = 0f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            float curveValue = slideInCurve.Evaluate(t);
            

            
            yield return null;
        }
        

    }

    System.Collections.IEnumerator SlideOut()
    {
        float elapsed = 0f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            float curveValue = slideInCurve.Evaluate(t);
            

            
            yield return null;
        }
        

        gameObject.SetActive(false);
    }
}