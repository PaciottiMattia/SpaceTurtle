using UnityEngine;

public class AudioSatellite : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null)
        {
            // 1. Piccola variazione di tono (Pitch) per realismo
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            
            // 2. Volume al massimo (lasciamo che sia la distanza 3D a regolarlo)
            audioSource.volume = 0.6f; 
            
            // 3. IL RITARDO (Ecco la modifica)
            // Invece di Play(), usiamo PlayDelayed(1f).
            // Il suono aspetta 1 secondo esatto e poi parte.
            audioSource.PlayDelayed(1f);
        }
    }
}