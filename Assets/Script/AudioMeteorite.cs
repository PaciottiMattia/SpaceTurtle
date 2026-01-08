using UnityEngine;

public class AudioMeteorite : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null)
        {
            // 1. Variazione Tonalità (Pitch)
            // Valori bassi (0.8) = suono grosso e grave
            // Valori alti (1.2) = suono acuto e veloce
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            
            // 2. Variazione Volume
            // Così alcuni sono leggermente più silenziosi di altri
            audioSource.volume = Random.Range(0.8f, 1f); 
            
            // 3. Fai partire il suono
            audioSource.Play();
        }
    }
}