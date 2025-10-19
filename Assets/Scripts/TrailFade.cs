using UnityEngine;

public class TrailFade : MonoBehaviour
{
    public float lifetime = 0.4f;
    public Color startColor = new Color(1, 1, 1, 0.1f);
    public Color endColor = new Color(1, 1, 1, 0);
    
    private SpriteRenderer spriteRenderer;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
        }
        
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}