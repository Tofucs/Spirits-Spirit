using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    [Header("Light Reference")]
    [SerializeField] private Light2D light2D;
    
    [Header("Flicker Settings")]
    [SerializeField] private float baseIntensity = 1.0f;
    [SerializeField] private float flickerAmount = 1.0f;
    [SerializeField] private float flickerSpeed = 6f;
    
    [Header("Flicker Pattern")]
    [SerializeField] private bool useRandomFlicker = false;
    [SerializeField] private bool usePerlinNoise = true;
    
    private float randomOffset;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();
        
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (light2D == null) return;

        float flicker = 0f;
        
        if (usePerlinNoise)
        {
            flicker = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);
            flicker = (flicker - 0.5f) * 2f * flickerAmount; 
        }
        else if (useRandomFlicker)
        {
            if (Random.value < flickerSpeed)
            {
                flicker = Random.Range(-flickerAmount, flickerAmount);
            }
        }
        
        light2D.intensity = baseIntensity + flicker;
    }
}