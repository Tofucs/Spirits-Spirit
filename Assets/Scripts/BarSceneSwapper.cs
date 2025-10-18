using UnityEngine;
using System.Collections;   
public class BarSceneSwapper : MonoBehaviour
{
    public static BarSceneSwapper Instance;
    [SerializeField] private GameObject BartenderViewGroup;
    [SerializeField] private GameObject CustomerViewGroup;

    [SerializeField] private float swapDuration = 0.5f;
    [SerializeField] private AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); //bezier cubic

    private bool isBartenderSide = true; // start on bartender side

    [SerializeField] public PlayerController player;



    private SpriteRenderer[] frontSprites;
    private SpriteRenderer[] backSprites;

    void Start()
    {
        Instance = this;
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

    void Update()
    {

    }
}
