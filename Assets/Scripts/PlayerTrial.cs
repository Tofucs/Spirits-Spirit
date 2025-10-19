using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private GameObject trailSpritePrefab;
    [SerializeField] private float spawnInterval = 0.1f; 
    [SerializeField] private float trailLifetime = 1f; 
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = new Color(1, 1, 1, 0);

    [SerializeField] PlayerController playerController;
    
    private float spawnTimer;
    [SerializeField] SpriteRenderer playerSprite;

    void Start()
    {
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval)
        {
            SpawnTrailSprite();
            spawnTimer = 0f;
        }
    }

    void SpawnTrailSprite()
    {
        GameObject trail = Instantiate(trailSpritePrefab, transform.position, transform.rotation);
        
        trail.transform.localScale = transform.localScale;
        SpriteRenderer trailRenderer = trail.GetComponent<SpriteRenderer>();
        if (trailRenderer != null && playerSprite != null)
        {
            trailRenderer.sprite = playerSprite.sprite;
            trailRenderer.color = startColor;

        }
        
        TrailFade fade = trail.AddComponent<TrailFade>();
        fade.lifetime = trailLifetime;
        fade.startColor = startColor;
        fade.endColor = endColor;
    }
}