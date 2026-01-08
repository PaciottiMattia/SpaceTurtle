using UnityEngine;

public class RotazioneJarvis : MonoBehaviour
{
    [Header("Impostazioni")]
    [Tooltip("Velocità di rotazione. Positivo = Antiorario, Negativo = Orario")]
    public float velocita = -30f; 

    void Update()
    {
        // Ruota l'oggetto sull'asse Z (quello che esce dallo schermo)
        // Time.unscaledDeltaTime serve per farlo ruotare anche se il gioco è in Pausa!
        transform.Rotate(0, 0, velocita * Time.unscaledDeltaTime);
    }
}